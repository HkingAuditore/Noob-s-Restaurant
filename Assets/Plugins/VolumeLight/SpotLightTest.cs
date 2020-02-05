using UnityEngine;
using UnityEngine.Rendering;

namespace PostEffect
{
    [RequireComponent(typeof(Light))]
    public class SpotLightTest : PostEffectsBase
    {
        public Mesh spotMesh;
        public Shader spotShader;
        private Material spotMat;

        [HideInInspector]
        public Material spotMaterial
        {
            get
            {
                spotMat = CheckShaderAndCreateMaterial(spotShader, spotMat);
                return spotMat;
            }
        }
        
        private Light thisLight;
        private CommandBuffer commandBuffer;
        private CommandBuffer shadowCommandBuffer;

        [Range(1, 64)]
        public int SampleCount = 16;
        [Range(0.0f, 1.0f)]
        public float ScatteringCoef = 0.5f;
        [Range(0.0f, 0.1f)]
        public float ExtinctionCoef = 0.01f;
        [Range(0.0f, 1.0f)]
        public float SkyboxExtinctionCoef = 0.9f;
        [Range(0.0f, 0.999f)]
        public float HenyeyGreensteinG = 0.1f;
        public bool HeightFog = false;
        [Range(0, 0.5f)]
        public float HeightScale = 0.10f;
        public float GroundLevel = 0;
        public bool Noise = false;
        [Range(0.0f,0.2f)]
        public float NoiseScale = 0.015f;
        [Range(0.0f,10.0f)]
        public float NoiseIntensity = 1.0f;
        [Range(0.0f,1.0f)]
        public float NoiseIntensityOffset = 0.3f;
        public Vector2 NoiseVelocity = new Vector2(-3.0f, -3.0f);
        private bool _reversedZ = false;

        private void Awake() {
             if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D12 ||
                SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal || SystemInfo.graphicsDeviceType == GraphicsDeviceType.PlayStation4 ||
                SystemInfo.graphicsDeviceType == GraphicsDeviceType.Vulkan || SystemInfo.graphicsDeviceType == GraphicsDeviceType.XboxOne)
            {
                _reversedZ = true;
            }

            thisLight = this.GetComponent<Light>();
            thisLight.type = LightType.Spot;

            commandBuffer = new CommandBuffer();
            commandBuffer.name = "Spot Light CommandBuffer";
            thisLight.AddCommandBuffer(LightEvent.AfterShadowMap,commandBuffer);
        }
        
        private void OnEnable() {
            VolumeLightManager.PreRenderEvent += PreRenderEvent;
        }

        private void OnDisable() {
            VolumeLightManager.PreRenderEvent -= PreRenderEvent;
        }

        private void PreRenderEvent(VolumeLightManager renderer, Matrix4x4 viewProj){
            if(thisLight.enabled == false || spotMaterial == null || spotMesh == null){
                return;
            }
            spotMaterial.SetVector("_CameraForward", Camera.current.transform.forward);

            spotMaterial.SetInt("_SampleCount", SampleCount);
            spotMaterial.SetVector("_NoiseVelocity", new Vector4(NoiseVelocity.x, NoiseVelocity.y) * NoiseScale);
            spotMaterial.SetVector("_NoiseData", new Vector4(NoiseScale, NoiseIntensity, NoiseIntensityOffset));
            spotMaterial.SetVector("_MieG", new Vector4(1 - (HenyeyGreensteinG * HenyeyGreensteinG), 1 + (HenyeyGreensteinG * HenyeyGreensteinG), 2 * HenyeyGreensteinG, 1.0f / (4.0f * Mathf.PI)));
            spotMaterial.SetVector("_VolumetricLight", new Vector4(ScatteringCoef, ExtinctionCoef, thisLight.range, 1.0f - SkyboxExtinctionCoef));
            spotMaterial.SetTexture("_CameraDepthTexture", renderer.GetVolumeLightDepthBuffer());
            spotMaterial.SetFloat("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always); 

            if (HeightFog)
            {
                spotMaterial.EnableKeyword("HEIGHT_FOG");
                spotMaterial.SetVector("_HeightFog", new Vector4(GroundLevel, HeightScale));
            }
            else
            {
                spotMaterial.DisableKeyword("HEIGHT_FOG");
            }

            SetupSpotLight(renderer, viewProj);
        }

        private void SetupSpotLight(VolumeLightManager renderer, Matrix4x4 viewProj)
        {
            commandBuffer.Clear();

            int pass = 0;
            if (!IsCameraInSpotLightBounds())
            {
                pass = 1;     
            }
                    
            float scale = thisLight.range;
            float angleScale = Mathf.Tan((thisLight.spotAngle + 1) * 0.5f * Mathf.Deg2Rad) * thisLight.range;

            Matrix4x4 world = Matrix4x4.TRS(transform.position,transform.rotation, new Vector3(angleScale, angleScale, scale));
            Matrix4x4 view = Matrix4x4.TRS(thisLight.transform.position, thisLight.transform.rotation, Vector3.one).inverse;

            Matrix4x4 clip = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.0f), Quaternion.identity, new Vector3(-0.5f, -0.5f, 1.0f));
            Matrix4x4 proj = Matrix4x4.Perspective(thisLight.spotAngle, 1, 0, 1);

