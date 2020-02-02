Shader "Hidden/V-Light/Downscale Depth" 
{
	CGINCLUDE
	#include "UnityCG.cginc"
	
	struct v2f 
	{
		float4 pos : SV_POSITION;
		float2 uv[4] : TEXCOORD0;
	};

	sampler2D _CameraDepthTexture;
	float4 _CameraDepthTexture_TexelSize;

	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);

		const float2 texelSize = 0.5f * _CameraDepthTexture_TexelSize.xy;

		o.uv[0] = float2(v.texcoord + float2(-1,-1) * texelSize);
		o.uv[1] = float2(v.texcoord + float2(-1, 1) * texelSize);
		o.uv[2] = float2(v.texcoord + float2( 1,-1) * texelSize);
		o.uv[3] = float2(v.texcoord + float2( 1, 1) * texelSize);
		
		return o;
	}

	float4 frag(v2f i) : SV_Target 
	{		
		float depth1 = (SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv[0]));
		float depth2 = (SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv[1]));
		float depth3 = (SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv[2]));
		float depth4 = (SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv[3]));

		float result = min(depth1, min(depth2, min(depth3, depth4)));
		return float4(result, 0, 0, 0);
	}
	
	ENDCG

	SubShader 
	{
		 Pass 
		 {
			  ZTest Always Cull Off ZWrite Off
			  CGPROGRAM
			  #pragma vertex vert
			  #pragma fragment frag
			  ENDCG
		  }
	}
	Fallback off
}
