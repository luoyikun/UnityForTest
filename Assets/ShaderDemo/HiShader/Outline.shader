Shader "HiShader/Outline"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_OutlineWidth("Outline Width", Range(0.01, 1)) = 0.01
		_OutLineColor("OutLine Color", Color) = (0.5,0.5,0.5,1)
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					// sample the texture
					fixed4 col = tex2D(_MainTex, i.uv);
					// apply fog
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG
			}

			Pass
			{
				// 开启前向剔除 表示剔除前面 只显示背面
					Cull Front

					CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag

				// 线条宽度
				float _OutlineWidth;
			// 线条颜色
				float4 _OutLineColor;

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					// 法线
					float3 normal : NORMAL;
				};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				// 顶点沿着法线方向外扩(放大模型)
				float4 newVertex = float4(v.vertex.xyz + v.normal * _OutlineWidth * 0.01 ,1);
				// UnityObjectToClipPos(v.vertex) 将模型空间下的顶点转换到齐次裁剪空间
				o.vertex = UnityObjectToClipPos(newVertex);
				return o;
			}

			half4 frag(v2f i) : SV_TARGET
			{
				// 返回线条色彩
					return _OutLineColor;
			}

			ENDCG
			}
		}
}