Shader "PostEffects/BilateralUpsample"
{
    Properties
    {
        _MainTex ("Texture", any) = "" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"}
        Cull Off ZWrite Off ZTest Always

        CGINCLUDE
        #include "UnityCG.cginc"

        UNITY_DECLARE_TEX2D(_CameraDepthTexture);
        float4 _CameraDepthTexture_TexelSize;

        UNITY_DECLARE_TEX2D(_HalfResDepthBuffer);   
        float4 _HalfResDepthBuffer_TexelSize;
        UNITY_DECLARE_TEX2D(_HalfResColor);

		UNITY_DECLARE_TEX2D(_QuarterResDepthBuffer); 
		float4 _QuarterResDepthBuffer_TexelSize;
		UNITY_DECLARE_TEX2D(_QuarterResColor);

        struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

        struct v2fDownsample
		{
#if SHADER_TARGET > 40
			float2 uv : TEXCOORD0;
#else
			float2 uv00 : TEXCOORD0;
			float2 uv01 : TEXCOORD1;
			float2 uv10 : TEXCOORD2;
			float2 uv11 : TEXCOORD3;
#endif
			float4 vertex : SV_POSITION;
		};

		struct v2fUpsample
		{
			float2 uv : TEXCOORD0;
			float2 uv00 : TEXCOORD1;
			float2 uv01 : TEXCOORD2;
			float2 uv10 : TEXCOORD3;
			float2 uv11 : TEXCOORD4;
			float4 vertex : SV_POSITION;
		};

        v2fDownsample vertDownsampleDepth(appdata v, float2 texelSize)
		{
			v2fDownsample o;
			o.vertex = UnityObjectToClipPos(v.vertex);
#if SHADER_TARGET > 40
			o.uv = v.uv;
#else
			o.uv00 = v.uv - 0.5 * texelSize.xy;
			o.uv10 = o.uv00 + float2(texelSize.x, 0);
			o.uv01 = o.uv00 + float2(0, texelSize.y);
			o.uv11 = o.uv00 + texelSize.xy;
#endif
			return o;
		}

        v2fUpsample vertUpsample(appdata v, float2 texelSize)
        {
            v2fUpsample o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = v.uv;

            o.uv00 = v.uv - 0.5 * texelSize.xy;
            o.uv10 = o.uv00 + float2(texelSize.x, 0);
            o.uv01 = o.uv00 + float2(0, texelSize.y);
            o.uv11 = o.uv00 + texelSize.xy;
            return o;
        }

        float4 BilateralUpsample(v2fUpsample input, Texture2D hiDepth, Texture2D loDepth, Texture2D loColor, SamplerState linearSampler, SamplerState pointSampler)
		{
            const float threshold = 1.5f;
            float4 highResDepth = LinearEyeDepth(hiDepth.Sample(pointSampler, input.uv)).xxxx;
			float4 lowResDepth;

            lowResDepth[0] = LinearEyeDepth(loDepth.Sample(pointSampler, input.uv00));
            lowResDepth[1] = LinearEyeDepth(loDepth.Sample(pointSampler, input.uv10));
            lowResDepth[2] = LinearEyeDepth(loDepth.Sample(pointSampler, input.uv01));
            lowResDepth[3] = LinearEyeDepth(loDepth.Sample(pointSampler, input.uv11));

			float4 depthDiff = abs(lowResDepth - highResDepth);

			float accumDiff = dot(depthDiff, float4(1, 1, 1, 1));

			[branch]
			if (accumDiff < threshold)
			{
				return loColor.Sample(linearSampler, input.uv);
			}
            
			float minDepthDiff = depthDiff[0];
			float2 nearestUv = input.uv00;

			if (depthDiff[1] < minDepthDiff)
			{
				nearestUv = input.uv10;
				minDepthDiff = depthDiff[1];
			}

			if (depthDiff[2] < minDepthDiff)
			{
				nearestUv = input.uv01;
				minDepthDiff = depthDiff[2];
			}

			if (depthDiff[3] < minDepthDiff)
			{
				nearestUv = input.uv11;
				minDepthDiff = depthDiff[3];
			}

            return loColor.Sample(pointSampler, nearestUv);
		}

        float DownsampleDepth(v2fDownsample input, Texture2D depthTexture, SamplerState depthSampler)
		{
#if SHADER_TARGET > 40
            float4 depth = depthTexture.Gather(depthSampler, input.uv);
#else
			float4 depth;
			depth.x = depthTexture.Sample(depthSampler, input.uv00).x;
			depth.y = depthTexture.Sample(depthSampler, input.uv01).x;
			depth.z = depthTexture.Sample(depthSampler, input.uv10).x;
			depth.w = depthTexture.Sample(depthSampler, input.uv11).x;
#endif
			float minDepth = min(min(depth.x, depth.y), min(depth.z, depth.w));
			float maxDepth = max(max(depth.x, depth.y), max(depth.z, depth.w));

			// chessboard pattern
			int2 position = input.vertex.xy % 2;
			int index = position.x + position.y;
			return index == 1 ? minDepth : maxDepth;
		}

        v2fUpsample vertUpsampleToFull(appdata v)
        {
            return vertUpsample(v, _HalfResDepthBuffer_TexelSize);
        }
        float4 frag(v2fUpsample input) : SV_Target
        {
            return BilateralUpsample(input, _CameraDepthTexture, _HalfResDepthBuffer, _HalfResColor, sampler_HalfResColor, sampler_HalfResDepthBuffer);
        }

		v2fUpsample vertUpsampleToFull2(appdata v)
		{
			return vertUpsample(v, _QuarterResDepthBuffer_TexelSize);
		}
		float4 frag2(v2fUpsample input) : SV_Target
		{
			return BilateralUpsample(input, _CameraDepthTexture, _QuarterResDepthBuffer, _QuarterResColor, sampler_QuarterResColor, sampler_QuarterResDepthBuffer);
		}
		v2fDownsample vertHalfDepth(appdata v)
		{
			return vertDownsampleDepth(v, _CameraDepthTexture_TexelSize);
		}

		float frag3(v2fDownsample input) : SV_Target
		{
			return DownsampleDepth(input, _CameraDepthTexture, sampler_CameraDepthTexture);
		} 
		v2fDownsample vertQuarterDepth(appdata v)
		{
			return vertDownsampleDepth(v, _HalfResDepthBuffer_TexelSize);
		}

		float frag4(v2fDownsample input) : SV_Target
		{
			return DownsampleDepth(input, _HalfResDepthBuffer, sampler_HalfResDepthBuffer);
		}
		ENDCG
		//0 for >> 1
        Pass
		{
			Blend One Zero

			CGPROGRAM
			#pragma vertex vertUpsampleToFull
			#pragma fragment frag		
            #pragma target 4.5
			ENDCG
		}

		//1 for >> 2
		pass
		{
			Blend One Zero

			CGPROGRAM
			#pragma vertex vertUpsampleToFull2
			#pragma fragment frag2		
            #pragma target 4.5
			ENDCG
		}

		//2 depth >>1
		Pass
		{
			CGPROGRAM
			#pragma vertex vertHalfDepth
			#pragma fragment frag3		
			ENDCG
		}

		//3 depth >> 2
		Pass
		{
			CGPROGRAM
            #pragma vertex vertQuarterDepth
            #pragma fragment frag4
			ENDCG
		}
    }
}
