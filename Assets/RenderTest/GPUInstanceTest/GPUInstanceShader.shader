Shader "GPUInstanceShader"{
	Properties{
		_Color("Color", Color) = (1, 1, 1, 1)
	}

		SubShader{
			Tags { "RenderType" = "Opaque" }

			Pass{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_instancing                        // ������ʵ���ı�������
				#include "UnityCG.cginc"

				struct appdata {
					float4 vertex : POSITION;
					UNITY_VERTEX_INPUT_INSTANCE_ID                      //������ɫ���� InstancingID ����
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					UNITY_VERTEX_INPUT_INSTANCE_ID                      //ƬԪ��ɫ���� InstancingID ����
				};

				UNITY_INSTANCING_BUFFER_START(Props)                    // �����ʵ����������

					UNITY_DEFINE_INSTANCED_PROP(float4, _Color)

				UNITY_INSTANCING_BUFFER_END(Props)

				v2f vert(appdata v) {
					v2f o;

					UNITY_SETUP_INSTANCE_ID(v);                         //װ�� InstancingID
					UNITY_TRANSFER_INSTANCE_ID(v, o);                   //���뵽�ṹ�д���ƬԪ��ɫ��

					o.vertex = UnityObjectToClipPos(v.vertex);          // �������������ת��Ļ�ü�����
					return o;
				}

				fixed4 frag(v2f i) : SV_Target{
					UNITY_SETUP_INSTANCE_ID(i);                         //װ�� InstancingID
					return UNITY_ACCESS_INSTANCED_PROP(Props, _Color);  //��ȡ��ʵ���еĵ�ǰʵ����Color���Ա���ֵ
				}
				ENDCG
			}
	}
}