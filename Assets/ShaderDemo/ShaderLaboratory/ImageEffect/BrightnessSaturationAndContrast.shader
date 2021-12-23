Shader "Custom/BrightnessSaturationAndContrast"
{
	Properties
	{
		_MainTex ("Base(RGB)", 2D) = "white" {}
		_Brightness ("Brightness",Float) = 1
		_Saturation ("Saturation",Float) = 1
		_Contrast ("Contrast",Float) = 1
	}
	SubShader
	{
		Pass
		{
			ZTest Always 
			Cull Off 
			ZWrite Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct v2f
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Brightness;
			float _Saturation;
			float _Contrast;

			v2f vert (appdata_img v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 renderTex = tex2D(_MainTex,i.uv);

			    fixed3 finalColor = renderTex.rgb * _Brightness;//亮度：直接相乘

				fixed luminance = 0.2125 * renderTex.r + 0.7154 *renderTex.g + 0.0721*renderTex.b;
				fixed3 luminanceColor = fixed3(luminance, luminance, luminance);
				finalColor = lerp(luminanceColor, finalColor, _Saturation);//饱和度:将颜色值转化，再进行插值

				fixed3 avgColor = fixed3(0.5, 0.5, 0.5);//对比度：与灰色插值
				finalColor = lerp(avgColor, finalColor, _Contrast);
			
				return fixed4(finalColor, renderTex.a);
			}
			ENDCG
		}
	}
	Fallback Off
}
