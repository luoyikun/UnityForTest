// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.18 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.18;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5441177,fgcg:0.4627208,fgcb:0.4440961,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:557,x:34512,y:32574,varname:node_557,prsc:2|emission-9578-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:1977,x:31746,y:32638,varname:node_1977,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3062,x:31734,y:32824,varname:node_3062,prsc:2|A-1977-XYZ,B-5979-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5979,x:31734,y:33001,ptovrint:False,ptlb:Caustics Scale,ptin:_CausticsScale,varname:node_5979,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_ComponentMask,id:8193,x:32110,y:32826,varname:node_8193,prsc:2,cc1:0,cc2:2,cc3:-1,cc4:-1|IN-3062-OUT;n:type:ShaderForge.SFN_Tex2d,id:1637,x:32441,y:32758,ptovrint:False,ptlb:Caustic,ptin:_Caustic,varname:node_1637,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:8f73045fe260a1b41ae0ed08a11e457e,ntxv:0,isnm:False|UVIN-8193-OUT;n:type:ShaderForge.SFN_Color,id:1372,x:32441,y:32561,ptovrint:False,ptlb:Caustic Color,ptin:_CausticColor,varname:node_1372,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:6373,x:32717,y:32847,varname:node_6373,prsc:2|A-1372-RGB,B-1637-RGB;n:type:ShaderForge.SFN_ValueProperty,id:1555,x:32530,y:33198,ptovrint:False,ptlb:Height Cut,ptin:_HeightCut,varname:node_1555,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:-22;n:type:ShaderForge.SFN_Clamp01,id:7460,x:32940,y:32828,varname:node_7460,prsc:2|IN-6373-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:7156,x:32530,y:33286,varname:node_7156,prsc:2;n:type:ShaderForge.SFN_Desaturate,id:9852,x:33199,y:33073,varname:node_9852,prsc:2|COL-7063-OUT;n:type:ShaderForge.SFN_Multiply,id:179,x:33171,y:32708,varname:node_179,prsc:2|A-7460-OUT,B-858-OUT,C-6791-OUT;n:type:ShaderForge.SFN_Clamp01,id:1029,x:34060,y:32817,varname:node_1029,prsc:2|IN-3841-OUT;n:type:ShaderForge.SFN_OneMinus,id:858,x:33199,y:32869,varname:node_858,prsc:2|IN-9852-OUT;n:type:ShaderForge.SFN_Add,id:3869,x:32881,y:33208,varname:node_3869,prsc:2|A-1555-OUT,B-7156-Y;n:type:ShaderForge.SFN_Clamp01,id:7063,x:33061,y:33208,varname:node_7063,prsc:2|IN-3869-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5162,x:32530,y:33451,ptovrint:False,ptlb:Underwater Cut,ptin:_UnderwaterCut,varname:node_5162,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;n:type:ShaderForge.SFN_Add,id:1378,x:32813,y:33392,varname:node_1378,prsc:2|A-7156-Y,B-5162-OUT;n:type:ShaderForge.SFN_Clamp01,id:9864,x:33221,y:33306,varname:node_9864,prsc:2|IN-7984-OUT;n:type:ShaderForge.SFN_Desaturate,id:6791,x:32940,y:32974,varname:node_6791,prsc:2|COL-9864-OUT;n:type:ShaderForge.SFN_RemapRange,id:7984,x:33004,y:33392,varname:node_7984,prsc:2,frmn:0,frmx:1,tomn:0,tomx:0.1|IN-1378-OUT;n:type:ShaderForge.SFN_Multiply,id:3841,x:33886,y:32817,varname:node_3841,prsc:2|A-179-OUT,B-2791-OUT;n:type:ShaderForge.SFN_NormalVector,id:5318,x:33899,y:33326,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:4775,x:33920,y:33154,varname:node_4775,prsc:2,dt:1|A-8764-OUT,B-5318-OUT;n:type:ShaderForge.SFN_Clamp01,id:2791,x:33874,y:32956,varname:node_2791,prsc:2|IN-4775-OUT;n:type:ShaderForge.SFN_LightVector,id:8764,x:33618,y:33180,varname:node_8764,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:4760,x:33886,y:32665,ptovrint:False,ptlb:Caustic Intensity,ptin:_CausticIntensity,varname:node_4760,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_Multiply,id:7200,x:34146,y:32662,varname:node_7200,prsc:2|A-4760-OUT,B-1029-OUT;n:type:ShaderForge.SFN_RemapRange,id:4247,x:33915,y:32377,varname:node_4247,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:2|IN-7200-OUT;n:type:ShaderForge.SFN_Clamp01,id:5816,x:34111,y:32265,varname:node_5816,prsc:2|IN-4247-OUT;n:type:ShaderForge.SFN_Multiply,id:2645,x:34175,y:32415,varname:node_2645,prsc:2|A-5816-OUT,B-2605-OUT;n:type:ShaderForge.SFN_Vector1,id:2605,x:34118,y:32603,varname:node_2605,prsc:2,v1:100;n:type:ShaderForge.SFN_Blend,id:9578,x:34515,y:32383,varname:node_9578,prsc:2,blmd:8,clmp:True|SRC-7200-OUT,DST-2645-OUT;proporder:5979-1637-1372-4760-1555-5162;pass:END;sub:END;*/

Shader "DCG/Water Shader/Caustics" {
    Properties {
        _CausticsScale ("Caustics Scale", Float ) = 2
        _Caustic ("Caustic", 2D) = "white" {}
        _CausticColor ("Caustic Color", Color) = (0.5,0.5,0.5,1)
        _CausticIntensity ("Caustic Intensity", Float ) = 3
        _HeightCut ("Height Cut", Float ) = -22
        _UnderwaterCut ("Underwater Cut", Float ) = 10
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers d3d11_9x 
            #pragma target 3.0
            uniform float _CausticsScale;
            uniform sampler2D _Caustic; uniform float4 _Caustic_ST;
            uniform float4 _CausticColor;
            uniform float _HeightCut;
            uniform float _UnderwaterCut;
            uniform float _CausticIntensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
////// Emissive:
                float2 node_8193 = (i.posWorld.rgb*_CausticsScale).rb;
                float4 _Caustic_var = tex2D(_Caustic,TRANSFORM_TEX(node_8193, _Caustic));
                float3 node_7200 = (_CausticIntensity*saturate(((saturate((_CausticColor.rgb*_Caustic_var.rgb))*(1.0 - dot(saturate((_HeightCut+i.posWorld.g)),float3(0.3,0.59,0.11)))*dot(saturate(((i.posWorld.g+_UnderwaterCut)*0.1+0.0)),float3(0.3,0.59,0.11)))*saturate(max(0,dot(lightDirection,i.normalDir))))));
                float3 emissive = saturate((node_7200+(saturate((node_7200*3.0+-1.0))*100.0)));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma exclude_renderers d3d11_9x 
            #pragma target 3.0
            uniform float _CausticsScale;
            uniform sampler2D _Caustic; uniform float4 _Caustic_ST;
            uniform float4 _CausticColor;
            uniform float _HeightCut;
            uniform float _UnderwaterCut;
            uniform float _CausticIntensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                LIGHTING_COORDS(2,3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// Lighting:
                float3 finalColor = 0;
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
