Shader "Hidden/EM-X/Highlighter - Neon Background Soft Additive"
{
	Properties{
		_MainTex("Texture", Any) = "white" {}
	_Mask("Mask", Color) = (1,1,1,1)
	}
		
		CGINCLUDE
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0
	
#include "UnityCG.cginc"

		struct appdata_t {
		float4 vertex : POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f {
		float4 vertex : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		float2 clipUV : TEXCOORD1;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	uniform float4 _Mask;
	uniform float4 _MainTex_ST;
	uniform float4 _MainTex_TexelSize;
	
	uniform fixed4 _Color;
	uniform float4x4 unity_GUIClipTextureMatrix;

	v2f vert(appdata_t v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.vertex = UnityObjectToClipPos(v.vertex);
		float3 eyePos = UnityObjectToViewPos(v.vertex);
		o.clipUV = mul(unity_GUIClipTextureMatrix, float4(eyePos.xy, 0, 1.0));
		o.color = v.color;// v.color;
		o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
		return o;
	}

	uniform bool _ManualTex2SRGB;
	sampler2D _MainTex;
	sampler2D _GUIClipTexture;

	fixed4 frag(v2f i) : SV_Target
	{

		fixed4 colTex = tex2D(_MainTex, i.texcoord);
	//if (_ManualTex2SRGB)
#if !UNITY_NO_LINEAR_COLORSPACE
	colTex.rgb = LinearToGammaSpace(colTex.rgb);
	//i.color.rgb = LinearToGammaSpace(i.color.rgb);
#endif
	//	colTex.rgb = saturate(colTex.rgb);
	//	i.color.rgb = saturate(i.color.rgb);

	fixed4 col;

	fixed3 _c = colTex.rgb * i.color.rgb;
	fixed _lerp = colTex.r * colTex.a;
	_lerp = pow( _lerp, 10) ;
	if (_MainTex_TexelSize.z < 2 && _MainTex_TexelSize.w < 2) _lerp = 0;	
	col.rgb =  lerp(_c, colTex.rgb, _lerp );
	//return float4(colTex.rgb, 1);
	fixed alpha = colTex.a * tex2D(_GUIClipTexture, i.clipUV).a * i.color.a;// *_Color.a;
	
	
	return float4(col.rgb, alpha);
	//return float4(col.rgb, tex2D(_GUIClipTexture, i.texcoord).a * alpha);
	}
		ENDCG

		SubShader {
		Tags{ "ForceSupported" = "True" }

			Lighting Off
			Blend SrcAlpha One, One One
			//	Blend SrcAlpha OneMinusSrcAlpha, One One

			Cull Off
			ZWrite Off
			ZTest Always

			Pass{
			CGPROGRAM
			ENDCG
		}
	}

	SubShader {
		Tags{ "ForceSupported" = "True" }

			Lighting Off
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			ZWrite Off
			ZTest Always

			Pass{
			CGPROGRAM
			ENDCG
		}
	}
}
