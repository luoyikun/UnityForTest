Shader "PlanarShadow/Shadow"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ShadowInvLen ("ShadowInvLen", float) = 1.0 //0.4449261
	
	}
	
	SubShader
	{
		//在所有Geometry渲染完以后渲染。主要是考虑到Pass2渲染阴影的时候使用了AlphaBlend, 想在所有的Geometry渲染完以后再AlphaBlend, 得出正确的渲染效果
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry+10" }
		LOD 100
		
		//unity生成的默认shader, 也就是角色的渲染
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			
			ENDCG
		}

			//影子
		Pass
		{		
			Blend SrcAlpha  OneMinusSrcAlpha
			ZWrite Off
			Cull Back
			ColorMask RGB
			
			//模版状态：通过使用模版，对于需要做淡化美观的阴影，消除了阴影重叠的现象
			Stencil
			{
				Ref 0			//参考值
				Comp Equal			//条件，关键字有，Greater（>），GEqual（>=），Less（<），LEqual（<=），Equal（=），NotEqual（!=），Always（总是满足），Never（总是不满足）
				WriteMask 255		//输出掩码，当写入模板缓冲时进行掩码操作（按位与【&】），writeMask取值范围是0-255的整数，默认值也是255，即当修改stencilBufferValue值时，写入的仍然是原始值
				ReadMask 255  //读取掩码，取值范围也是0-255的整数，默认值为255，二进制位11111111，即读取的时候不对referenceValue和stencilBufferValue产生效果，读取的还是原始值
				//Pass IncrSat
				Pass Invert  //条件满足后的处理
				Fail Keep  //条件不满足后的处理
				ZFail Keep  //深度测试失败后的处理
			}
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag

			//寄存器值
			float4 _ShadowPlane;//阴影平面，用一个Vector4就是表示平面
			float4 _ShadowProjDir;//灯光方向
			float4 _WorldPos;//人物的当前坐标
			float _ShadowInvLen;//阴影长度
			float4 _ShadowFadeParams;//淡化参数
			float _ShadowFalloff;
			
			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 xlv_TEXCOORD0 : TEXCOORD0;
				float3 xlv_TEXCOORD1 : TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;

				float3 lightdir = normalize(_ShadowProjDir);
				float3 worldpos = mul(unity_ObjectToWorld, v.vertex).xyz;
				// _ShadowPlane.w = p0 * n  // 平面的w分量就是p0 * n
				float distance = (_ShadowPlane.w - dot(_ShadowPlane.xyz, worldpos)) / dot(_ShadowPlane.xyz, lightdir.xyz);
				worldpos = worldpos + distance * lightdir.xyz;
				o.vertex = mul(unity_MatrixVP, float4(worldpos, 1.0));
				o.xlv_TEXCOORD0 = _WorldPos.xyz;
				o.xlv_TEXCOORD1 = worldpos;

				return o;
			}
			
			float4 frag(v2f i) : SV_Target
			{
				float3 posToPlane_2 = (i.xlv_TEXCOORD0 - i.xlv_TEXCOORD1);
				float4 color;
				color.xyz = float3(0.0, 0.0, 0.0);
				
				// 下面两种阴影衰减公式都可以使用(当然也可以自己写衰减公式)
				// 王者荣耀的衰减公式
				color.w = (pow((1.0 - clamp(((sqrt(dot(posToPlane_2, posToPlane_2)) * _ShadowInvLen) - _ShadowFadeParams.x), 0.0, 1.0)), _ShadowFadeParams.y) * _ShadowFadeParams.z);

				// 另外的阴影衰减公式
				//color.w = 1.0 - saturate(distance(i.xlv_TEXCOORD0, i.xlv_TEXCOORD1) * _ShadowFalloff);

				return color;
			}
			
			ENDCG
		}
	}
}
