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
					o.color = v.color;
					return o;
				}

			
				fixed4 frag(v2f i) : SV_Target
				{
					//��ʾ��Ȧ
					float2 diffCurWithSmallllCenter = float2(i.worldPosition.x, i.worldPosition.y) - float2(_CenterSmall.x, _CenterSmall.y);
					float disCurWithSmallCenter = sqrt(diffCurWithSmallllCenter.x * diffCurWithSmallllCenter.x + diffCurWithSmallllCenter.y * diffCurWithSmallllCenter.y);
					if (abs(disCurWithSmallCenter - _SmallRSquare) < _SmallWidth )
					{
						
						return float4(1, 1, 1, 1);
					}


				   float2 center = float2(i.worldPosition.x, i.worldPosition.y) - float2(_Center.x, _Center.y);
				   float disSquare = (center.x * center.x + center.y * center.y);

				   //�ȽϾ����С�����Բ��ÿ�ƽ��
				   clip(disSquare - _BigRSquare);

				   fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
				   return col;
				}

			ENDCG
			}
		}
}