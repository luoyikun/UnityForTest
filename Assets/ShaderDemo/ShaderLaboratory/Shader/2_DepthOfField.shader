Shader "MyShaderTest/2_DepthOfField"
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BlurMainTex("Bloom (RGB)", 2D) = "black" {}
		_LuminanceThreshold ("Luminance Threshold", Float) = 0.5
		_BlurSize ("Blur Size", Float) = 1.0
		_FocusStart("FocusStart", Float) = 0.4
		_FocusEnd("FocusEnd", Float) = 0.6
	}

	SubShader
	{
		ZTest Always Cull Off ZWrite Off

		CGINCLUDE
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		half4 _MainTex_TexelSize;
		sampler2D _CameraDepthTexture;
		sampler2D _BlurMainTex;
		float _BlurSize;
		float _FocusStart;
		float _FocusEnd;

		struct v2fBlur
		{
			float4 pos : SV_POSITION;
			half2 uv[5] : TEXCOORD0;
		};

		//垂直方向(Y轴方向)的高斯模糊
		v2fBlur vertBlurVertical(appdata_img v)
		{
			v2fBlur o;
			o.pos = UnityObjectToClipPos(v.vertex);
			half2 uv = v.texcoord;

			o.uv[0] = uv;
			o.uv[1] = uv + float2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
			o.uv[2] = uv - float2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
			o.uv[3] = uv + float2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
			o.uv[4] = uv - float2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
			return o;
		}

		//水平方向(X轴方向)的高斯模糊
		v2fBlur vertBlurHorizontal(appdata_img v)
		{
			v2fBlur o;
			o.pos = UnityObjectToClipPos(v.vertex);
			half2 uv = v.texcoord;

			o.uv[0] = uv;
			o.uv[1] = uv + float2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
			o.uv[2] = uv - float2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
			o.uv[3] = uv + float2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
			o.uv[4] = uv - float2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;

			return o;
		}

		fixed4 fragBlur(v2fBlur i) : SV_Target
		{
			float weight[3] = { 0.4026,0.2442,0.0545 };
			fixed3 sum = tex2D(_MainTex, i.uv[0]).rgb * weight[0];

			for (int it = 1; it < 3; it++)
			{
				sum += tex2D(_MainTex, i.uv[it * 2 - 1]).rgb * weight[it];
				sum += tex2D(_MainTex, i.uv[2 * it]).rgb * weight[it];
			}

			return fixed4(sum, 1.0);
		}


		struct v2f
		{
			float4 vertex : SV_POSITION;
			half2 uv : TEXCOORD0;
		};


		v2f vertDepthOfField(appdata_img v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;

			return o;
		}


		fixed4 fragDepthOfField(v2f i) : SV_Target
		{
			float linearDepth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv));
			
			float distance = 0; 

			if (linearDepth < _FocusStart)
			{
				distance = abs(linearDepth - _FocusStart);
			}
			else if (linearDepth > _FocusEnd)
			{
				distance = abs(linearDepth - _FocusEnd);
			}			

			half4 color = tex2D(_MainTex,i.uv);
			half4 blurColor = tex2D(_BlurMainTex, i.uv);

			color = lerp(color, blurColor, distance);

			return fixed4(color.rgb,1);
		}

		ENDCG

		Pass
		{  
			CGPROGRAM  
			#pragma vertex vertBlurVertical 
			#pragma fragment fragBlur	
			ENDCG  
		}

		Pass
		{  
			CGPROGRAM  
			#pragma vertex vertBlurHorizontal 
			#pragma fragment fragBlur	
			ENDCG  
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vertDepthOfField
			#pragma fragment fragDepthOfField
			ENDCG
		}
		
	}
	FallBack Off
}

