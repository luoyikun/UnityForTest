Shader "MyShaderTest/1_WarpLighting" 
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)

		_Wrap("Wrap", Range(0,1)) = 0

		_FixColor("Fix Color",Color) = (1,0,0,1)

		_FixWidth("Fix Width",Range(0,0.5)) = 0

		_SpecularColor("Specular Color",Color) = (1,1,1,1)

		_SpecGloss("_Gloss",float) = 20
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
			Tags {"LightMode" = "ForwardBase"}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			fixed4 _Color;
			fixed _Wrap;
			fixed4 _FixColor;
			fixed _FixWidth;
			fixed4 _SpecularColor;
			float _SpecGloss;

			struct a2v
			{
				float4 pos : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
			};

			v2f vert(a2v v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.pos);

				o.worldPos = mul(unity_ObjectToWorld, v.pos);

				o.worldNormal = UnityObjectToWorldNormal(v.normal);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				half3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

				half3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

				half3 worldNormal = normalize(i.worldNormal);

				fixed3 ambient = (UNITY_LIGHTMODEL_AMBIENT * _Color).rgb;

				fixed wrap_diff = saturate((dot(worldNormal, worldLightDir) + _Wrap) / (1 + _Wrap));

				fixed3 diffuse = _LightColor0.rgb * _Color.rgb * saturate(wrap_diff);

				fixed3 fixColor = _LightColor0.rgb * _FixColor * smoothstep(0, _FixWidth, wrap_diff) * smoothstep(_FixWidth * 2, _FixWidth, wrap_diff);

				fixed3 h = normalize(worldViewDir + worldLightDir);

				fixed3 specular = _LightColor0.rgb * _SpecularColor.rgb * pow(saturate(dot(worldNormal, h)), _SpecGloss);

				fixed3 color = diffuse + ambient + specular + fixColor;

				return fixed4(color, 1);
			}

			ENDCG	
		}
	}
	FallBack "Specular"
}
