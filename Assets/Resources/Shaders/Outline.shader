Shader "Custom/Outline"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" { }
		_OutlineColor("Outline Color", Color) = (1, 1, 0, 1)
		_OutlineWidth("Outline width", Range(0.0, 1.0)) = .07
	}

		SubShader
		{
			Pass
			{
				Cull front

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
				};

				struct v2f
				{
					float4 pos : POSITION;
				};

				uniform float _OutlineWidth;
				uniform float4 _OutlineColor;

				v2f vert(appdata v)
				{
					v2f o;

					float3 norm = normalize(v.normal);
					v.vertex.xyz += v.normal * _OutlineWidth;
					o.pos = UnityObjectToClipPos(v.vertex);

					return o;
				}

				half4 frag(v2f i) : COLOR
				{
					return _OutlineColor;
				}
				ENDCG
			}

			Pass
			{
				SetTexture[_MainTex]
				{
					Combine Primary * Texture
				}
			}

			Pass
			{
					Tags{"LightMode" = "ShadowCaster"}
					CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					#pragma multi_compile_shadowcaster
					#include "UnityCG.cginc"
					struct v2f {
					V2F_SHADOW_CASTER;

				};

				v2f vert(appdata_base v)
				{
					v2f o;
					TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
						return o;
				}

				float4 frag(v2f i):SV_Target
				{
					SHADOW_CASTER_FRAGMENT(i)
				}
					ENDCG
			}
		}
}