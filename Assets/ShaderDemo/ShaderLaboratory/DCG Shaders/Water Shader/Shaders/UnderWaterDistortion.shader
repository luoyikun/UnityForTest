// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.18 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.18;sub:START;pass:START;ps:flbk:Unlit/Texture,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5441177,fgcg:0.4627208,fgcb:0.4440961,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:3133,x:33876,y:32624,varname:node_3133,prsc:2|custl-8822-OUT;n:type:ShaderForge.SFN_Tex2d,id:2429,x:33019,y:32690,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_2429,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-3865-OUT;n:type:ShaderForge.SFN_Vector3,id:8810,x:32829,y:34192,cmnt:IRON - base color,varname:node_8810,prsc:2,v1:0,v2:0,v3:0;n:type:ShaderForge.SFN_Tex2d,id:751,x:32045,y:32715,varname:node_751,prsc:2,tex:40a59d660a527ee4cbad46b701a4d2cd,ntxv:3,isnm:True|UVIN-6155-UVOUT,TEX-6875-TEX;n:type:ShaderForge.SFN_ValueProperty,id:2353,x:32374,y:33080,ptovrint:False,ptlb:Intensity,ptin:_Intensity,varname:node_2353,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_ValueProperty,id:4043,x:31265,y:33400,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_4043,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.7;n:type:ShaderForge.SFN_Time,id:6750,x:31265,y:33453,varname:node_6750,prsc:2;n:type:ShaderForge.SFN_Tex2dAsset,id:6875,x:31583,y:32673,ptovrint:False,ptlb:Distortion,ptin:_Distortion,varname:node_6875,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:40a59d660a527ee4cbad46b701a4d2cd,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:6741,x:32112,y:32881,varname:node_6741,prsc:2,tex:40a59d660a527ee4cbad46b701a4d2cd,ntxv:0,isnm:False|UVIN-6391-UVOUT,TEX-6875-TEX;n:type:ShaderForge.SFN_TexCoord,id:5080,x:31263,y:32867,varname:node_5080,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:61,x:31845,y:33298,varname:node_61,prsc:2|A-4043-OUT,B-6750-TSL;n:type:ShaderForge.SFN_Add,id:4467,x:31612,y:33047,varname:node_4467,prsc:2|A-44-OUT,B-2345-OUT;n:type:ShaderForge.SFN_Vector2,id:2345,x:31612,y:33183,varname:node_2345,prsc:2,v1:0.5,v2:0.5;n:type:ShaderForge.SFN_Panner,id:6391,x:31845,y:33080,varname:node_6391,prsc:2,spu:-1,spv:1|UVIN-4467-OUT,DIST-61-OUT;n:type:ShaderForge.SFN_Panner,id:6155,x:31850,y:32922,varname:node_6155,prsc:2,spu:1,spv:-1|UVIN-44-OUT,DIST-61-OUT;n:type:ShaderForge.SFN_Add,id:1254,x:32291,y:32680,varname:node_1254,prsc:2|A-751-R,B-6741-R;n:type:ShaderForge.SFN_Add,id:507,x:32291,y:32813,varname:node_507,prsc:2|A-751-G,B-6741-G;n:type:ShaderForge.SFN_Append,id:2851,x:32482,y:32711,varname:node_2851,prsc:2|A-1254-OUT,B-507-OUT;n:type:ShaderForge.SFN_TexCoord,id:1448,x:32482,y:32527,varname:node_1448,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:1349,x:32551,y:32968,varname:node_1349,prsc:2|A-2851-OUT,B-2353-OUT;n:type:ShaderForge.SFN_Add,id:3865,x:32709,y:32614,varname:node_3865,prsc:2|A-1448-UVOUT,B-1349-OUT;n:type:ShaderForge.SFN_Color,id:8290,x:33220,y:33127,ptovrint:False,ptlb:Tint,ptin:_Tint,varname:node_8290,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.275519,c2:0.6100883,c3:0.7205882,c4:0;n:type:ShaderForge.SFN_Multiply,id:8822,x:33436,y:33048,varname:node_8822,prsc:2|A-2429-RGB,B-8290-RGB;n:type:ShaderForge.SFN_Multiply,id:44,x:31340,y:33078,varname:node_44,prsc:2|A-5080-UVOUT,B-1136-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1136,x:31152,y:33204,ptovrint:False,ptlb:Scale,ptin:_Scale,varname:node_1136,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;proporder:2353-4043-6875-8290-1136-2429;pass:END;sub:END;*/

Shader "Hidden/DCG/Water Shader/Underwater" {
    Properties {
        _Intensity ("Intensity", Float ) = 0.1
        _Speed ("Speed", Float ) = 0.7
        _Distortion ("Distortion", 2D) = "bump" {}
        _Tint ("Tint", Color) = (0.275519,0.6100883,0.7205882,0)
        _Scale ("Scale", Float ) = 1
        _MainTex ("MainTex", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Intensity;
            uniform float _Speed;
            uniform sampler2D _Distortion; uniform float4 _Distortion_ST;
            uniform float4 _Tint;
            uniform float _Scale;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
/////// Vectors:
////// Lighting:
                float4 node_6750 = _Time + _TimeEditor;
                float node_61 = (_Speed*node_6750.r);
                float2 node_44 = (i.uv0*_Scale);
                float2 node_6155 = (node_44+node_61*float2(1,-1));
                float3 node_751 = UnpackNormal(tex2D(_Distortion,TRANSFORM_TEX(node_6155, _Distortion)));
                float2 node_6391 = ((node_44+float2(0.5,0.5))+node_61*float2(-1,1));
                float3 node_6741 = UnpackNormal(tex2D(_Distortion,TRANSFORM_TEX(node_6391, _Distortion)));
                float2 node_3865 = (i.uv0+(float2((node_751.r+node_6741.r),(node_751.g+node_6741.g))*_Intensity));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_3865, _MainTex));
                float3 finalColor = (_MainTex_var.rgb*_Tint.rgb);
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Unlit/Texture"
    CustomEditor "ShaderForgeMaterialInspector"
}
