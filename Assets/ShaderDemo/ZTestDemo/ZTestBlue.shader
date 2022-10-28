Shader "Custom/ZTestBlue"
{
	Properties
	{
		_MainColor("MainColor", color) = (0.5,0.5,0.5,1)
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent-1" } //3000-1,越小，越先渲染

		//ZWrite Off
		//ZTest
		Pass
		{
		Blend SrcAlpha OneMinusSrcAlpha //

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
		//return _MainColor;
			return fixed4(0,0,1,0.6);
		}
		ENDCG
		}
	}
}