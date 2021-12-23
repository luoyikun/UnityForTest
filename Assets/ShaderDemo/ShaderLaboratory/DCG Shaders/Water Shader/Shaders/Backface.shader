// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.18 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.18;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:1,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5441177,fgcg:0.4627208,fgcb:0.4440961,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:7675,x:32773,y:32756,varname:node_7675,prsc:2|alpha-7454-OUT,refract-2036-OUT;n:type:ShaderForge.SFN_Vector1,id:7454,x:32441,y:32894,varname:node_7454,prsc:2,v1:0;n:type:ShaderForge.SFN_Tex2dAsset,id:3414,x:30923,y:33102,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:node_3414,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:a0c55168c4eb26646a6bd97b7c8e4da0,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:9810,x:31555,y:33075,varname:node_9810,prsc:2,tex:a0c55168c4eb26646a6bd97b7c8e4da0,ntxv:0,isnm:False|UVIN-9690-UVOUT,TEX-3414-TEX;n:type:ShaderForge.SFN_Tex2d,id:3463,x:31555,y:33203,varname:node_3463,prsc:2,tex:a0c55168c4eb26646a6bd97b7c8e4da0,ntxv:0,isnm:False|UVIN-1291-UVOUT,TEX-3414-TEX;n:type:ShaderForge.SFN_TexCoord,id:8884,x:31200,y:32254,varname:node_8884,prsc:2,uv:0;n:type:ShaderForge.SFN_ObjectScale,id:4972,x:31200,y:32426,varname:node_4972,prsc:2,rcp:False;n:type:ShaderForge.SFN_Multiply,id:314,x:31435,y:32279,varname:node_314,prsc:2|A-8884-UVOUT,B-8149-OUT,C-2527-OUT;n:type:ShaderForge.SFN_Slider,id:2527,x:31100,y:32613,ptovrint:False,ptlb:Size,ptin:_Size,varname:node_2527,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5641026,max:1;n:type:ShaderForge.SFN_Vector2,id:6378,x:31697,y:32561,varname:node_6378,prsc:2,v1:0.5,v2:0.5;n:type:ShaderForge.SFN_Add,id:2607,x:31707,y:32406,varname:node_2607,prsc:2|A-314-OUT,B-6378-OUT;n:type:ShaderForge.SFN_Panner,id:9690,x:32033,y:32341,varname:node_9690,prsc:2,spu:1,spv:-1|UVIN-9091-OUT,DIST-2453-OUT;n:type:ShaderForge.SFN_Panner,id:1291,x:32033,y:32184,varname:node_1291,prsc:2,spu:-1,spv:1|UVIN-314-OUT,DIST-2453-OUT;n:type:ShaderForge.SFN_Append,id:8149,x:31414,y:32438,varname:node_8149,prsc:2|A-4972-X,B-4972-Z;n:type:ShaderForge.SFN_Time,id:8110,x:31755,y:32820,varname:node_8110,prsc:2;n:type:ShaderForge.SFN_Slider,id:685,x:31598,y:32723,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_685,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2.283741,max:4;n:type:ShaderForge.SFN_Multiply,id:2453,x:31961,y:32690,varname:node_2453,prsc:2|A-685-OUT,B-8110-TSL;n:type:ShaderForge.SFN_Add,id:863,x:31804,y:33107,varname:node_863,prsc:2|A-9810-R,B-3463-R;n:type:ShaderForge.SFN_Add,id:6283,x:31804,y:33231,varname:node_6283,prsc:2|A-9810-G,B-3463-G;n:type:ShaderForge.SFN_Append,id:6575,x:32088,y:33118,varname:node_6575,prsc:2|A-863-OUT,B-6283-OUT,C-9119-OUT;n:type:ShaderForge.SFN_Vector1,id:9119,x:32051,y:33294,varname:node_9119,prsc:2,v1:1;n:type:ShaderForge.SFN_Lerp,id:9680,x:32366,y:33123,varname:node_9680,prsc:2|A-4056-OUT,B-6575-OUT,T-1201-OUT;n:type:ShaderForge.SFN_Vector3,id:4056,x:32135,y:32989,varname:node_4056,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Slider,id:1201,x:32232,y:33296,ptovrint:False,ptlb:Normal Intensity,ptin:_NormalIntensity,varname:node_1201,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.4781802,max:1;n:type:ShaderForge.SFN_ComponentMask,id:8123,x:32727,y:33264,varname:node_8123,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-9680-OUT;n:type:ShaderForge.SFN_Multiply,id:9091,x:31932,y:32490,varname:node_9091,prsc:2|A-2607-OUT,B-1770-OUT;n:type:ShaderForge.SFN_Vector1,id:1770,x:31834,y:32601,varname:node_1770,prsc:2,v1:0.75;n:type:ShaderForge.SFN_Vector1,id:1327,x:32580,y:33474,varname:node_1327,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Multiply,id:2036,x:32919,y:33421,varname:node_2036,prsc:2|A-8123-OUT,B-1327-OUT;proporder:3414-1201-2527-685;pass:END;sub:END;*/

Shader "DCG/Water Shader/Backface Distortion" {
    Properties {
        _Normal ("Normal", 2D) = "bump" {}
        _NormalIntensity ("Normal Intensity", Range(0, 1)) = 0.4781802
        _Size ("Size", Range(0, 1)) = 0.5641026
        _Speed ("Speed", Range(0, 4)) = 2.283741
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Front
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _Size;
            uniform float _Speed;
            uniform float _NormalIntensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                o.pos = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float4 node_8110 = _Time + _TimeEditor;
                float node_2453 = (_Speed*node_8110.r);
                float2 node_314 = (i.uv0*float2(objScale.r,objScale.b)*_Size);
                float2 node_9690 = (((node_314+float2(0.5,0.5))*0.75)+node_2453*float2(1,-1));
                float3 node_9810 = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_9690, _Normal)));
                float2 node_1291 = (node_314+node_2453*float2(-1,1));
                float3 node_3463 = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_1291, _Normal)));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (lerp(float3(0,0,1),float3((node_9810.r+node_3463.r),(node_9810.g+node_3463.g),1.0),_NormalIntensity).rg*0.1);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
/////// Vectors:
////// Lighting:
                float3 finalColor = 0;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,0.0),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
