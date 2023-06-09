Shader "Custom/UI_CircleMask"
{
	Properties
	{
	}
	
	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"DisableBatching" = "True"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"


			float2 _Center;
			float _Radius;

			float2 _Center2;
			float _Radius2;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
	
			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				half3 pos : TEXCOORD1;
				fixed4 color : COLOR;
				UNITY_VERTEX_OUTPUT_STEREO
			};
	
			v2f o;

			v2f vert (appdata_t v)
			{
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.pos = mul(unity_ObjectToWorld, v.vertex); //片元的世界坐标
				o.texcoord = v.texcoord;
				o.color = v.color;
				return o;
			}
				
			fixed4 frag (v2f IN) : SV_Target
			{

				float dis2 = distance(IN.pos,float3(_Center2, 0));//白圈优先级高先计算
				float smallWidth = 2;
				if (abs(dis2 - _Radius2)< smallWidth)
				{
					float a = lerp(0,1,1- abs(dis2 - _Radius2)/ smallWidth);//基于一个点,做边缘柔滑
					return float4(1, 1, 1, a);

				}

				float dis = distance(IN.pos,float3(_Center, 0));
				clip(dis - _Radius);

				return float4(IN.color.rgba);


			}
			ENDCG
		}
	}


}
