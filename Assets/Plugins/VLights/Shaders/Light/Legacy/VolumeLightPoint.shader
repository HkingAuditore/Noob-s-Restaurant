Shader "V-Light/Point Version 2"
{
	Properties
	{
		_InvFade ("Smoothness", Range (0.0, 3.0)) = 1
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	struct v2f
	{
		float4 pos :SV_POSITION;
		float4 positionV :TEXCOORD0;
		#if _DITHER_ON || _SOFTBLEND_ON
		float4 screenPos : TEXCOORD1;
		#endif
	};

	// x = near y = far z = far - near z = fov
	float4 _LightParams;
	float4 _minBounds;
	float4 _maxBounds;
	float4x4 _ViewWorldLight;
	float4x4 _Rotation;
	float4x4 _LocalRotation;

	// User
	samplerCUBE _NoiseTex;
	samplerCUBE _LightColorEmission;

	// Auto Set
	samplerCUBE _ShadowTexture;
	sampler2D _CameraDepthTexture;

	// Attenuation values
	float _SpotExp;
	float _ConstantAttn;
	float _LinearAttn;
	float _QuadAttn;

	// Light settings
	float _Strength;
	float _Offset;
	float4 _Color;

	v2f vert (appdata_full v) {
		v2f o;
		v.vertex -= float4(0, 0, _Offset, 0);

		float4 pos = _minBounds * v.vertex + _maxBounds * (1  - v.vertex);
		pos.w = 1;

		o.pos = mul(UNITY_MATRIX_P, pos);
		o.positionV = mul(_ViewWorldLight, pos);

#if _DITHER_ON || _SOFTBLEND_ON
		o.screenPos = ComputeScreenPos(o.pos);
#endif
		return o;
	}

	#include "../VLightHelperPoint.cginc"

	fixed4 frag (v2f i) : COLOR
	{
		return computeFragPoint(i);
	}

	ENDCG

	Subshader
	{
		Tags {"RenderType"="VLightPoint" "Queue"="Transparent" "IgnoreProjector"="true"}
		LOD 500

		Pass
		{
			Fog { Mode Off }
			ZWrite off
			Blend One One

			CGPROGRAM
			
			#pragma multi_compile __ _CURVE_ON
			#pragma multi_compile __ _SHADOW_ON
			#pragma multi_compile __ _DITHER_ON
			#pragma multi_compile __ _SOFTBLEND_ON
			
			#pragma vertex vert
			#pragma fragment frag
			#if _DITHER_ON
			#pragma target 3.0
			#endif
			ENDCG
		}
	}

	Fallback Off
}
