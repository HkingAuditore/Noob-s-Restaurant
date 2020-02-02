#if _CURVE_ON
sampler2D _FallOffTex;
#endif

#if _DITHER_ON
float _JitterAmount;
sampler2D _DitherTex;
#endif

float3 _WorldPos;

float _InvFade;

inline fixed4 computeFragSpot (v2f i) 
{
	float _LightNearRange = _LightParams.x;
	float _LightFarRange = _LightParams.y;
	float _NearNormalised = _LightNearRange / _LightFarRange;
	float _Aspect = _LightParams.z;
	float _Fov = _LightParams.w;	

#if _SHADOW_ON
	float distShadow = -i.positionV.z / _LightFarRange;
	float shadowMapDepth = SAMPLE_DEPTH_TEXTURE_PROJ(_ShadowTexture, UNITY_PROJ_COORD(i.tcProj));
	#if _SHADOW_EXP
	float y = _LightFarRange / _LightNearRange;
	float x = (1.0 - y);
	shadowMapDepth = 1.0 / (x * shadowMapDepth + y);
	#endif
	clip(shadowMapDepth - distShadow);
	#else
#endif

#if _DITHER_ON
	float2 sp = 0.5 * _ScreenParams.xy * (i.screenPos.xy / i.screenPos.w);
  	float jitter = tex2D(_DitherTex, float2(sp.x, sp.y)).r - 0.5;
  	jitter *= _JitterAmount;

  	i.positionV.z += jitter;
  	i.tcProj.w += jitter;
  	i.tcProjScroll.w += jitter;
#endif
	
	half noise = tex2D(_NoiseTex, i.tcProjScroll.xy / i.tcProjScroll.w + (_WorldPos.xy + _WorldPos.zz)).r;
	const float3 lightDir = float3(0.0f, 0.0f, -1.0);
	
	i.positionV.x /= _Aspect;
	
	float spotEffect = dot(lightDir, normalize(i.positionV.xyz));
	float attenuation = 0.0f;
	float dist = (-i.positionV.z - _LightNearRange) / (_LightFarRange - _LightNearRange);

	float clipArea = acos(spotEffect);

	clip(_Fov - clipArea);
	clip(min(dist, 1 - dist));
	
	spotEffect = pow(spotEffect, _SpotExp);
	
#if _CURVE_ON
	float4 fallOff = tex2D(_FallOffTex, float2(dist, 0.5));	
	attenuation = fallOff.a;
#else
	attenuation = spotEffect / (_ConstantAttn + _LinearAttn * dist + _QuadAttn * (dist * dist));
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
