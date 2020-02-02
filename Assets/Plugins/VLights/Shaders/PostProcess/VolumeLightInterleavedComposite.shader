Shader "Hidden/V-Light/Post" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
		_MainTexA ("Sample A", 2D) = "" {}
		_MainTexB ("Sample B", 2D) = "" {}
		_MainTexC ("Sample C", 2D) = "" {}
		_MainTexD ("Sample D", 2D) = "" {}
		_MainTexHighRes ("High Res Edge", 2D) = "" {}
	}

	CGINCLUDE

	#include "UnityCG.cginc" 
	#define DEBUG_INTERLEAVED 0

	struct v2fSimple
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2fInterleaved
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float4 screenPos : TEXCOORD1;
	};

	struct v2fBlur
	{
		float4 pos : SV_POSITION;
		float4 screenPos : TEXCOORD0;
		float2 uv[6] : TEXCOORD1;
	};

	struct v2fBlurBilateral
	{
		float4 pos : SV_POSITION;
		float2 uv[4] : TEXCOORD0;
		float2 uv2[3] : TEXCOORD4;
	};

	struct v2fUpsample
	{
		float4 pos : SV_POSITION;
		float2 uvMain : TEXCOORD0;
		float2 uv[5] : TEXCOORD1;
	};

	struct v2fComposite
	{
		float4 pos : SV_POSITION;
		float2 uv[2] : TEXCOORD0;
	};

	sampler2D _MainTex;
	sampler2D _MainTexBlurred;
	sampler2D _MainTexBlurredPoint;

	sampler2D _MainTexA;
	sampler2D _MainTexB;
	sampler2D _MainTexC;
	sampler2D _MainTexD;
	//
	float4 samplingOffset;
	float _BlurSize;
	//
	float4 _MainTexBlurred_TexelSize;
	float4 _MainTex_TexelSize;

	v2fSimple vertSimple( appdata_img v )
	{
		v2fSimple o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	}

	v2fInterleaved vertInterleaved( appdata_img v )
	{
		v2fInterleaved o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		o.screenPos = ComputeScreenPos(v.vertex);

		return o;
	}

	float2 BlurDir;
	float BlurDepthFalloff;

	sampler2D _CameraDepthTexture;
	float4 _CameraDepthTexture_TexelSize;
	sampler2D _LastCameraDepthTexture;

	sampler2D LowResDepthTexture;
	float DepthThreshold;

	v2fUpsample vertUpsample( appdata_img v ) 
	{
		v2fUpsample o;

		o.pos = UnityObjectToClipPos(v.vertex);

		const float2 lowResTexelSize = 2 * _CameraDepthTexture_TexelSize.xy;

		float2 origUV = v.texcoord;

#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			origUV.y = 1 - origUV.y;
#endif
		o.uvMain = v.texcoord;
		o.uv[0] = origUV;
		o.uv[1] = o.uv[0] - 0.5 * lowResTexelSize;
		o.uv[2] = float2(o.uv[1].x + lowResTexelSize.x, o.uv[1].y);
		o.uv[3] = float2(o.uv[1].x, o.uv[1].y + lowResTexelSize.y);
		o.uv[4] = o.uv[1] + lowResTexelSize;

		return o;
	}

	v2fBlurBilateral vertBlurBilateral( appdata_img v ) 
	{
		v2fBlurBilateral o;

		o.pos = UnityObjectToClipPos(v.vertex);

		o.uv[3] = v.texcoord.xy;
		o.uv[0] = v.texcoord.xy + BlurDir * 1 * _MainTex_TexelSize.xy;
		o.uv[1] = v.texcoord.xy + BlurDir * 2 * _MainTex_TexelSize.xy;
		o.uv[2] = v.texcoord.xy + BlurDir * 3 * _MainTex_TexelSize.xy;

		o.uv2[0] = v.texcoord.xy - BlurDir * 1 * _MainTex_TexelSize.xy;
		o.uv2[1] = v.texcoord.xy - BlurDir * 2 * _MainTex_TexelSize.xy;
		o.uv2[2] = v.texcoord.xy - BlurDir * 3 * _MainTex_TexelSize.xy;

		return o;
	}

	v2fBlur vertBlurHorz( appdata_img v ) {
		v2fBlur o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.screenPos = 0;

 		float3 off = float3(_MainTex_TexelSize.x, -_MainTex_TexelSize.x, 0) * _BlurSize;

		o.uv[0] = v.texcoord.xy + off.xz;
		o.uv[1] = v.texcoord.xy + off.yz;
		o.uv[2] = v.texcoord.xy + off.xz * 2;
		o.uv[3] = v.texcoord.xy + off.yz * 2;
		o.uv[4] = v.texcoord.xy + off.xz * 3;
		o.uv[5] = v.texcoord.xy + off.yz * 3;
		return o;
	}

	v2fBlur vertBlurVert( appdata_img v ) {
		v2fBlur o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.screenPos = 0;

 		float3 off = float3(_MainTex_TexelSize.y, -_MainTex_TexelSize.y, 0) * _BlurSize;

		o.uv[0] = v.texcoord.xy + off.zx;
		o.uv[1] = v.texcoord.xy + off.zy;
		o.uv[2] = v.texcoord.xy + off.zx * 2;
		o.uv[3] = v.texcoord.xy + off.zy * 2;
		o.uv[4] = v.texcoord.xy + off.zx * 3;
		o.uv[5] = v.texcoord.xy + off.zy * 3;
		return o;
	} 


	v2fComposite vert( appdata_img v ) {
		v2fComposite o;
		o.pos = UnityObjectToClipPos(v.vertex);

		o.uv[0] = v.texcoord.xy;
		o.uv[1] = v.texcoord.xy;
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			o.uv[0].y = 1-o.uv[0].y;
		#endif
		return o;
	}

	fixed4 frag(v2fInterleaved i) : COLOR
	{
		int2 screenPos = i.screenPos.xy  * _ScreenParams.xy;

		// Based on the screen (x,y), determine whether the pixel is even or odd
		float2 vEvenOdd = (float2) floor(fmod(screenPos.xy + 0.5, 2));
		float index = abs(3 * (float)vEvenOdd.x - 2 * (float)vEvenOdd.y);

		half4 color = 0;

#if DEBUG_INTERLEAVED
		color += tex2D (_MainTexA, i.uv * 2 - float2(1.0, 1.0));
		color += tex2D (_MainTexB, i.uv * 2 - float2(0.0, 1.0));
		color += tex2D (_MainTexC, i.uv * 2 - float2(1.0, 0.0));
		color += tex2D (_MainTexD, i.uv * 2 - float2(0.0, 0.0));
#else
		color += tex2D (_MainTexA, i.uv) * (index == 0);
		color += tex2D (_MainTexB, i.uv) * (index == 1);
		color += tex2D (_MainTexC, i.uv) * (index == 2);
		color += tex2D (_MainTexD, i.uv) * (index == 3);
#endif
		return color;
	}

	fixed4 fragBlur(v2fBlur i) : COLOR
	{
		half4 color = tex2D(_MainTex, i.uv[0]);
		color += tex2D(_MainTex, i.uv[1]);
		color += tex2D(_MainTex, i.uv[2]);
		color += tex2D(_MainTex, i.uv[3]);
		return (color / 4);
	}


	void UpdateNearestSample(inout float mindist, inout float2 nearestUV, float z, float2 uv, float zfull)
	{
		float dist = abs(z - zfull);
		if (dist < mindist)
		{
			mindist = dist;
			nearestUV = uv;
		}
	}

	float4 fragBlurBilateral(v2fBlurBilateral input) : COLOR 
	{		
		const float weight[3] = { 0.213, 0.1, 0.036 };
				 
		float centralDepth = SAMPLE_DEPTH_TEXTURE(LowResDepthTexture, input.uv[3]);
		float4 result = tex2D(_MainTex, input.uv[3]) * 0.266;		
		float totalWeight = 0.266;

		// #if !SHADER_API_METAL && !SHADER_API_GLES && !SHADER_API_OPENGL
		[unroll]
		// #endif
		for (int i = 0; i < 3; i++) 
		{
			float depth = SAMPLE_DEPTH_TEXTURE(LowResDepthTexture, input.uv[i]);	
			
			float w = abs(depth - centralDepth) * BlurDepthFalloff;	
			w = exp(-w * w);
		
			result += tex2D(_MainTex, input.uv[i]) * w * weight[i];
			
			totalWeight += w * weight[i];
			
			depth = SAMPLE_DEPTH_TEXTURE(LowResDepthTexture, input.uv2[i]);

			w = abs(depth - centralDepth) * BlurDepthFalloff;	
			w = exp(-w * w);

			result += tex2D(_MainTex, input.uv2[i]) * w * weight[i];

			totalWeight += w * weight[i];
		}
			
		return result / totalWeight;
	}

	float4 GetNearestDepthSample(float2 uv[5])
	{
		//read full resolution depth
		const float2 lowResTexelSize = 2.0 * _CameraDepthTexture_TexelSize.xy;
		float zfull = (SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv[0]));

		//find low res depth texture texel size
		const float depthTreshold = DepthThreshold;
		
		float2 lowResUV = uv[0]; 
		
		float mindist = 1.e8f;
		
		float2 uv00 = uv[1];

		float2 nearestUV = uv00;
		float z00 = (SAMPLE_DEPTH_TEXTURE(LowResDepthTexture, uv00)); 
		UpdateNearestSample(mindist, nearestUV, z00, uv00, zfull);
		
		float2 uv10 = uv[2];
		float z10 = (SAMPLE_DEPTH_TEXTURE(LowResDepthTexture, uv10));  
		UpdateNearestSample(mindist, nearestUV, z10, uv10, zfull);
		
		float2 uv01 = uv[3];
		float z01 = (SAMPLE_DEPTH_TEXTURE(LowResDepthTexture, uv01));  
		UpdateNearestSample(mindist, nearestUV, z01, uv01, zfull);
		
		float2 uv11 = uv[4];
		float z11 = (SAMPLE_DEPTH_TEXTURE(LowResDepthTexture, uv11));  
		UpdateNearestSample(mindist, nearestUV, z11, uv11, zfull);
		
		float4 sample = float4(0, 0, 0, 0);

		if (abs(z00 - zfull) < depthTreshold &&
		    abs(z10 - zfull) < depthTreshold &&
		    abs(z01 - zfull) < depthTreshold &&
		    abs(z11 - zfull) < depthTreshold )
		{
			sample = tex2D(_MainTexBlurred, lowResUV); 
		}
		else
		{
			sample = tex2D(_MainTexBlurredPoint, nearestUV);
		}
		
		return sample;
	}
	
	fixed4 fragCompositeBilateral(v2fUpsample i) : COLOR 
	{			
		fixed4 colourSample = tex2D(_MainTex, i.uvMain);
		fixed4 sample = GetNearestDepthSample(i.uv);
		return colourSample + sample;
	}

	fixed4 fragComposite(v2fUpsample i) : COLOR 
	{			
		fixed4 colourSample = tex2D(_MainTex, i.uvMain);
		fixed4 sample = tex2D(_MainTexBlurred, i.uv[0]);
		return colourSample + sample;
	}

	fixed4 fragCopyDepth(v2fSimple i) : COLOR 
	{		
		return tex2D(_LastCameraDepthTexture, i.uv);
	}

	ENDCG

