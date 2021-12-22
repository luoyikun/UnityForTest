Shader "Unlit/RotationUV"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_RotationSpeed("Rotation Speed", Float) = 2.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			uniform float _RotationSpeed;
			float4 _MainTex_ST;

			float4 CalculateRotation(float4 pos)
			{
				//先计算距离位置点距离原点的值
				float2 div= (_MainTex_ST.xy / 2.0) + _MainTex_ST.zw;
				float rot = _RotationSpeed * _Time.y;

				pos.xy -= div;

				float s, c;
				sincos(radians(rot), s, c);
				float2x2 rotMatrix = float2x2(c, -s, s, c);
				pos.xy = mul(pos.xy, rotMatrix);

				pos.xy += div;
				return pos;
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = CalculateRotation(v.uv);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
