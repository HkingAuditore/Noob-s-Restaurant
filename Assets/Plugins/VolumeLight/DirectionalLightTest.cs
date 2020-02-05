using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PostEffect
{
    [RequireComponent(typeof(Light))]
    public class DirectionalLightTest : PostEffectsBase
    {
        public Shader directionShader;
        private Material direMat;

        [HideInInspector]
        public Material direMaterial
        {
            get
            {
                direMat = CheckShaderAndCreateMaterial(directionShader, direMat);
                return direMat;
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
        private int MaxRayLength = 400;
        private Vector4[] _frustumCorners = new Vector4[4];

        private void Awake() {
            thisLight = this.GetComponent<Light>();
            thisLight.type = LightType.Directional;

            commandBuffer = new CommandBuffer();
            commandBuffer.name = "Directional Light CommandBuffer";
            shadowCommandBuffer = new CommandBuffer();
            shadowCommandBuffer.name = "Direction Light CommandBuffer2";
            shadowCommandBuffer.SetGlobalTexture("_CascadeShadowMapTexture", new UnityEngine.Rendering.RenderTargetIdentifier(UnityEngine.Rendering.BuiltinRenderTextureType.CurrentActive));

            thisLight.AddCommandBuffer(LightEvent.BeforeScreenspaceMask,commandBuffer);
            thisLight.AddCommandBuffer(LightEvent.AfterShadowMap,shadowCommandBuffer);
        }
        
        private void OnEnable() {
            VolumeLightManager.PreRenderEvent += PreRenderEvent;
        }

        private void OnDisable() {
            VolumeLightManager.PreRenderEvent -= PreRenderEvent;
        }

        private void PreRenderEvent(VolumeLightManager renderer, Matrix4x4 viewProj){
            if(thisLight.enabled == false || direMaterial == null){
                return;
            }
            direMaterial.SetVector("_CameraForward", Camera.current.transform.forward);

            direMaterial.SetInt("_SampleCount", SampleCount);
            direMaterial.SetVector("_NoiseVelocity", new Vector4(NoiseVelocity.x, NoiseVelocity.y) * NoiseScale);
            direMaterial.SetVector("_NoiseData", new Vector4(NoiseScale, NoiseIntensity, NoiseIntensityOffset));
            direMaterial.SetVector("_MieG", new Vector4(1 - (HenyeyGreensteinG * HenyeyGreensteinG), 1 + (HenyeyGreensteinG * HenyeyGreensteinG), 2 * HenyeyGreensteinG, 1.0f / (4.0f * Mathf.PI)));
            direMaterial.SetVector("_VolumetricLight", new Vector4(ScatteringCoef, ExtinctionCoef, thisLight.range, 1.0f - SkyboxExtinctionCoef));
            direMaterial.SetTexture("_CameraDepthTexture", renderer.GetVolumeLightDepthBuffer());
            direMaterial.SetFloat("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always); 

            if (HeightFog)
            {
                direMaterial.EnableKeyword("HEIGHT_FOG");
                direMaterial.SetVector("_HeightFog", new Vector4(GroundLevel, HeightScale));
            }
            else
            {
                direMaterial.DisableKeyword("HEIGHT_FOG");
            }

            SetupDirectionalLight(renderer, viewProj);
        }

        private void SetupDirectionalLight(VolumeLightManager renderer, Matrix4x4 viewProj)
        {
            commandBuffer.Clear();

            int pass = 0;

            direMaterial.SetPass(pass);
            
            if (Noise)
                direMaterial.EnableKeyword("NOISE");
            else
                direMaterial.DisableKeyword("NOISE");

            direMaterial.SetVector("_LightDir", new Vector4(thisLight.transform.forward.x, thisLight.transform.forward.y, thisLight.transform.forward.z, 1.0f / (thisLight.range * thisLight.range)));
            direMaterial.SetVector("_LightColor", thisLight.color * thisLight.intensity);
            direMaterial.SetFloat("_MaxRayLength", MaxRayLength);

            if (thisLight.cookie == null)
            {
                direMaterial.EnableKeyword("DIRECTIONAL");
                direMaterial.DisableKeyword("DIRECTIONAL_COOKIE");
            }
            else
            {
                direMaterial.EnableKeyword("DIRECTIONAL_COOKIE");
                direMaterial.DisableKeyword("DIRECTIONAL");

                direMaterial.SetTexture("_LightTexture0", thisLight.cookie);
            }
            _frustumCorners[0] = Camera.current.ViewportToWorldPoint(new Vector3(0, 0, Camera.current.farClipPlane));
            _frustumCorners[2] = Camera.current.ViewportToWorldPoint(new Vector3(0, 1, Camera.current.farClipPlane));
            _frustumCorners[3] = Camera.current.ViewportToWorldPoint(new Vector3(1, 1, Camera.current.farClipPlane));
            _frustumCorners[1] = Camera.current.ViewportToWorldPoint(new Vector3(1, 0, Camera.current.farClipPlane));

    #if UNITY_5_4_OR_NEWER
            direMaterial.SetVectorArray("_FrustumCorners", _frustumCorners);
    #else
            _material.SetVector("_FrustumCorners0", _frustumCorners[0]);
            _material.SetVector("_FrustumCorners1", _frustumCorners[1]);
            _material.SetVector("_FrustumCorners2", _frustumCorners[2]);
            _material.SetVector("_FrustumCorners3", _frustumCorners[3]);
    #endif

            Texture nullTexture = null;
            if (thisLight.shadows != LightShadows.None)
            {
                direMaterial.EnableKeyword("SHADOWS_DEPTH");            
                commandBuffer.Blit(nullTexture, renderer.GetVolumeLightBuffer(), direMaterial, pass);
            }
            else
            {
                direMaterial.DisableKeyword("SHADOWS_DEPTH");
                renderer.commandBuffers[0].Blit(nullTexture, renderer.GetVolumeLightBuffer(), direMaterial, pass);
            }
        }
    }
}
