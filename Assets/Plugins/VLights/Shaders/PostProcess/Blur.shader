Shader "Hidden/MultiTap Blur" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
		_BlurSize ("Blur Size", FLOAT) = 0.01
	}

	// Shader code pasted into all further CGPROGRAM blocks
	CGINCLUDE

	#include "UnityCG.cginc"

	struct v2f {
		float4 pos : POSITION;
		float2 uv[2] : TEXCOORD0;
	};

	sampler2D _MainTex;
	float4 _MainTex_TexelSize;
	float _BlurSize;

	v2f vert( appdata_img v ) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);

		o.uv[0] = v.texcoord.xy;
		o.uv[1] = v.texcoord.xy;
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			o.uv[0].y = 1-o.uv[0].y;
		#endif
		return o;
	}

	half4 frag(v2f i) : COLOR
	{
		#define SAMPLE_COUNT 12
		const float3 RAND_SAMPLES[SAMPLE_COUNT] = {
			float3(0.4010039,0.8899381,-0.01751772),
			float3(0.1617837,0.1338552,-0.3530486),
			float3(-0.2305296,-0.1900085,0.5025396),
			float3(-0.6256684,0.1241661,0.1163932),
			float3(0.3820786,-0.3241398,0.4112825),
			float3(-0.08829653,0.1649759,0.1395879),
			float3(0.1891677,-0.1283755,-0.09873557),
			float3(0.1986142,0.1767239,0.4380491),
			float3(-0.3294966,0.02684341,-0.4021836),
			float3(-0.01956503,-0.3108062,-0.410663),
			float3(-0.3215499,0.6832048,-0.3433446),
			float3(0.7026125,0.1648249,0.02250625)
		};

		half4 color = 0;
		for (int s = 0; s < SAMPLE_COUNT; ++s)
		{
			color += tex2D(_MainTex, i.uv[1] + RAND_SAMPLES[s].xy * _BlurSize);
		}

		return (color / SAMPLE_COUNT);
	}

	ENDCG

Subshader {
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment frag
      ENDCG
  }
}

Fallback off

} // shader