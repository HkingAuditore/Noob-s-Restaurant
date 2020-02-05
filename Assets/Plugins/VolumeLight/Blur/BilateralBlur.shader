Shader "PostEffects/BilateralBlur"
{
    Properties
    {
        _MainTex ("Texture", any) = "" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off ZWrite Off ZTest Always

        CGINCLUDE
        #include "UnityCG.cginc"

        #define BLUR_DEPTH_FACTOR 0.5
        #define GAUSS_BLUR_DEVIATION 1.5        
        #define FULL_KERNEL_SIZE 7
        #define HALF_KERNEL_SIZE 5
        #define QUARTER_KERNEL_SIZE 6

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
        };

        UNITY_DECLARE_TEX2D(_MainTex);
        float4 _MainTex_ST;
        UNITY_DECLARE_TEX2D(_CameraDepthTexture);
        float4 _CameraDepthTexture_TexelSize;
        

        float GaussianWeight(float offset, float deviation)
        {
            float weight = 1.0f / sqrt(2.0f * 3.1415927f * deviation * deviation);
            weight *= exp(-(offset * offset) / (2.0f * deviation * deviation));
            return weight;
        }

        float4 BilateralBlur(v2f input, int2 direction, Texture2D depth, const int kernelRadius)
        {
            const float deviation = kernelRadius / GAUSS_BLUR_DEVIATION; // make it really strong

            float2 uv = input.uv;
            float4 centerColor = _MainTex.Sample(sampler_MainTex, uv);
            float3 color = centerColor.xyz;
            float centerDepth = (LinearEyeDepth(depth.Sample(sampler_CameraDepthTexture, uv)));

            float weightSum = 0;

            float weight = GaussianWeight(0, deviation);
            color *= weight;
            weightSum += weight;
                        
            [unroll] for (int i = -kernelRadius; i < 0; i += 1)
            {
                float2 offset = (direction * i);
                float3 sampleColor = _MainTex.Sample(sampler_MainTex, input.uv, offset);
                float sampleDepth = (LinearEyeDepth(depth.Sample(sampler_CameraDepthTexture, input.uv, offset)));

                float depthDiff = abs(centerDepth - sampleDepth);
                float dFactor = depthDiff * BLUR_DEPTH_FACTOR;
                float w = exp(-(dFactor * dFactor));

                weight = GaussianWeight(i, deviation) * w;

                color += weight * sampleColor;
                weightSum += weight;
            }

            [unroll] for (i = 1; i <= kernelRadius; i += 1)
            {
                float2 offset = (direction * i);
                float3 sampleColor = _MainTex.Sample(sampler_MainTex, input.uv, offset);
                float sampleDepth = (LinearEyeDepth(depth.Sample(sampler_CameraDepthTexture, input.uv, offset)));

                float depthDiff = abs(centerDepth - sampleDepth);
                float dFactor = depthDiff * BLUR_DEPTH_FACTOR;
                float w = exp(-(dFactor * dFactor));
                
                weight = GaussianWeight(i, deviation) * w;

                color += weight * sampleColor;
                weightSum += weight;
            }

            color /= weightSum;
            return float4(color, centerColor.w);
        }

        v2f vert (appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            return o;
        }

        fixed4 fragHorizonFull (v2f i) : SV_Target
        {
            return BilateralBlur(i,int2(1,0),_CameraDepthTexture,FULL_KERNEL_SIZE);
        }

        fixed4 fragverticalFull(v2f i) : SV_TARGET
        {
            return BilateralBlur(i,int2(0,1),_CameraDepthTexture,FULL_KERNEL_SIZE);
        }
        fixed4 fragHorizonHalf (v2f i) : SV_Target
        {
            return BilateralBlur(i,int2(1,0),_CameraDepthTexture,HALF_KERNEL_SIZE);
        }
        fixed4 fragverticalHalf(v2f i) : SV_TARGET
        {
            return BilateralBlur(i,int2(0,1),_CameraDepthTexture,HALF_KERNEL_SIZE);
        }
        fixed4 fragHorizonQuarter (v2f i) : SV_Target
        {
            return BilateralBlur(i,int2(1,0),_CameraDepthTexture,QUARTER_KERNEL_SIZE);
        }
        fixed4 fragverticalQuarter(v2f i) : SV_TARGET
        {
            return BilateralBlur(i,int2(0,1),_CameraDepthTexture,QUARTER_KERNEL_SIZE);
        }
        ENDCG

        //0，横向不降采样
        pass
        {
            CGPROGRAM
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment fragHorizonFull
            ENDCG
        }
        //1，纵向不降采样
        pass
        {
            CGPROGRAM
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment fragverticalFull
            ENDCG
        }
        //2，横向降采样>>1
        pass
        {
            CGPROGRAM
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment fragHorizonHalf
            ENDCG
        }
        //3，纵向降采样>>1
        pass
        {
            CGPROGRAM
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment fragverticalHalf
            ENDCG
        }
        //4，横向降采样>>2
        pass
        {
            CGPROGRAM
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment fragHorizonQuarter
            ENDCG
        }
        //5，纵向降采样>>2
        pass
        {
            CGPROGRAM
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment fragverticalQuarter
            ENDCG
        }
    }
}
