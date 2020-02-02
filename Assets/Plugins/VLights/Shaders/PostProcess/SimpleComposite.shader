Shader "Hidden/Simple Composite" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
		_MainTexBlurred ("Base Blurred (RGBA)", 2D) = "" {}
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
	sampler2D _MainTexBlurred;

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
		half4 textureBlurred = tex2D(_MainTexBlurred, i.uv[0]);
		half4 sourceTexture = tex2D(_MainTex, i.uv[1]);

		return textureBlurred + sourceTexture;
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