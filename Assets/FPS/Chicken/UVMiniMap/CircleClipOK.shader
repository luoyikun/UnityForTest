Shader "Custom/CircleClipSurface" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
	}

		SubShader{
			Tags { "RenderType" = "Opaque" }

			CGPROGRAM
			#pragma surface surf Standard

		// 声明圆心坐标和半径变量
		uniform float4 _Circle;
	sampler2D _MainTex;
	fixed4 _Color;
	struct Input {
		float2 uv_MainTex;
		float3 worldPos;
		float3 worldNormal;
		float4 screenPos;
	};

	void surf(Input IN, inout SurfaceOutputStandard o) {
		// 从世界坐标系转换到自身坐标系
		float4 selfPos = mul(unity_ObjectToWorld, IN.worldPos);

		// 计算当前像素到圆心的距离
		float dist = length(selfPos.xy - _Circle.xy);

		// 如果距离大于半径，则裁剪掉该像素
		//clip(dist - _Circle.w);

		if ((dist - _Circle.w) < 0)
		{
			o.Alpha = 0;
		}
		else
		{
			o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a;
		}

		clip(o.Alpha - 0.001);
		// 正常输出纹理颜色

		o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
		


	}
	ENDCG
	}

		FallBack "Diffuse"
}
