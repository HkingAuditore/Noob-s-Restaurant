sampler3D _MainTex;
float _VolumeParams;
float4 _VolumeOffset;
float3 _NoiseOffset;

#if _DITHER_ON
float _JitterAmount;
sampler2D _DitherTex;
#endif

float _InvFade;

inline fixed4 computeFrag (v2f i) 
{		
	#if _DITHER_ON
	float2 sp = 0.5 * _ScreenParams.xy * (i.screenPos.xy / i.screenPos.w);
	float jitter = tex2D(_DitherTex, float2(sp.x, sp.y)).r - 0.5;
	jitter *= _JitterAmount;
  	i.positionV.xyz += jitter;
  	#endif	

	float _LightNearRange = _LightParams.x;
	float _LightFarRange = _LightParams.y;
	float _Aspect = _LightParams.z;
	
	i.positionV.x /= _Aspect;

	float3 direction = i.positionV.xyz;
	float3 rotatedDirection = mul(direction, (float3x3)_Rotation).xyz; 	
	
	float finalVal = 0;
	
#if _SHAPE_SPHERE
	finalVal = length(direction) - _LightFarRange;
#elif _SHAPE_CUBE		
	finalVal = length(max(abs(direction) - _LightFarRange, 0.0));
#elif _SHAPE_ROUNDED_CUBE
	finalVal = length(max(abs(direction) - _LightFarRange + _VolumeParams,0.0)) - _VolumeParams;
#elif _SHAPE_CYLINDER
	float2 h = float2(_LightFarRange * _VolumeParams, _LightFarRange);
	float2 ds = abs(float2(length(direction.xy), direction.z)) - h;
 	finalVal = min(max(ds.x, ds.y), 0.0) + length(max(ds, 0.0));
#endif

	clip(finalVal < 0.01 ? 1 : -1);
	
	fixed4 col = tex3D(_MainTex, ((_VolumeOffset.w / 2) * direction) + _VolumeOffset.xyz + _NoiseOffset.xyz) * _Strength;

	#if _SOFTBLEND_ON
	half sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
	col *= saturate(_InvFade * (sceneZ - i.screenPos.w));
	#endif

	return col * _Color;
}
