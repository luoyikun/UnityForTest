Shader "Unlit/Wave"

{

	Properties

	{

		_MainTex("Texture", 2D) = "white" {}

_Ctor("Ctor", float) = 84

		_timeCtor("timector",float) = 60

		_max_dis("maxdis",Range(0,1)) = 0.5

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

		  float4 _ArrayParams[10];

		  float _Ctor;

		  float _timeCtor;

		  float _max_dis;

		v2f vert(appdata v)

		{

			v2f o;

			o.vertex = UnityObjectToClipPos(v.vertex);

			o.uv = TRANSFORM_TEX(v.uv, _MainTex);

			UNITY_TRANSFER_FOG(o,o.vertex);

			return o;

		}
fixed4 frag(v2f i):SV_Target
		{
		  float2 uv = float2(0,0);

			[unroll]

			for (int j = 0; j < 10; j++)

		  {

			float2 dv = float2(_ArrayParams[j].x,_ArrayParams[j].y) - i.uv;

	  float dis = sqrt(dv.x * dv.x + dv.y * dv.y);

	  float sinFactor = sin(dis* _Ctor + _Time.y *_timeCtor);

	  float2 dv1 = normalize(dv);

	  float2 offset = dv1  * sinFactor*max(0,_max_dis - dis)*step(dis,_ArrayParams[j].z)*step(_ArrayParams[j].w,dis);

	  uv += offset;

		  }

		  uv = i.uv + uv / 10;

			return tex2D(_MainTex, uv);

		}

		ENDCG

	}

}

}
