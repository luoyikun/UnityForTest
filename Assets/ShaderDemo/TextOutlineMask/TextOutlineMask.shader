Shader "Custom/TextOutlineMask" {
	Properties
	{
		_MainTex("Text Texture", 2D) = "white" {}
		_OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
		_OutlineWidth("Outline Width", Range(0, 1)) = 0.1
	}
		SubShader
		{
			Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
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
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _OutlineColor;
				float _OutlineWidth;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 color = tex2D(_MainTex, i.uv);

				// Ìí¼ÓÃè±ßÐ§¹û
				fixed4 outlineColor = _OutlineColor;
				float outlineWidth = _OutlineWidth;

				float2 uvOffset = float2(outlineWidth, 0);
				float4 outline = tex2D(_MainTex, i.uv + uvOffset) +
								 tex2D(_MainTex, i.uv - uvOffset) +
								 tex2D(_MainTex, i.uv + uvOffset.yx) +
								 tex2D(_MainTex, i.uv - uvOffset.yx);

				outline *= outlineColor;
				outline.a = color.a;

				return max(outline, color);
			}
			ENDCG
		}
		}
}