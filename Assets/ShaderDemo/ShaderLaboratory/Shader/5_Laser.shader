Shader "MyShaderTest/5_Laser"
{
	Properties
	{
		_Color ("Color",Color) = (1,1,1,1)

		_RampTex("Ramp Texture", 2D) = "white" {}
		_RampScale("Ramp Scale", Range(0,1)) = 1
		_AlphaScale("Alpha Size",Range(0,1)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" 
			   "Queue" = "Transparent" 
			   "IgnoreProjector" = "True" }

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{				
				float4 pos : SV_POSITION;
				float3 worldPos : TEXCOORD0;
				half3 worldNormal : TEXCOORD1;
			};

			fixed4 _Color;
			sampler2D _RampTex;
			half _RampScale;
			half _AlphaScale;

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

				fixed3 worldNormal = normalize(i.worldNormal);

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyx;

				fixed NdotL = dot(worldNormal, worldLightDir);
				
				fixed alpha = _AlphaScale * (0.5 - NdotL * 0.5);

				//alpha = _AlphaScale * (0.5 + NdotL * 0.5);

				fixed diff = 0.5 + NdotL * 0.5;

				fixed3 diffuse_Ramp = _LightColor0 * tex2D(_RampTex,fixed2(diff,diff)).rgb; 
				
				fixed3 diffuse = _LightColor0 * diff;

				fixed3 finalColor = lerp(diffuse, diffuse_Ramp, _RampScale) + ambient;

				return fixed4(_Color.rgb * finalColor,alpha);
			}
			ENDCG
		}
	}
}
