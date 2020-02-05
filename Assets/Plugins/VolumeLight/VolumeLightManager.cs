using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;

namespace PostEffect
{
    public class VolumeLightManager : PostEffectsBase
    {
        public static event Action<VolumeLightManager,Matrix4x4> PreRenderEvent;
        private bool enable;
        //降采样
        [Range(0, 2)]
        public int downSampling;
        public Shader blurShader;
        public Shader addShader;
        public Shader upSampleShader;

        private Material blurMat;
        private Material addMat;
        private Material upMat;

        [HideInInspector]
        public Material blurMaterial
        {
            get
            {
                blurMat = CheckShaderAndCreateMaterial(blurShader, blurMat);
                return blurMat;
            }
        }
        [HideInInspector]
        public Material addMaterial
        {
            get
            {
                addMat = CheckShaderAndCreateMaterial(addShader, addMat);
                return addMat;
            }
        }
        [HideInInspector]
        public Material upSampleMaterial
        {
            get
            {
                upMat = CheckShaderAndCreateMaterial(upSampleShader,upMat);
                return upMat;
            }
        }

        //3D Noise
        public Texture3D noiseTexture;
        public Texture2D spotTexutre;
        public Texture2D ditherTexture;

        //摄像机
        private Camera myCamera;
        [HideInInspector]
        public Camera thisCamera
        {
            get
            {
                if (myCamera == null)
                {
                    myCamera = GetComponent<Camera>();
                }
                return myCamera;
            }
        }

        //RT
        private List<RenderTexture> renderTextures;

        //CommandBuffer
        [HideInInspector]
        public List<CommandBuffer> commandBuffers;

        private Transform cameraTransform;

        //近截面的角
        private Matrix4x4 frustumCorners;
        //上一帧的投影矩阵
        private Matrix4x4 reprojectionMatrix;

        //当前的降采样级别
        private int downSamplingLevel;

        //获取RT
        public RenderTexture GetVolumeLightBuffer(){
            if(downSampling == 0)
            {
                return renderTextures[0];
            }
            else if(downSampling == 1)
            {
                return renderTextures[1];
            }
            else
            {
                return renderTextures[2];
            }
        }

        //获取深度
        public RenderTexture GetVolumeLightDepthBuffer()
        {
            if(downSampling == 1)
            {
                return renderTextures[3];
            }
            else if(downSampling == 2)
            {
                return renderTextures[4];
            }
            else
            {
                return null;
            }
        }

        private void Awake()
        {
            enable = true;
            cameraTransform = this.transform;
            downSamplingLevel = downSampling;
        }

        private void OnEnable()
        {
            if (blurMaterial == null || addMaterial == null || upSampleMaterial == null || noiseTexture == null || spotTexutre == null || ditherTexture == null)
            {
                enable = false;
            }

            if (thisCamera.actualRenderingPath == RenderingPath.Forward)
            thisCamera.depthTextureMode = DepthTextureMode.Depth;

            renderTextures = new List<RenderTexture>();
            commandBuffers = new List<CommandBuffer>();

            //刷新RT
            renderTextures.Add(null);
            renderTextures.Add(null);
            renderTextures.Add(null);
            renderTextures.Add(null);
            renderTextures.Add(null);
            RefreshResolution();

            commandBuffers.Add(new CommandBuffer());
            commandBuffers[0].name = "CB_BeforeLighting";

            if(thisCamera.actualRenderingPath == RenderingPath.Forward)
            {
                thisCamera.AddCommandBuffer(CameraEvent.AfterDepthTexture,commandBuffers[0]);
            }
            else
            {
                thisCamera.AddCommandBuffer(CameraEvent.BeforeLighting, commandBuffers[0]);
            }
        }

        private void OnDisable()
        {
            if (thisCamera.actualRenderingPath == RenderingPath.Forward)
            thisCamera.depthTextureMode = DepthTextureMode.None;

            if(thisCamera.actualRenderingPath == RenderingPath.Forward){
                thisCamera.RemoveCommandBuffer(CameraEvent.AfterDepthTexture,commandBuffers[0]);
            }
            else
            {
                thisCamera.RemoveCommandBuffer(CameraEvent.BeforeLighting,commandBuffers[0]);
            }

            ClearRt();
            ClearCb();
        }

        public void OnPreRender()
        {
            Matrix4x4 proj = Matrix4x4.Perspective(thisCamera.fieldOfView, thisCamera.aspect, 0.01f, thisCamera.farClipPlane);
            proj = GL.GetGPUProjectionMatrix(proj, true);
            reprojectionMatrix = proj * myCamera.worldToCameraMatrix;
            commandBuffers[0].Clear();
            if(downSampling == 2)
            {
                commandBuffers[0].Blit(null,renderTextures[3],upSampleMaterial,2);
                commandBuffers[0].Blit(null,renderTextures[4],upSampleMaterial,3);
                commandBuffers[0].SetRenderTarget(renderTextures[2]);
            }
            else if(downSampling == 1)
            {
                commandBuffers[0].Blit(null,renderTextures[3],upSampleMaterial,2);
                commandBuffers[0].SetRenderTarget(renderTextures[1]);
            }
            else
            {
                commandBuffers[0].SetRenderTarget(renderTextures[0]);
            }

            commandBuffers[0].ClearRenderTarget(false, true, new Color(0, 0, 0, 1));

            if(enable)
            {
                UpDateMaterialProperties();

                if(PreRenderEvent != null){
                    PreRenderEvent(this,reprojectionMatrix);
                }
            }
        }