            spotMaterial.SetMatrix("_MyLightMatrix0", clip * proj * view);

            spotMaterial.SetMatrix("_WorldViewProj", viewProj * world);

            spotMaterial.SetVector("_LightPos", new Vector4(thisLight.transform.position.x, thisLight.transform.position.y, thisLight.transform.position.z, 1.0f / (thisLight.range * thisLight.range)));
            spotMaterial.SetVector("_LightColor", thisLight.color * thisLight.intensity);


            Vector3 apex = transform.position;
            Vector3 axis = transform.forward;

            Vector3 center = apex + axis * thisLight.range;
            float d = -Vector3.Dot(center, axis);

            spotMaterial.SetFloat("_PlaneD", d);        
            spotMaterial.SetFloat("_CosAngle", Mathf.Cos((thisLight.spotAngle + 1) * 0.5f * Mathf.Deg2Rad));

            spotMaterial.SetVector("_ConeApex", new Vector4(apex.x, apex.y, apex.z));
            spotMaterial.SetVector("_ConeAxis", new Vector4(axis.x, axis.y, axis.z));

            spotMaterial.EnableKeyword("SPOT");

            if (Noise)
                spotMaterial.EnableKeyword("NOISE");
            else
            {
                spotMaterial.DisableKeyword("NOISE");                
            }
            
            spotMaterial.SetTexture("_LightTexture0", renderer.spotTexutre);

            bool forceShadowsOff = false;
            if ((thisLight.transform.position - Camera.current.transform.position).magnitude >= QualitySettings.shadowDistance)
                forceShadowsOff = true;

            if (thisLight.shadows != LightShadows.None && forceShadowsOff == false)
            {
                clip = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, new Vector3(0.5f, 0.5f, 0.5f));

                if(_reversedZ)
                    proj = Matrix4x4.Perspective(thisLight.spotAngle, 1, thisLight.range, thisLight.shadowNearPlane);
                else
                    proj = Matrix4x4.Perspective(thisLight.spotAngle, 1, thisLight.shadowNearPlane, thisLight.range);

                Matrix4x4 m = clip * proj;
                m[0, 2] *= -1;
                m[1, 2] *= -1;
                m[2, 2] *= -1;
                m[3, 2] *= -1;

                spotMaterial.SetMatrix("_MyWorld2Shadow", m * view);
                spotMaterial.SetMatrix("_WorldView", m * view);

                spotMaterial.EnableKeyword("SHADOWS_DEPTH");
                commandBuffer.SetGlobalTexture("_ShadowMapTexture", BuiltinRenderTextureType.CurrentActive);
                commandBuffer.SetRenderTarget(renderer.GetVolumeLightBuffer());

                commandBuffer.DrawMesh(spotMesh, world, spotMaterial, 0, pass); 
                renderer.commandBuffers[0].DrawMesh(spotMesh,world,spotMaterial,0,pass); 
            }
            else
            {
                spotMaterial.DisableKeyword("SHADOWS_DEPTH");
                renderer.commandBuffers[0].DrawMesh(spotMesh, world, spotMaterial, 0, pass);
            }
        }

        private bool IsCameraInSpotLightBounds()
        {
            float distance = Vector3.Dot(thisLight.transform.forward, (Camera.current.transform.position - thisLight.transform.position));
            float extendedRange = thisLight.range + 1;
            if (distance > (extendedRange))
                return false;

            float cosAngle = Vector3.Dot(transform.forward, (Camera.current.transform.position - thisLight.transform.position).normalized);
            if((Mathf.Acos(cosAngle) * Mathf.Rad2Deg) > (thisLight.spotAngle + 3) * 0.5f)
                return false;

            return true;
        }            
    }
}

