
Shader"Food/Main" 
{
    Properties 
    {
        // _LightCol("Light Color", Color) = (1,1,1,1)
        // _Gloss("_Gloss" , Range(0,1)) = 0.5
        _Splat0 ("Layer 1(RGBA)", 2D) = "white" {}
        _BumpSplat0 ("Layer 1 Normal(Bump)", 2D) = "Bump" {}
        _Splat1 ("Layer 2(RGBA)", 2D) = "white" {}
        _BumpSplat1 ("Layer 2 Normal(Bump)", 2D) = "Bump" {}
        _Splat2 ("Layer 3(RGBA)", 2D) = "white" {}
        _BumpSplat2 ("Layer 3 Normal(Bump)", 2D) = "Bump" {}
        _Splat3 ("Layer 4(RGBA)", 2D) = "white" {}
        _BumpSplat3 ("Layer 4 Normal(Bump)", 2D) = "Bump" {}
        _Control ("Control (RGBA)", 2D) = "white" {}
        _Weight("Blend Weight" , Range(0,5)) = 0.2      
        _Tex0Power("Tex0Power" , Range(0,5)) = 0.2
        _Tex1Power("Tex1Power" , Range(0,5)) = 0.2
        _Tex2Power("Tex2Power" , Range(0,5)) = 0.2
        _Tex3Power("Tex3Power" , Range(0,5)) = 0.2
        _Tex4Power("Tex4Power" , Range(0,5)) = 0.2
        [Gamma] _Metallic ("Metallic", Range(0, 5)) = 0
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5

    }
    
    SubShader {
        Pass{
            Tags {
                 "Queue" = "Opaque"
                "SplatCount" = "4"
                "LightMode" = "ForwardBase"
                
            }

            CGPROGRAM

            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd_fullshadows

            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "Lighting.cginc" 
            #include "AutoLight.cginc"
            
            sampler2D _Control;
            sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
            sampler2D _BumpSplat0,_BumpSplat1,_BumpSplat2,_BumpSplat3;
            float4 _Control_ST;

            half _Weight;
            float _Tex0Power;
            float _Tex1Power;
            float _Tex2Power;
            float _Tex3Power;
            float _Tex4Power;
            float _Metallic;
            float _Smoothness;

            struct VertexData{
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct Interpolators{
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvSplat : TEXCOORD1;
                float3 normal : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
                SHADOW_COORDS(4)
            };


            inline half4 Blend(half depth1 ,half depth2,half depth3,half depth4 , fixed4 control) 
            {
                half4 blend ;
                
                blend.r =depth1 * control.r;
                blend.g =depth2 * control.g;
                blend.b =depth3 * control.b;
                blend.a =depth4 * control.a;
                
                half ma = max(blend.r, max(blend.g, max(blend.b, blend.a)));
                blend = max(blend - ma +_Weight , 0) * control;
                return blend/(blend.r + blend.g + blend.b + blend.a);
            }

            Interpolators vert(VertexData v){
                Interpolators i;
                i.pos = UnityObjectToClipPos(v.position);
                i.uv = TRANSFORM_TEX(v.uv, _Control);
                i.uvSplat = v.uv;
                i.normal = UnityObjectToWorldNormal(v.normal);
                i.worldPos = mul(unity_ObjectToWorld, v.position);
                TRANSFER_SHADOW(i);
                
                return i;
            }

            float4 frag(Interpolators i) : SV_TARGET {

                // float4 splat = tex2D(_MainTex, i.uvSplat);
                fixed4 splat_control = tex2D (_Control, i.uv).rgba;	
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                
                i.normal = normalize(i.normal);
                fixed4 lay1 = tex2D (_Splat0, i.uv);
                fixed4 lay2 = tex2D (_Splat1, i.uv);
                fixed4 lay3 = tex2D (_Splat2, i.uv);
                fixed4 lay4 = tex2D (_Splat3, i.uv);

                fixed3 nor1 = UnpackNormal(tex2D (_BumpSplat0, i.uv));
                fixed3 nor2 = UnpackNormal(tex2D (_BumpSplat1, i.uv));
                fixed3 nor3 = UnpackNormal(tex2D (_BumpSplat2, i.uv));
                fixed3 nor4 = UnpackNormal(tex2D (_BumpSplat3, i.uv));

                half4 blend = Blend(_Tex1Power*lay1.a,_Tex2Power*lay2.a,_Tex3Power*lay3.a,_Tex4Power* lay4.a,_Tex0Power* splat_control);

                // o.Alpha = 0.0;
                fixed4 albedo;
                albedo.rgb = lay1 *blend.r  + lay2 * blend.g + lay3 * blend.b + lay4 * blend.a;//混合
				albedo.a =0.0;
                // i.normal = nor1 * blend.r  + nor2 * blend.g  + nor3 * blend.b  + nor4 * blend.a;//法线混合
				i.normal = normalize(i.normal);


                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;

                albedo += fixed4(ambient,1);
                float3 specularTint;
                float oneMinusReflectivity;
                DiffuseAndSpecularFromMetallic(albedo, _Metallic, specularTint, oneMinusReflectivity);

                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 normalDir = i.normal;
                float3 viewReflectDir = reflect( -viewDir, normalDir);

                UnityLight light;
                light.color = _LightColor0.rgb;
                light.dir = _WorldSpaceLightPos0.xyz;
                light.ndotl = DotClamped(i.normal, light.dir);
                light.color *= atten;

                
                UnityIndirect indirectLight;
                indirectLight.diffuse = UNITY_LIGHTMODEL_AMBIENT;
                indirectLight.specular = UNITY_LIGHTMODEL_AMBIENT;
                


                // return albedo;
                return UNITY_BRDF_PBS(
                albedo, specularTint,
                oneMinusReflectivity, _Smoothness,
                i.normal, viewDir,
                light,indirectLight
                );
            }
            ENDCG 
        }
        pass
        {
            Tags { "LightMode" = "ShadowCaster" }
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"
            struct v2f
            {
                float4 pos: SV_POSITION;
            };
            
            v2f vert(appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }
            
            float4 frag(v2f o): SV_Target
            {
                SHADOW_CASTER_FRAGMENT(o)
            }
            
            ENDCG
        } 
    }
}