Subshader
{
	//0
	Pass
	{
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

		CGPROGRAM
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma vertex vertInterleaved
		#pragma fragment frag
		ENDCG
	}

	//1
	Pass
	{
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

		CGPROGRAM
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma vertex vertBlurHorz
		#pragma fragment fragBlur
		ENDCG
	}

	//2
	Pass
	{
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

		CGPROGRAM
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma vertex vertBlurVert
		#pragma fragment fragBlur
		ENDCG
	}

	//3
	Pass 
	{
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }
		CGPROGRAM
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma vertex vertUpsample
		#pragma fragment fragCompositeBilateral
		ENDCG
	}

	//4
	Pass 
	{
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }
		CGPROGRAM
		#if SHADER_API_D3D9 || SHADER_API_D3D11
		#pragma target 3.0
		#endif
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma vertex vertBlurBilateral
		#pragma fragment fragBlurBilateral
		ENDCG
	}

	//5
	Pass 
	{
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }
		CGPROGRAM

		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma vertex vertUpsample
		#pragma fragment fragComposite
		ENDCG
	}

	//6
	Pass 
	{
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }
		CGPROGRAM
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma vertex vertSimple
		#pragma fragment fragCopyDepth
		ENDCG
	}
}

Fallback off

} // shader