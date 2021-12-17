Shader "Unlit/RotataeVertices"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_RotationSpeed ("Rotation Speed", Float) = 2.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _RotationSpeed;
			
			float4 CalculateRotation(float4 pos)
			{
			    float rotation=_RotationSpeed*_Time.y;
			    float s,c;
				sincos(radians(rotation), s, c);
			    float2x2 rotMatrix=float2x2(c,-s,s,c);
			    pos.xy=mul(pos.xy,rotMatrix);
			    
			    return pos;
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				float4 k=float4(v.vertex.x,v.vertex.z,1.0,1.0);
				k=CalculateRotation(k);
				o.vertex = UnityObjectToClipPos(float4(k.x,v.vertex.y,k.y,v.vertex.w));
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
