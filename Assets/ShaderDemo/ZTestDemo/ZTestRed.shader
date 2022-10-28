Shader "Custom/ZTestRed"
{
	Properties
	{
		_MainColor("MainColor", color) = (0.5,0.5,0.5,1)
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent" } //非透明的要先渲染
		//ZWrite Off
		ZTest Greater
		Pass
		{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag       
		#include "UnityCG.cginc"

		fixed4 _MainColor;

		struct a2v
		{
		float4 vertex : POSITION;
		};

		struct v2f
		{
		float4 pos : SV_POSITION;
		};

		v2f vert(a2v v)
		{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
		return _MainColor;
		}
		ENDCG
		}
	}
}