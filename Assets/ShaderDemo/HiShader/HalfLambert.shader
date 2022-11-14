Shader "HiShader/HalfLambert"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION; //����λ��
				float3 normal :NORMAL; //����
                float2 uv : TEXCOORD0;
            };

	struct v2f
	{
		float2 uv : TEXCOORD0;

		// ���������Ҫ�õ����ߺ�����λ��
		// ͨ��ʹ��TEXCOORDn����������float2, float3, float4����
		float3 worldNormal: TEXCOORD1;
		float3 worldPos:TEXCOORD2;

		UNITY_FOG_COORDS(1)
			float4 vertex : SV_POSITION;
	};


            sampler2D _MainTex;
            float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				// ģ�Ϳռ�ת������ռ�
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				// ����������һ��
				o.worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
				return o;
			}


			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
			// �õ����շ���
			float3 worldLightDir = UnityWorldSpaceLightDir(i.worldPos);
			// NoL���������ܵ�������С
			float NoL = dot(i.worldNormal, worldLightDir);
			// ����half-lambert����ֵ
			float halfLambert = NoL * 0.5 + 0.5;
			// apply fog
			//UNITY_APPLY_FOG(i.fogCoord, col);
			return col * halfLambert;
			}

            ENDCG
        }
    }
}
