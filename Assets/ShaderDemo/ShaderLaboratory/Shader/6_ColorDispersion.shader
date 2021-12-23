Shader "MyShaderTest/6_ColorDispersion"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			float _Scale;
			float _Speed;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed3 col = fixed3(0,0,0);

				fixed scale = _Scale * sin(_Time.y * _Speed); 

				scale = saturate(scale);

				half2 uv_0 = half2(i.uv.x - scale,i.uv.y);
				half2 uv_1 = half2(i.uv.x + scale, i.uv.y);

				col.r = tex2D(_MainTex, uv_0).r;
				col.g = tex2D(_MainTex, i.uv).g;
				col.b = tex2D(_MainTex, uv_1).b;

				return fixed4(col,1);
			}
			ENDCG
		}
	}
}
