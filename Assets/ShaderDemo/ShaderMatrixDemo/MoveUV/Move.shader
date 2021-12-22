Shader "Unlit/MoveUV"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_XSpeed("X轴方向的纹理滚动速度：", Range(0, 10)) = 1
		_YSpeed("Y轴方向的纹理滚动速度：", Range(0, 10)) = 1
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
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _XSpeed;
			fixed _YSpeed;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed2 UV = i.uv;
				fixed xV = _XSpeed * _Time.x;
				fixed yV = _YSpeed * _Time.y;
				UV += fixed2(xV, yV);
				fixed4 col = tex2D(_MainTex, UV);
				return col;
			}
			ENDCG
		}
	}
}
