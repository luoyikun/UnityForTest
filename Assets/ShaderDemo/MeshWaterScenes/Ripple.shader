Shader "Custom/Ripple" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_RippleTex("RippleTex", 2D) = "white" {}
		_RippleScale("RippleScale",Range(1,10)) =1

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0
		#define PI 3.141592653
		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			half2 texcoord;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		sampler2D _RippleTex;
		float _RippleScale;
		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		//计算波纹的主函数
		float3 ComputeRipple(float2 uv, float t)
		{
			//波纹贴图采样，并把采样的高度值扩展到-1到1。
			float4 ripple = tex2D(_RippleTex, uv);
			ripple.yz = ripple.yz * 2.0 - 1.0;
			//获取波纹的时间,从A通道获取不同的波纹时间,
			float dropFrac = frac(ripple.a + t);
			//把时间限制在R通道内
			float timeFrac = dropFrac - 1.0 + ripple.x;
			//做淡出处理
			float dropFactor = 1-saturate( dropFrac);
			//计算最终的高度，用一个sin计算出随时间的振幅，修改一下值就知道什么效果了
			float final = dropFactor* sin(clamp(timeFrac * 9.0, 0.0, 4.0) * PI);
			return float3(ripple.yz * final, 1.0);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;

			//调用方法，获取高度。这里我懒得用一个新uv，索性直接用的主贴图UV
			float3 ripple = ComputeRipple(IN.uv_MainTex / _RippleScale, _Time.y);
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			//赋值到法线上
			o.Normal = ripple;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