        private void Update()
        {
            if(downSamplingLevel != downSampling)
            {
                downSamplingLevel = downSampling;
                RefreshResolution();
                return;
            }

            if((renderTextures[0].width != thisCamera.pixelWidth || renderTextures[0].height != thisCamera.pixelHeight))
            {
                RefreshResolution();
            }
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (enable)
            {
                if(downSampling == 2)
                {
                    RenderTexture temp = RenderTexture.GetTemporary(renderTextures[2].width, renderTextures[2].height, 0, RenderTextureFormat.ARGBHalf);
                    temp.filterMode = FilterMode.Bilinear;

                    Graphics.Blit(renderTextures[2],temp,blurMaterial,4);
                    Graphics.Blit(temp,renderTextures[2],blurMaterial,5);
                    Graphics.Blit(renderTextures[2],renderTextures[0],upSampleMaterial,1);

                    RenderTexture.ReleaseTemporary(temp);
                }
                else if(downSampling == 1)
                {
                    RenderTexture temp = RenderTexture.GetTemporary(renderTextures[1].width, renderTextures[1].height, 0, RenderTextureFormat.ARGBHalf);
                    temp.filterMode = FilterMode.Bilinear;

                    Graphics.Blit(renderTextures[1],temp,blurMaterial,2);
                    Graphics.Blit(temp,renderTextures[1],blurMaterial,3);
                    Graphics.Blit(renderTextures[1],renderTextures[0],upSampleMaterial,0);

                    RenderTexture.ReleaseTemporary(temp);
                }
                else
                {
                    RenderTexture temp = RenderTexture.GetTemporary(renderTextures[0].width, renderTextures[0].height, 0, RenderTextureFormat.ARGBHalf);
                    temp.filterMode = FilterMode.Bilinear;

                    Graphics.Blit(renderTextures[0],temp,blurMaterial,0);
                    Graphics.Blit(temp,renderTextures[0],blurMaterial,1);

                    RenderTexture.ReleaseTemporary(temp);
                }

                int screenWidth = thisCamera.pixelWidth;
                int screenHeight = thisCamera.pixelHeight;

                addMaterial.SetTexture("_Source",source);
                Graphics.Blit(renderTextures[0],destination,addMaterial,0);
            }
            else
            {
                Graphics.Blit(source,destination);
            }
        }

        private void UpDateMaterialProperties()
        {
            upSampleMaterial.SetTexture("_HalfResDepthBuffer", renderTextures[3]);
            upSampleMaterial.SetTexture("_HalfResColor", renderTextures[1]);
            upSampleMaterial.SetTexture("_QuarterResDepthBuffer", renderTextures[4]);
            upSampleMaterial.SetTexture("_QuarterResColor", renderTextures[2]);

            Shader.SetGlobalTexture("_DitherTexture", ditherTexture);
            Shader.SetGlobalTexture("_NoiseTexture", noiseTexture);
        }

        //更新屏幕分辨率
        private void RefreshResolution()
        {
            ClearRt();
            int screenWidth = thisCamera.pixelWidth;
            int screenHeight = thisCamera.pixelHeight;

            renderTextures[0] = new RenderTexture(screenWidth , screenHeight, 0, RenderTextureFormat.ARGBHalf);
            renderTextures[0].filterMode = FilterMode.Bilinear;
            renderTextures[0].name = "VolumeLightBuffer_0";

            for (int i = 1; i <= downSampling; i++)
            {
                renderTextures[i] =new RenderTexture(screenWidth/(i*2), screenHeight/(i*2), 0, RenderTextureFormat.ARGBHalf);
                renderTextures[i].filterMode = FilterMode.Bilinear;
                renderTextures[i].name = string.Format("VolumeLightBuffer_{0}", i);

                renderTextures[i+2] = new RenderTexture(screenWidth/(i*2), screenHeight/(i*2), 0, RenderTextureFormat.RFloat);
                renderTextures[i+2].name = string.Format("DepthBuffer_{0}", i);
                renderTextures[i+2].Create();
                renderTextures[i+2].filterMode = FilterMode.Point;
            }
        }

        //释放所有的RT
        private void ClearRt()
        {
            if (renderTextures != null)
            {
                for (int i = renderTextures.Count - 1; i > -1; i--)
                {
                    if (renderTextures[i])
                    {
                        Destroy(renderTextures[i]);
                        renderTextures[i] = null;
                    }
                }
            }
        }

        //释放所有CB并移除
        private void ClearCb() { 
            if (commandBuffers != null)
            {
                for (int i = commandBuffers.Count - 1; i > -1; i--)
                {
                    if (commandBuffers[i] != null)
                    {
                        commandBuffers[i].Release();
                        commandBuffers = null;
                    }
                }
            }
        }

        //用于重建世界坐标
        void GetFrustumCorners()
        {
            frustumCorners = Matrix4x4.identity;

            float fov = thisCamera.fieldOfView;
            float near = thisCamera.nearClipPlane;
            float aspect = thisCamera.aspect;

            float halfHeight = near * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
            Vector3 toRight = cameraTransform.right * halfHeight * aspect;
            Vector3 toTop = cameraTransform.up * halfHeight;

            Vector3 topLeft = cameraTransform.forward * near + toTop - toRight;
            float scale = topLeft.magnitude / near;

            topLeft.Normalize();
            topLeft *= scale;

            Vector3 topRight = cameraTransform.forward * near + toRight + toTop;
            topRight.Normalize();
            topRight *= scale;

            Vector3 bottomLeft = cameraTransform.forward * near - toTop - toRight;
            bottomLeft.Normalize();
            bottomLeft *= scale;

            Vector3 bottomRight = cameraTransform.forward * near + toRight - toTop;
            bottomRight.Normalize();
            bottomRight *= scale;

            frustumCorners.SetRow(0, bottomLeft);
            frustumCorners.SetRow(1, bottomRight);
            frustumCorners.SetRow(2, topRight);
            frustumCorners.SetRow(3, topLeft);
        }
    }

}

