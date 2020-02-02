using UnityEngine;
using UnityEngine.Rendering;

namespace PostEffect
{
    [RequireComponent(typeof(Light))]
    public class PointLightTest :PostEffectsBase
    {
        public Mesh sphereMesh;
        public Shader pointShader;
        private Material poinMat;

        [HideInInspector]
        public Material poinMaterial
        {
            get
            {
                poinMat = CheckShaderAndCreateMaterial(pointShader, poinMat);
                return poinMat;
            }
        }

        private Light thisLight;
        private CommandBuffer commandBuffer;

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

        private void Awake() {

            thisLight = this.GetComponent<Light>();
            thisLight.type = LightType.Point;

            commandBuffer = new CommandBuffer();
            commandBuffer.name = "Point Light CommandBuffer";
            thisLight.AddCommandBuffer(LightEvent.AfterShadowMap,commandBuffer);
        }

        private void OnEnable() {
            VolumeLightManager.PreRenderEvent += PreRenderEvent;
        }

        private void OnDisable() {
            VolumeLightManager.PreRenderEvent -= PreRenderEvent;
        }

        private void PreRenderEvent(VolumeLightManager renderer, Matrix4x4 viewProj){
            if(thisLight.enabled == false || sphereMesh == null || poinMaterial == null){
                return;
            }
            poinMaterial.SetVector("_CameraForward", Camera.current.transform.forward);

            poinMaterial.SetInt("_SampleCount", SampleCount);
            poinMaterial.SetVector("_NoiseVelocity", new Vector4(NoiseVelocity.x, NoiseVelocity.y) * NoiseScale);
            poinMaterial.SetVector("_NoiseData", new Vector4(NoiseScale, NoiseIntensity, NoiseIntensityOffset));
            poinMaterial.SetVector("_MieG", new Vector4(1 - (HenyeyGreensteinG * HenyeyGreensteinG), 1 + (HenyeyGreensteinG * HenyeyGreensteinG), 2 * HenyeyGreensteinG, 1.0f / (4.0f * Mathf.PI)));
            poinMaterial.SetVector("_VolumetricLight", new Vector4(ScatteringCoef, ExtinctionCoef, thisLight.range, 1.0f - SkyboxExtinctionCoef));
            poinMaterial.SetTexture("_CameraDepthTexture", renderer.GetVolumeLightDepthBuffer());
            poinMaterial.SetFloat("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always); 

            if (HeightFog)
            {
                poinMaterial.EnableKeyword("HEIGHT_FOG");
                poinMaterial.SetVector("_HeightFog", new Vector4(GroundLevel, HeightScale));
            }
            else
            {
                poinMaterial.DisableKeyword("HEIGHT_FOG");
            }

            SetupPointLight(renderer, viewProj);
        }

        private void SetupPointLight(VolumeLightManager renderer, Matrix4x4 viewProj)
        {
            commandBuffer.Clear();

            int pass = 1;
            if (!IsCameraInPointLightBounds())
                pass = 1;

            poinMaterial.SetPass(pass);
            
            float scale = thisLight.range * 2.0f;
            Matrix4x4 world = Matrix4x4.TRS(transform.position, thisLight.transform.rotation, new Vector3(scale, scale, scale));

            poinMaterial.SetMatrix("_WorldViewProj", viewProj * world);
            poinMaterial.SetMatrix("_WorldView", Camera.current.worldToCameraMatrix * world);

            if (Noise)
                poinMaterial.EnableKeyword("NOISE");
            else
                poinMaterial.DisableKeyword("NOISE");

            poinMaterial.SetVector("_LightPos", new Vector4(thisLight.transform.position.x, thisLight.transform.position.y, thisLight.transform.position.z, 1.0f / (thisLight.range * thisLight.range)));
            poinMaterial.SetColor("_LightColor", thisLight.color * thisLight.intensity);

            if (thisLight.cookie == null)
            {
                poinMaterial.EnableKeyword("POINT");
                poinMaterial.DisableKeyword("POINT_COOKIE");
            }
            else
            {
                Matrix4x4 view = Matrix4x4.TRS(thisLight.transform.position, thisLight.transform.rotation, Vector3.one).inverse;
                poinMaterial.SetMatrix("_MyLightMatrix0", view);

                poinMaterial.EnableKeyword("POINT_COOKIE");
                poinMaterial.DisableKeyword("POINT");
                
                poinMaterial.SetTexture("_LightTexture0", thisLight.cookie);
            }

            bool forceShadowsOff = false;
            if ((thisLight.transform.position - Camera.current.transform.position).magnitude >= QualitySettings.shadowDistance)
                forceShadowsOff = true;

            if (thisLight.shadows != LightShadows.None && forceShadowsOff == false)
            {
                poinMaterial.EnableKeyword("SHADOWS_CUBE");
                commandBuffer.SetGlobalTexture("_ShadowMapTexture", BuiltinRenderTextureType.CurrentActive);
                commandBuffer.SetRenderTarget(renderer.GetVolumeLightBuffer());
                commandBuffer.DrawMesh(sphereMesh, world, poinMaterial, 0, pass);
            }
            else
            {
                poinMaterial.DisableKeyword("SHADOWS_CUBE");
            }
        }

        private bool IsCameraInPointLightBounds()
        {
            float distanceSqr = (thisLight.transform.position - Camera.current.transform.position).sqrMagnitude;
            float extendedRange = thisLight.range + 1;
            if (distanceSqr < (extendedRange * extendedRange))
                return true;
            return false;
        }
    }

}
