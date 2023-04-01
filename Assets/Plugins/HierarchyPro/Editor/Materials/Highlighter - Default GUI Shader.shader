Shader "Hidden/EM-X/Highlighter - Default GUI Shader"
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
		fixed4 vertex : POSITION;
		fixed4 color : COLOR;
		fixed2 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f {
		fixed4 vertex : SV_POSITION;
		fixed4 color : COLOR;
		fixed2 texcoord : TEXCOORD0;
		fixed2 clipUV : TEXCOORD1;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	uniform fixed4 _Mask;
	uniform fixed4 _MainTex_ST;
	uniform fixed4x4 unity_GUIClipTextureMatrix;

	v2f vert(appdata_t v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.vertex = UnityObjectToClipPos(v.vertex);
		fixed3 eyePos = UnityObjectToViewPos(v.vertex);
		o.clipUV = mul(unity_GUIClipTextureMatrix, fixed4(eyePos.xy, 0, 1.0));
		o.color = v.color;// v.color;
		o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
		return o;
	}
	

	 float LinearToGammaSpaceExact2(float value)
	{
		 return value + 0.155;
		 return pow(value, 0.65454545F);

		if (value <= 0.0F)
			return 0.0F;
		else if (value <= 0.0031308F)
			return 12.92F * value;
		else if (value < 1.0F)
			return 1.055F * pow(value, 0.4166667F) - 0.055F;
		else
			return pow(value, 0.45454545F);
	}
	inline half3 LinearToGammaSpace2(half3 linRGB)
	{
		//linRGB = max(saturate(linRGB), half3(0.h, 0.h, 0.h));
		//return saturate(max(1.055h * pow(linRGB, 0.496666667h) - 0.155h, 0.h)); //0.416666667
		linRGB.r = LinearToGammaSpaceExact2(linRGB.x);
		linRGB.g = LinearToGammaSpaceExact2(linRGB.g);
		linRGB.b = LinearToGammaSpaceExact2(linRGB.b);
		return linRGB;
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
	//colTex.a = LinearToGammaSpaceExact(colTex.a);
	//i.color.rgb = LinearToGammaSpace(i.color.rgb);
	//i.color.rgb = LinearToGammaSpace(i.color.rgb);
	//colTex.rgb = LinearToGammaSpace(colTex.rgb);
	//i.color.rgb = LinearToGammaSpace(i.color.rgb);
#endif
//	colTex.a = pow(colTex.a, 0.6);

	colTex.rgb = saturate(colTex.rgb);
	i.color.rgb = saturate(i.color.rgb);
	//	if (i.color.r == 0) return 0.5;
	colTex.rgb *= i.color.rgb;
	//if (i.color.r <= 0 || i.color.r > 1)  return 0.5;


		colTex.a *= tex2D(_GUIClipTexture, i.clipUV).a *i.color.a;

	return colTex;
	//return fixed4(col.rgb, tex2D(_GUIClipTexture, i.texcoord).a * alpha);
	}
		ENDCG

		/*SubShader {
		Tags{ "ForceSupported" = "True" }

			Lighting Off
			//Blend SrcAlpha One, One One
				Blend SrcAlpha OneMinusSrcAlpha, One One

			Cull Off
			ZWrite Off
			ZTest Always

			Pass{
			CGPROGRAM
			ENDCG
		}
	}*/

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
