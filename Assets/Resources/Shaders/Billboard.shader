Shader "Unlit/Billboard" {
	Properties{
	   _MainTex("Texture Image", 2D) = "white" {}
	   _ScaleX("Scale X", Float) = 1.0
	   _ScaleY("Scale Y", Float) = 1.0
	   _PositionX("Position X", Float) = 1.0
	   _PositionY("Position Y", Float) = 1.0
	   _Color("_Color",Color) = (1,1,1,1)
	}
		SubShader{
			Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "DisableBatching" = "True"}
			//Tags { "DisableBatching" = "True" }
		   Pass {
			 Tags { "DisableBatching" = "True" }
			 ZWrite On
			 Blend SrcAlpha OneMinusSrcAlpha
			 Cull Off

			  CGPROGRAM

			  #pragma vertex vert
			  #pragma fragment frag
			  #include "Lighting.cginc"

		   // User-specified uniforms
		   uniform sampler2D _MainTex;
		   uniform float _ScaleX;
		   uniform float _ScaleY;
		   uniform float _PositionX;
		   uniform float _PositionY;
		   uniform fixed4 _Color;
		   uniform float4 _MainTex_ST;

		   struct vertexInput {
			  float4 vertex : POSITION;
			  float4 tex : TEXCOORD0;
		   };
		   struct vertexOutput {
			  float4 pos : SV_POSITION;
			  float2 tex : TEXCOORD0;
			  //float2 uv:TEXCOORD1;
		   };

		   vertexOutput vert(vertexInput input)
		   {
			  vertexOutput output;

			  output.pos = mul(UNITY_MATRIX_P,
				mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
				+ float4(input.vertex.x + _PositionX, input.vertex.y + _PositionY, 0.0, 0.0)
				* float4(_ScaleX, _ScaleY, 1.0, 1.0));

				output.tex = TRANSFORM_TEX(input.tex,_MainTex);
				//output.uv=TRANSFORM_TEX(input.tex,_MainTex);
				 //output.tex=input.tex;
				 return output;
			  }

			  float4 frag(vertexOutput input) : SV_Target
			  {
				   fixed4 c = tex2D(_MainTex, float2(input.tex.xy));
				   c.rgb *= _Color.rgb;
				   return c;
			   }
		   ENDCG
		}
	   }
}