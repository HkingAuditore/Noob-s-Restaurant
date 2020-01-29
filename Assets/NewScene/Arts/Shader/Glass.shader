// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/Glass"
{
		Properties{
			_FrostTex ("Fross Texture", 2D) = "white" {}
			_blurSize("BlurSize", Range(0,10)) = 2
			_FrostedSize("FrostedSize", Range(0,10)) = 2
			_Cube("Reflection Map", Cube) = "" {}
			_Gloss("Gloss", Range(0,1)) = 0.5
			_ReflectSize("ReflectSize", Range(0,1)) = 0.5

            //_BumpMap("Bump Map", 2D) = "black"{}  
            //_BumpScale ("Bump Scale", Range(0.1, 1.0)) = 0.5 
		}
			SubShader{
			// Draw ourselves after all opaque geometry
			Tags{ "Queue" = "Transparent" }
			// Grab the screen behind the object into _GrabTexture
			GrabPass{}
			// Render the object with the texture generated above
			Pass{

			CGPROGRAM
                #pragma debug
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 3.0
                #include "UnityCG.cginc"
                sampler2D _GrabTexture : register(s0);
                sampler2D _FrostTex;
                float4 _FrostTex_ST;
                float _blurSize;
                float _FrostedSize;
                float _Gloss;
                float _ReflectSize;
                
                samplerCUBE _Cube;
                
                //sampler2D _BumpMap;  
                //float _BumpScale;  
                
                struct data {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normal : NORMAL;
                    
                    //float4 tangent : TANGENT;
                };
        
                struct v2f {
        
                    float4 position : POSITION;
                    float4 screenPos : TEXCOORD0;
                    
                    //float2 uv : TEXCOORD1;
                    
                    float2 uvfrost : TEXCOORD1;
                    float3 reflectTex : TEXCOORD2;  
                };
        
                v2f vert(data v) {
                    v2f o;
                    o.position = UnityObjectToClipPos(v.vertex);
                    o.uvfrost = TRANSFORM_TEX(v.uv, _FrostTex);
                    o.screenPos = o.position;
                    o.reflectTex = reflect(-WorldSpaceViewDir(v.vertex), UnityObjectToWorldNormal(v.normal));
                    

                    
                    return o;
                }
        
                half4 frag(v2f i) : COLOR
                {
                 
                
                    float surfSmooth = 1-tex2D(_FrostTex, i.uvfrost) * _FrostedSize;
                    surfSmooth = clamp(0, 1, surfSmooth);
                    float2 screenPos = i.screenPos.xy / i.screenPos.w;
                    float depth = _blurSize * 0.0003 * surfSmooth;
                    screenPos.x = (screenPos.x + 1) * 0.5;
                    screenPos.y = 1 - (screenPos.y + 1) * 0.5;
                    half4 sum = half4(0.0h,0.0h,0.0h,0.0h);
                    sum += tex2D(_GrabTexture, float2(screenPos.x - 5.0 * depth, screenPos.y + 5.0 * depth)) * 0.025;
                    sum += tex2D(_GrabTexture, float2(screenPos.x + 5.0 * depth, screenPos.y - 5.0 * depth)) * 0.025;
        
                    sum += tex2D(_GrabTexture, float2(screenPos.x - 4.0 * depth, screenPos.y + 4.0 * depth)) * 0.05;
                    sum += tex2D(_GrabTexture, float2(screenPos.x + 4.0 * depth, screenPos.y - 4.0 * depth)) * 0.05;
        
                    sum += tex2D(_GrabTexture, float2(screenPos.x - 3.0 * depth, screenPos.y + 3.0 * depth)) * 0.09;
                    sum += tex2D(_GrabTexture, float2(screenPos.x + 3.0 * depth, screenPos.y - 3.0 * depth)) * 0.09;
        
                    sum += tex2D(_GrabTexture, float2(screenPos.x - 2.0 * depth, screenPos.y + 2.0 * depth)) * 0.12;
                    sum += tex2D(_GrabTexture, float2(screenPos.x + 2.0 * depth, screenPos.y - 2.0 * depth)) * 0.12;
        
                    sum += tex2D(_GrabTexture, float2(screenPos.x - 1.0 * depth, screenPos.y + 1.0 * depth)) *  0.15;
                    sum += tex2D(_GrabTexture, float2(screenPos.x + 1.0 * depth, screenPos.y - 1.0 * depth)) *  0.15;
        
        
                    sum += tex2D(_GrabTexture, screenPos - 5.0 * depth) * 0.025;
                    sum += tex2D(_GrabTexture, screenPos - 4.0 * depth) * 0.05;
                    sum += tex2D(_GrabTexture, screenPos - 3.0 * depth) * 0.09;
                    sum += tex2D(_GrabTexture, screenPos - 2.0 * depth) * 0.12;
                    sum += tex2D(_GrabTexture, screenPos - 1.0 * depth) * 0.15;
                    sum += tex2D(_GrabTexture, screenPos) * 0.16;
                    sum += tex2D(_GrabTexture, screenPos + 5.0 * depth) * 0.15;
                    sum += tex2D(_GrabTexture, screenPos + 4.0 * depth) * 0.12;
                    sum += tex2D(_GrabTexture, screenPos + 3.0 * depth) * 0.09;
                    sum += tex2D(_GrabTexture, screenPos + 2.0 * depth) * 0.05;
                    sum += tex2D(_GrabTexture, screenPos + 1.0 * depth) * 0.025;
        
                    half4 refraction;
                    
                    float step00 = smoothstep(0.75, 1.00, surfSmooth);
                    float step01 = smoothstep(0.5, 0.75, surfSmooth);
                    float step02 = smoothstep(0.05, 0.5, surfSmooth);
                    float step03 = smoothstep(0.00, 0.05, surfSmooth);
        
                    
                    refraction = lerp(sum/2, lerp( lerp( lerp(sum/2, sum/2, step02), sum/2, step01), sum/2, step00), step03);
                    
                    float4 reflection = texCUBElod(_Cube,float4(i.reflectTex,_Gloss));     
                    //return reflection;           
                    reflection = lerp(reflection, lerp( lerp( lerp(reflection, reflection, step02), reflection, step01), reflection, step00), step03);
 
                    return (_ReflectSize * reflection)  + refraction;
                    //return refraction;
                }
			ENDCG
		}
		}
			 fallback "Reflective/VertexLit"
	}
