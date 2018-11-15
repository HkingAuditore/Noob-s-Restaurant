Shader " DoubleSideShader" {
	Properties{
		//正面5个参数
			 _Color("Main Color", Color) = (1,1,1,1)
			  _SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
			  _Shininess("Shininess", Range(0.03, 1)) = 0.078125
			  _MainTex("Base (RGB) Gloss (A)", 2D) = "white" {}
			 _BumpMap("Normalmap", 2D) = "bump" {}
			 //反面拷贝 改名 也是5个
				 _BackColor("Back Main Color", Color) = (1,1,1,1)
				 _BackSpecColor("Back Specular Color", Color) = (0.5, 0.5, 0.5, 1)
				 _BackShininess("Back Shininess", Range(0.03, 1)) = 0.078125
				 _BackMainTex("Back Base (RGB) Gloss (A)", 2D) = "white" {}
				 _BackBumpMap("Back Normalmap", 2D) = "bump" {}
	}
		SubShader{
		Tags { "RenderType" = "Opaque" }
			LOD 400
			Cull back
		//开始渲染正面    
		CGPROGRAM
		//表明是surface渲染方式 主渲染程序是surf 光照模型是BLinnPhong
		#pragma surface surf BlinnPhong  


	   sampler2D _MainTex;
	   sampler2D _BumpMap;
		fixed4 _Color;
		half _Shininess;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
		   o.Albedo = tex.rgb * _Color.rgb;
			o.Gloss = tex.a;
			o.Alpha = tex.a * _Color.a;
			o.Specular = _Shininess;
		   o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
}
ENDCG


	Cull front
	//开始渲染反面 其实和就是拷贝了一份正面渲染的代码 除了变量名要改    
	CGPROGRAM
 #pragma surface surf BlinnPhong

	sampler2D _BackMainTex;
	sampler2D _BackBumpMap;
	fixed4 _BackColor;
	half _BackShininess;

	struct Input {
		float2 uv_BackMainTex;
		float2 uv_BackBumpMap;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		 fixed4 tex = tex2D(_BackMainTex, IN.uv_BackMainTex);
		 o.Albedo = tex.rgb * _BackColor.rgb;
	   o.Gloss = tex.a;
		o.Alpha = tex.a * _BackColor.a;
		 o.Specular = _BackShininess;
		 o.Normal = UnpackNormal(tex2D(_BackBumpMap, IN.uv_BackBumpMap));
}
ENDCG
	}
		FallBack "Specular"
}
