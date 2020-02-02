#if _CURVE_ON
sampler2D _FallOffTex;
#endif

#if _DITHER_ON
float _JitterAmount;
sampler2D _DitherTex;
#endif

float3 _WorldPos;

float _InvFade;

inline fixed4 computeFragOrthographic (v2f i) 
{	
#if _DITHER_ON
	float2 sp = 0.5 * _ScreenParams.xy * (i.screenPos.xy / i.screenPos.w);
  	float jitter = tex2D(_DitherTex, float2(sp.x, sp.y)).r - 0.5;
  	jitter *= _JitterAmount;
  	i.positionV.xyz += jitter;
  	i.tcProj.xyz += jitter;
  	i.tcProjScroll.xyz += jitter;
#endif

	float _LightNearRange = _LightParams.x;
	float _LightFarRange = _LightParams.y;
	float _Size = _LightParams.w;
	float _Aspect = _LightParams.z;
	
	half noise = tex2Dproj(_NoiseTex, UNITY_PROJ_COORD(i.tcProjScroll + float4(_WorldPos.xy + _WorldPos.zz, 0, 0))).r;
	float dist = (-i.positionV.z - _LightNearRange) / _LightFarRange;

	clip(float2(_Size * _Aspect, _Size) - abs(i.positionV.xy));
	clip(min(dist, (1 - _LightNearRange / _LightFarRange) - dist));
	
#if _CURVE_ON
	float4 fallOff = tex2D(_FallOffTex, float2(dist, 0.5));	
	float attenuation = fallOff.a;
#else
	float attenuation = _SpotExp / (_ConstantAttn + _LinearAttn * dist + _QuadAttn * (dist * dist));
#endif
	
#if _SHADOW_ON
	float distShadow = -i.positionV.z / _LightFarRange;
	float shadowMapDepth = SAMPLE_DEPTH_TEXTURE_PROJ(_ShadowTexture, UNITY_PROJ_COORD(i.tcProj));
	clip(shadowMapDepth - distShadow);
#endif

	half4 color = tex2Dproj(_LightColorEmission, i.tcProj);	
	
#if _CURVE_ON
	half3 Albedo = fallOff.rgb;
	half Alpha = (noise * attenuation) * _Strength;
#else
	half3 Albedo = color.rgb * _Color.rgb;
	half Alpha = (noise * attenuation) * _Strength * _Color.a;
#endif

#if _SOFTBLEND_ON
	half sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
	Alpha *= saturate(_InvFade * (sceneZ - i.screenPos.w));
#endif

	return half4(Albedo * Alpha, 1);
}
