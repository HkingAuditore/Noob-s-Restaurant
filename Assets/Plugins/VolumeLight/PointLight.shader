Shader "PostEffects/VolumeLight/PointLight"
{
	Properties
	{
		[HideInInspector]
        _MainTex ("Texture", 2D) = "white" {}
		[HideInInspector]
        _ZTest ("ZTest", Float) = 0
		[HideInInspector]
        _LightColor("_LightColor", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		CGINCLUDE

		#if defined(SHADOWS_DEPTH) || defined(SHADOWS_CUBE)
		#define SHADOWS_NATIVE
		#endif
		
		#include "UnityCG.cginc"
		#include "UnityDeferredLibrary.cginc"

		sampler3D _NoiseTexture;
		sampler2D _DitherTexture;
		
		float4 _FrustumCorners[4];

		struct appdata
		{
			float4 vertex : POSITION;
		};
		
		float4x4 _WorldViewProj;
		float4x4 _MyLightMatrix0;
		float4x4 _MyWorld2Shadow;

		float3 _CameraForward;
		float4 _VolumetricLight;
        float4 _MieG;

		float4 _NoiseData;
		float4 _NoiseVelocity;
		float4 _HeightFog;

		float _MaxRayLength;

		int _SampleCount;

		struct v2f
		{
			float4 pos : SV_POSITION;
			float4 uv : TEXCOORD0;
			float3 wpos : TEXCOORD1;
		};

		v2f vert(appdata v)
		{
			v2f o;
			o.pos = mul(_WorldViewProj, v.vertex);
			o.uv = ComputeScreenPos(o.pos);
			o.wpos = mul(unity_ObjectToWorld, v.vertex);
			return o;
		}
		
		UNITY_DECLARE_SHADOWMAP(_CascadeShadowMapTexture);
		
		float GetLightAttenuation(float3 wpos)
		{
			float atten = 0;
			float3 tolight = wpos - _LightPos.xyz;
			half3 lightDir = -normalize(tolight);

			float att = dot(tolight, tolight) * _LightPos.w;
			atten = tex2D(_LightTextureB0, att.rr).UNITY_ATTEN_CHANNEL;

			atten *= UnityDeferredComputeShadow(tolight, 0, float2(0, 0));

#if defined (POINT_COOKIE)
			atten *= texCUBEbias(_LightTexture0, float4(mul(_MyLightMatrix0, half4(wpos, 1)).xyz, -8)).w;
#endif 
			return atten;
		}

        void ApplyHeightFog(float3 wpos, inout float density)
        {
#ifdef HEIGHT_FOG
            density *= exp(-(wpos.y + _HeightFog.x) * _HeightFog.y);
#endif
        }

		float GetDensity(float3 wpos)
		{
            float density = 1;
#ifdef NOISE
			float noise = tex3D(_NoiseTexture, frac(wpos * _NoiseData.x + float3(_Time.y * _NoiseVelocity.x, 0, _Time.y * _NoiseVelocity.y)));
			noise = saturate(noise - _NoiseData.z) * _NoiseData.y;
			density = saturate(noise);
#endif
            ApplyHeightFog(wpos, density);

            return density;
		}        

		float MieScattering(float cosAngle, float4 g)
		{
            return g.w * (g.x / (pow(g.y - g.z * cosAngle, 1.5)));			
		}

		float4 RayMarch(float2 screenPos, float3 rayStart, float3 rayDir, float rayLength)
		{
#ifdef DITHER_4_4
			float2 interleavedPos = (fmod(floor(screenPos.xy), 4.0));
			float offset = tex2D(_DitherTexture, interleavedPos / 4.0 + float2(0.5 / 4.0, 0.5 / 4.0)).w;
#else
			float2 interleavedPos = (fmod(floor(screenPos.xy), 8.0));
			float offset = tex2D(_DitherTexture, interleavedPos / 8.0 + float2(0.5 / 8.0, 0.5 / 8.0)).w;
#endif
			int stepCount = _SampleCount;

			float stepSize = rayLength / stepCount;
			float3 step = rayDir * stepSize;

			float3 currentPosition = rayStart + step * offset;

			float4 vlight = 0;

			float cosAngle;
			float extinction = length(_WorldSpaceCameraPos - currentPosition) * _VolumetricLight.y * 0.5;
			[loop]
			for (int i = 0; i < stepCount; ++i)
			{
				float atten = GetLightAttenuation(currentPosition);
				float density = GetDensity(currentPosition);

                float scattering = _VolumetricLight.x * stepSize * density;
				extinction += _VolumetricLight.y * stepSize * density;

				float4 light = atten * scattering * exp(-extinction);

                float3 tolight = normalize(currentPosition - _LightPos.xyz);
                cosAngle = dot(tolight, -rayDir);
				light *= MieScattering(cosAngle, _MieG);
     
				vlight += light;

				currentPosition += step;				
			}

			vlight *= _LightColor;

			vlight = max(0, vlight);
            vlight.w = 0;
			return vlight;
		}

		ENDCG
		Pass
		{
			ZTest Off
			Cull Front
			ZWrite Off
			Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragPointInside
			#pragma target 4.0

			#define UNITY_HDR_ON

			#pragma shader_feature HEIGHT_FOG
			#pragma shader_feature NOISE
			#pragma shader_feature SHADOWS_CUBE
			#pragma shader_feature POINT_COOKIE
			#pragma shader_feature POINT

			#ifdef SHADOWS_DEPTH
			#define SHADOWS_NATIVE
			#endif
						
			
			fixed4 fragPointInside(v2f i) : SV_Target
			{	
				float2 uv = i.uv.xy / i.uv.w;

				// read depth and reconstruct world position
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);

				float3 rayStart = _WorldSpaceCameraPos;
				float3 rayEnd = i.wpos;

				float3 rayDir = (rayEnd - rayStart);
				float rayLength = length(rayDir);
 
				rayDir /= rayLength;

				float linearDepth = LinearEyeDepth(depth);
				float projectedDepth = linearDepth / dot(_CameraForward, rayDir);
				rayLength = min(rayLength, projectedDepth);
				
				return RayMarch(i.pos.xy, rayStart, rayDir, rayLength);
			}
			ENDCG
		}
		Pass
		{
			ZTest [_ZTest]
			Cull Back
			ZWrite Off
			Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragPointOutside
			#pragma target 4.0

			#define UNITY_HDR_ON

			#pragma shader_feature HEIGHT_FOG
			#pragma shader_feature SHADOWS_CUBE
			#pragma shader_feature NOISE
			//#pragma multi_compile POINT POINT_COOKIE
			#pragma shader_feature POINT_COOKIE
			#pragma shader_feature POINT

			fixed4 fragPointOutside(v2f i) : SV_Target
			{
				float2 uv = i.uv.xy / i.uv.w;

				// read depth and reconstruct world position
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
			
				float3 rayStart = _WorldSpaceCameraPos;
				float3 rayEnd = i.wpos;

				float3 rayDir = (rayEnd - rayStart);
				float rayLength = length(rayDir);

				rayDir /= rayLength;

				float3 lightToCamera = _WorldSpaceCameraPos - _LightPos;

				float b = dot(rayDir, lightToCamera);
				float c = dot(lightToCamera, lightToCamera) - (_VolumetricLight.z * _VolumetricLight.z);

				float d = sqrt((b*b) - c);
				float start = -b - d;
				float end = -b + d;

				float linearDepth = LinearEyeDepth(depth);
				float projectedDepth = linearDepth / dot(_CameraForward, rayDir);
				end = min(end, projectedDepth);

				rayStart = rayStart + rayDir * start;
				rayLength = end - start;

				return RayMarch(i.pos.xy, rayStart, rayDir, rayLength);
			}
			ENDCG
		}
	}
}
