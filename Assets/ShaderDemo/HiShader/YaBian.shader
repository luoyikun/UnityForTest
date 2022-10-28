Shader "MyShader/YaBian"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Value("YaBianValue",Range(0, 1)) = 0
		_Bottom("Bottom", float) = 0

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

			float _Value;
			float _Bottom;

            v2f vert (appdata v)
            {
				v2f o;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				// 模型空间转到世界空间
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				// 压Y轴位置 这里把世界空间下顶点的y减去最低部y的值乘上一个系数
				// 然后再用y去减去这个值，就可以通过这个系数来控制兔子被压扁的程度
				float y = worldPos.y - (worldPos.y - _Bottom) * _Value;
				// 最终世界空间位置
				float3 tempWorld = float3(worldPos.x, y, worldPos.z);
				// 世界空间转裁剪空间
				o.vertex = UnityWorldToClipPos(tempWorld);
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
    }
}
