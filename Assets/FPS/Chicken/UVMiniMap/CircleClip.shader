Shader "Custom/CircleClip"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

			//add
			_Center("Center", vector) = (0, 0, 0, 0)
			_Silder("_Silder", float) = 1000 // sliders
			_BigRSquare("_BigRSquare",float) = 1000
			_SmallWidth("_SmallWidth",float) = 1
			_SmallRSquare("_SmallRSquare",float) = 1000
			//add
			_PointInCicle("_PointInCicle", vector) = (0, 0, 0, 0)

			[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Stencil
			{
				Ref[_Stencil]
				Comp[_StencilComp]
				Pass[_StencilOp]
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
			}

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest[unity_GUIZTestMode]
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask[_ColorMask]

			Pass
			{
				Name "Default"
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0

				#include "UnityCG.cginc"
				#include "UnityUI.cginc"

				#pragma multi_compile __ UNITY_UI_ALPHACLIP

				struct appdata_t
				{
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord  : TEXCOORD0;
					float4 worldPosition : TEXCOORD1;
					UNITY_VERTEX_OUTPUT_STEREO
				};

				fixed4 _Color;
				fixed4 _TextureSampleAdd;
				float4 _ClipRect;
				float _Silder;
				float2 _Center;
				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _BigRSquare;
				float _SmallRSquare;
				float2 _CenterSmall;
				float _SmallWidth;
				v2f vert(appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					//o.texcoord = v.texcoord;
					o.color = v.color;
					return o;
				}

				//ֱ�Ӵ���뾶
			//	fixed4 frag(v2f i) : SV_Target
			//	{
			//	   float2 center = float2(i.worldPosition.x,i.worldPosition.y) - float2(_Center.x, _Center.y);
			//	   float dis = sqrt(center.x * center.x + center.y * center.y);
			//	   clip(dis - _Silder);
			///*	   if (dis < _Silder)
			//		{
			//			i.color.a = 0;
			//		}*/

			//	   //clip(i.color.a - 0.001);
			//	   fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
			//	   return col;
			//	}
				
				//����뾶��һ��
				fixed4 frag(v2f i) : SV_Target
				{
					//��ʾ��Ȧ
					float2 diffCurWithSmallllCenter = float2(i.worldPosition.x, i.worldPosition.y) - float2(_CenterSmall.x, _CenterSmall.y);
					float disCurWithSmallCenter = sqrt(diffCurWithSmallllCenter.x * diffCurWithSmallllCenter.x + diffCurWithSmallllCenter.y * diffCurWithSmallllCenter.y);
					if (abs(disCurWithSmallCenter - _SmallRSquare) < _SmallWidth )
					{
						//float a = lerp(0, 1, 1 - abs(disCurWithSmallCenter - _SmallRSquare) / _SmallWidth);//����һ����,����Ե�Ử
	
						return float4(1, 1, 1, 1);
					}


					//float dis2 = distance(i.worldPosition,float3(_CenterSmall, 0));//��Ȧ���ȼ����ȼ���
					//float smallWidth = 2;
					//if (abs(dis2 - _Radius2) < smallWidth)
					//{
					//	float a = lerp(0,1,1 - abs(dis2 - _Radius2) / smallWidth);//����һ����,����Ե�Ử
					//	return float4(1, 1, 1, a);

					//}

				   float2 center = float2(i.worldPosition.x, i.worldPosition.y) - float2(_Center.x, _Center.y);
				   float disSquare = (center.x * center.x + center.y * center.y);

				   //float radius = distance(_Center, _PointInCicle);
				   //_BigRSquare = radius;
				   clip(disSquare - _BigRSquare);

				   fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
				   return col;
				}
					

				//surface������������
	//			fixed4 frag(v2f i) : SV_Target
	//			{
	//				float4 selfPos = mul(unity_ObjectToWorld, i.worldPosition);

	//				// ���㵱ǰ���ص�Բ�ĵľ���
	//				float dist = length(selfPos.xy - _Center.xy);

	//				// ���������ڰ뾶����ü���������
	//				//clip(dist - _Silder);
	//				if (dist < _Silder)
	//				{
	//					i.color.a = 0;
	//				}

	///*				float2 center = float2(i.worldPosition.x,i.worldPosition.y) - float2(_Center.x, _Center.y);
	//				float dis = sqrt(center.x * center.x + center.y * center.y);
	//				clip(dis - _Silder);*/
	//				fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
	//				return col;
	//			}

				//v2f vert(appdata_t IN)
				//{
				//	v2f OUT;
				//	UNITY_SETUP_INSTANCE_ID(IN);
				//	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				//	OUT.worldPosition = IN.vertex;
				//	OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
				//	OUT.texcoord = IN.texcoord;

				//	OUT.color = IN.color * _Color;
				//	return OUT;
				//}

				//

				//fixed4 frag(v2f IN) : SV_Target
				//{
				//	half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

				//	color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

				//	#ifdef UNITY_UI_ALPHACLIP
				//	clip(color.a - 0.001);
				//	#endif

				//	//-------------------add----------------------
				//	//�۶�
				//	color.a *= (distance(IN.worldPosition.xy,_Center.xy) > _Silder);
				//	//1���ؽ���͸�� ���ⶴ��Ե����
				//	float d = distance(IN.worldPosition.xy,_Center.xy);
				//	if (d > _Silder && d < (_Silder + 1)) {
				//		color.a *= (d - _Silder);
				//	}

				//	color.rgb *= color.a;
				//	//-------------------add----------------------


				//	return color;
				//}

					
			ENDCG
			}
		}
}