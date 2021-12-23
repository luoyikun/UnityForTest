Shader "MyShaderTest/3_PixelStyle"
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_PixelAmount ("Pixel Amount", float) = 50
	}

	SubShader
	{
		ZTest Always Cull Off ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			half4 _MainTex_TexelSize;
			float _PixelAmount;

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 uv : TEXCOORD0;
			};


			v2f vert(appdata_img v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;

				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0.0)
					o.uv.y = 1.0 - o.uv.y;
				#endif

				return o;
			}


			fixed4 frag(v2f i) : SV_Target
			{
				fixed uv_X = floor(i.uv.x * _PixelAmount) / _PixelAmount;
				fixed uv_Y = floor(i.uv.y * _PixelAmount) / _PixelAmount;

				i.uv = half2(uv_X,uv_Y);

				fixed4 color = tex2D(_MainTex,i.uv);

				return fixed4(color.rgb,1);
			}

			ENDCG
		}		
	}
	FallBack Off
}

