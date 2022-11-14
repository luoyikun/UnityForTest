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
                float4 vertex : POSITION; //顶点位置
				float3 normal :NORMAL; //法线
                float2 uv : TEXCOORD0;
            };

	struct v2f
	{
		float2 uv : TEXCOORD0;

		// 计算光照需要用到法线和世界位置
		// 通常使用TEXCOORDn语义来修饰float2, float3, float4类型
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
				// 模型空间转到世界空间
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				// 法线向量归一化
				o.worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
				return o;
			}


			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
			// 得到光照方向
			float3 worldLightDir = UnityWorldSpaceLightDir(i.worldPos);
			// NoL代表表面接受的能量大小
			float NoL = dot(i.worldNormal, worldLightDir);
			// 计算half-lambert亮度值
			float halfLambert = NoL * 0.5 + 0.5;
			// apply fog
			//UNITY_APPLY_FOG(i.fogCoord, col);
			return col * halfLambert;
			}

            ENDCG
        }
    }
}
