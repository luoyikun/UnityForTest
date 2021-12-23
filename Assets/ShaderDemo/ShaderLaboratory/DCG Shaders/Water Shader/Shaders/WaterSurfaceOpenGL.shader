// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.18 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.18;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:True,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5441177,fgcg:0.4627208,fgcb:0.4440961,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:2553,x:35073,y:31126,varname:node_2553,prsc:2|normal-6060-OUT,custl-3434-OUT,alpha-9087-OUT,refract-7011-OUT,voffset-4305-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:141,x:30677,y:33202,ptovrint:False,ptlb:ReflectionTex,ptin:_ReflectionTex,varname:node_141,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7838,x:30865,y:33095,varname:node_7838,prsc:2,ntxv:0,isnm:False|UVIN-7833-UVOUT,TEX-141-TEX;n:type:ShaderForge.SFN_ScreenPos,id:7833,x:30677,y:33021,varname:node_7833,prsc:2,sctp:2;n:type:ShaderForge.SFN_DepthBlend,id:8909,x:30961,y:33837,varname:node_8909,prsc:2|DIST-3660-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3660,x:30748,y:33837,ptovrint:False,ptlb:Water Density,ptin:_WaterDensity,varname:node_3660,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:20;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:4810,x:31269,y:33932,varname:node_4810,prsc:2|IN-8909-OUT,IMIN-6724-OUT,IMAX-2713-OUT,OMIN-8267-OUT,OMAX-6283-OUT;n:type:ShaderForge.SFN_Vector1,id:8267,x:31269,y:34071,varname:node_8267,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:6283,x:31255,y:34158,varname:node_6283,prsc:2,v1:0;n:type:ShaderForge.SFN_Power,id:4971,x:31630,y:34079,varname:node_4971,prsc:2|VAL-5907-OUT,EXP-4511-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4511,x:31630,y:34250,ptovrint:False,ptlb:Fade Level,ptin:_FadeLevel,varname:node_4511,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_Fresnel,id:8690,x:31027,y:33475,varname:node_8690,prsc:2|EXP-6998-OUT;n:type:ShaderForge.SFN_Multiply,id:3678,x:31527,y:33415,varname:node_3678,prsc:2|A-8031-OUT,B-8690-OUT,C-9833-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6998,x:31010,y:33653,ptovrint:False,ptlb:Reflection Fresnel,ptin:_ReflectionFresnel,varname:node_6998,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Blend,id:6813,x:32821,y:33213,varname:node_6813,prsc:2,blmd:6,clmp:True|SRC-435-OUT,DST-3678-OUT;n:type:ShaderForge.SFN_Multiply,id:435,x:32543,y:33418,varname:node_435,prsc:2|A-4971-OUT,B-9391-RGB,C-7227-OUT;n:type:ShaderForge.SFN_SceneColor,id:9391,x:31868,y:34183,varname:node_9391,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:6123,x:32986,y:29388,varname:node_6123,prsc:2,tex:a0c55168c4eb26646a6bd97b7c8e4da0,ntxv:0,isnm:False|UVIN-8298-UVOUT,TEX-4105-TEX;n:type:ShaderForge.SFN_Tex2d,id:7755,x:32986,y:29570,varname:node_7755,prsc:2,tex:a0c55168c4eb26646a6bd97b7c8e4da0,ntxv:0,isnm:False|UVIN-7614-UVOUT,TEX-4105-TEX;n:type:ShaderForge.SFN_TexCoord,id:5078,x:31923,y:29144,varname:node_5078,prsc:2,uv:0;n:type:ShaderForge.SFN_ObjectScale,id:6427,x:31490,y:29291,varname:node_6427,prsc:2,rcp:False;n:type:ShaderForge.SFN_Append,id:5190,x:31703,y:29328,varname:node_5190,prsc:2|A-6427-X,B-6427-Z;n:type:ShaderForge.SFN_Multiply,id:5986,x:31923,y:29305,varname:node_5986,prsc:2|A-5078-UVOUT,B-5190-OUT,C-4695-OUT;n:type:ShaderForge.SFN_Slider,id:4695,x:31575,y:29529,ptovrint:False,ptlb:Waves Scale,ptin:_WavesScale,varname:node_4695,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.01,cur:0.8,max:1;n:type:ShaderForge.SFN_Panner,id:8298,x:32641,y:29301,varname:node_8298,prsc:2,spu:1,spv:-1|UVIN-5986-OUT,DIST-8229-OUT;n:type:ShaderForge.SFN_Panner,id:7614,x:32656,y:29463,varname:node_7614,prsc:2,spu:-1,spv:1|UVIN-9836-OUT,DIST-8229-OUT;n:type:ShaderForge.SFN_Slider,id:9092,x:31552,y:29695,ptovrint:False,ptlb:Waves Speed,ptin:_WavesSpeed,varname:node_9092,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Time,id:2375,x:31552,y:29784,varname:node_2375,prsc:2;n:type:ShaderForge.SFN_Multiply,id:8229,x:31938,y:29780,varname:node_8229,prsc:2|A-9092-OUT,B-2375-TSL,C-7114-OUT;n:type:ShaderForge.SFN_Phi,id:7114,x:31727,y:29872,varname:node_7114,prsc:2;n:type:ShaderForge.SFN_Add,id:5462,x:32377,y:29545,varname:node_5462,prsc:2|A-5986-OUT,B-5546-OUT;n:type:ShaderForge.SFN_Vector2,id:5546,x:32377,y:29433,varname:node_5546,prsc:2,v1:0.5,v2:0.5;n:type:ShaderForge.SFN_Multiply,id:9836,x:32557,y:29643,varname:node_9836,prsc:2|A-5462-OUT,B-7610-OUT;n:type:ShaderForge.SFN_Vector1,id:7610,x:32383,y:29724,varname:node_7610,prsc:2,v1:0.8;n:type:ShaderForge.SFN_Add,id:60,x:33348,y:29403,varname:node_60,prsc:2|A-6123-R,B-7755-R;n:type:ShaderForge.SFN_Add,id:6221,x:33348,y:29529,varname:node_6221,prsc:2|A-6123-G,B-7755-G;n:type:ShaderForge.SFN_Vector1,id:1671,x:33348,y:29680,varname:node_1671,prsc:2,v1:1;n:type:ShaderForge.SFN_Append,id:5203,x:34138,y:29614,varname:node_5203,prsc:2|A-60-OUT,B-6221-OUT,C-1671-OUT;n:type:ShaderForge.SFN_Slider,id:4005,x:33120,y:29781,ptovrint:False,ptlb:Normal Intensity,ptin:_NormalIntensity,varname:node_4005,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Lerp,id:6060,x:34441,y:29978,varname:node_6060,prsc:2|A-7317-OUT,B-7893-OUT,T-4005-OUT;n:type:ShaderForge.SFN_Vector3,id:7317,x:34568,y:29623,varname:node_7317,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_LightVector,id:2656,x:31212,y:32227,varname:node_2656,prsc:2;n:type:ShaderForge.SFN_Dot,id:3399,x:31447,y:32332,varname:node_3399,prsc:2,dt:1|A-2656-OUT,B-2213-OUT;n:type:ShaderForge.SFN_Power,id:1573,x:31753,y:32361,varname:node_1573,prsc:2|VAL-3399-OUT,EXP-7083-OUT;n:type:ShaderForge.SFN_Slider,id:3311,x:31229,y:32677,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:node_3311,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.6,max:1;n:type:ShaderForge.SFN_RemapRange,id:9351,x:31646,y:32683,varname:node_9351,prsc:2,frmn:0,frmx:1,tomn:1,tomx:10|IN-3311-OUT;n:type:ShaderForge.SFN_Exp,id:7083,x:31683,y:32514,varname:node_7083,prsc:2,et:0|IN-9351-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6388,x:31753,y:32255,ptovrint:False,ptlb:Specular,ptin:_Specular,varname:node_6388,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:5;n:type:ShaderForge.SFN_Multiply,id:3513,x:32643,y:32612,varname:node_3513,prsc:2|A-6388-OUT,B-1573-OUT,C-3702-OUT;n:type:ShaderForge.SFN_ViewReflectionVector,id:2213,x:31211,y:32375,varname:node_2213,prsc:2;n:type:ShaderForge.SFN_Color,id:9712,x:30634,y:34235,ptovrint:False,ptlb:Water Color,ptin:_WaterColor,varname:node_9712,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_RemapRange,id:2713,x:30945,y:34231,varname:node_2713,prsc:2,frmn:0,frmx:1,tomn:1,tomx:10|IN-9712-RGB;n:type:ShaderForge.SFN_Clamp01,id:5907,x:31578,y:33928,varname:node_5907,prsc:2|IN-4810-OUT;n:type:ShaderForge.SFN_Slider,id:9833,x:31449,y:33260,ptovrint:False,ptlb:Reflection Intensity,ptin:_ReflectionIntensity,varname:node_9833,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_SwitchProperty,id:8031,x:31184,y:32967,ptovrint:False,ptlb:Use Reflection,ptin:_UseReflection,varname:node_8031,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8643-RGB,B-7838-RGB;n:type:ShaderForge.SFN_Color,id:8643,x:30880,y:32782,ptovrint:False,ptlb:Reflection Color,ptin:_ReflectionColor,varname:node_8643,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ComponentMask,id:5866,x:35549,y:31652,varname:node_5866,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-6060-OUT;n:type:ShaderForge.SFN_Slider,id:8873,x:34981,y:32538,ptovrint:False,ptlb:Refraction Intensity,ptin:_RefractionIntensity,varname:node_8873,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_Multiply,id:7011,x:34969,y:32150,varname:node_7011,prsc:2|A-5866-OUT,B-8873-OUT,C-2788-OUT;n:type:ShaderForge.SFN_Vector1,id:2788,x:34336,y:32435,varname:node_2788,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Tex2dAsset,id:4105,x:32986,y:29167,ptovrint:False,ptlb:Normal Texture,ptin:_NormalTexture,varname:node_4105,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:a0c55168c4eb26646a6bd97b7c8e4da0,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:3810,x:33318,y:28868,varname:node_3810,prsc:2,tex:a0c55168c4eb26646a6bd97b7c8e4da0,ntxv:0,isnm:False|UVIN-7203-UVOUT,TEX-4105-TEX;n:type:ShaderForge.SFN_Tex2d,id:2963,x:33318,y:29040,varname:node_2963,prsc:2,tex:a0c55168c4eb26646a6bd97b7c8e4da0,ntxv:0,isnm:False|UVIN-117-UVOUT,TEX-4105-TEX;n:type:ShaderForge.SFN_Panner,id:7203,x:32881,y:28789,varname:node_7203,prsc:2,spu:-1,spv:1|UVIN-5671-OUT,DIST-5594-OUT;n:type:ShaderForge.SFN_Panner,id:117,x:32879,y:28926,varname:node_117,prsc:2,spu:1,spv:-1|UVIN-7528-OUT,DIST-5594-OUT;n:type:ShaderForge.SFN_Multiply,id:5671,x:32483,y:28695,varname:node_5671,prsc:2|A-6903-OUT,B-5986-OUT;n:type:ShaderForge.SFN_Multiply,id:5594,x:32498,y:28979,varname:node_5594,prsc:2|A-8229-OUT,B-523-OUT;n:type:ShaderForge.SFN_Vector1,id:6903,x:32138,y:28844,varname:node_6903,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Vector1,id:523,x:32283,y:29163,varname:node_523,prsc:2,v1:0.6;n:type:ShaderForge.SFN_Multiply,id:7528,x:32536,y:28826,varname:node_7528,prsc:2|A-6903-OUT,B-9836-OUT;n:type:ShaderForge.SFN_Add,id:3379,x:33672,y:28970,varname:node_3379,prsc:2|A-3810-R,B-2963-R;n:type:ShaderForge.SFN_Add,id:822,x:33672,y:29102,varname:node_822,prsc:2|A-3810-G,B-2963-G;n:type:ShaderForge.SFN_Vector1,id:3338,x:33657,y:29246,varname:node_3338,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:1410,x:33873,y:29201,varname:node_1410,prsc:2|A-6110-OUT,B-3338-OUT;n:type:ShaderForge.SFN_Append,id:6110,x:33873,y:29038,varname:node_6110,prsc:2|A-3379-OUT,B-822-OUT;n:type:ShaderForge.SFN_Append,id:5085,x:33873,y:29334,varname:node_5085,prsc:2|A-60-OUT,B-6221-OUT;n:type:ShaderForge.SFN_Add,id:3322,x:34074,y:29334,varname:node_3322,prsc:2|A-5085-OUT,B-1410-OUT;n:type:ShaderForge.SFN_Append,id:7893,x:34138,y:29494,varname:node_7893,prsc:2|A-3322-OUT,B-1671-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:7399,x:33201,y:31482,ptovrint:False,ptlb:Displacement,ptin:_Displacement,varname:node_7399,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:77631be9fde1502449409a785cd40831,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:2033,x:33478,y:31377,varname:node_2033,prsc:2,tex:77631be9fde1502449409a785cd40831,ntxv:0,isnm:False|UVIN-8500-UVOUT,TEX-7399-TEX;n:type:ShaderForge.SFN_Tex2d,id:7404,x:33478,y:31529,varname:node_7404,prsc:2,tex:77631be9fde1502449409a785cd40831,ntxv:0,isnm:False|UVIN-1716-UVOUT,TEX-7399-TEX;n:type:ShaderForge.SFN_Multiply,id:4305,x:34363,y:31456,varname:node_4305,prsc:2|A-5168-OUT,B-9420-OUT,C-1355-OUT;n:type:ShaderForge.SFN_Lerp,id:6532,x:33725,y:31464,varname:node_6532,prsc:2|A-2033-RGB,B-7404-RGB,T-3003-OUT;n:type:ShaderForge.SFN_Vector1,id:3003,x:33654,y:31622,varname:node_3003,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Vector3,id:9420,x:34268,y:31618,varname:node_9420,prsc:2,v1:0,v2:1,v3:0;n:type:ShaderForge.SFN_ValueProperty,id:1355,x:34268,y:31734,ptovrint:False,ptlb:Displacement Intensity,ptin:_DisplacementIntensity,varname:node_1355,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_TexCoord,id:2208,x:32704,y:31286,varname:node_2208,prsc:2,uv:0;n:type:ShaderForge.SFN_ObjectScale,id:1187,x:32605,y:31481,varname:node_1187,prsc:2,rcp:False;n:type:ShaderForge.SFN_Slider,id:4557,x:32589,y:31683,ptovrint:False,ptlb:Displacement Scale,ptin:_DisplacementScale,varname:node_4557,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.01,cur:0.5,max:1;n:type:ShaderForge.SFN_Append,id:7649,x:32783,y:31511,varname:node_7649,prsc:2|A-1187-X,B-1187-Z;n:type:ShaderForge.SFN_Multiply,id:3654,x:32989,y:31537,varname:node_3654,prsc:2|A-7649-OUT,B-4557-OUT,C-2208-UVOUT,D-8509-OUT;n:type:ShaderForge.SFN_Add,id:4923,x:32989,y:31740,varname:node_4923,prsc:2|A-3654-OUT,B-6540-OUT;n:type:ShaderForge.SFN_Vector2,id:6540,x:32798,y:31867,varname:node_6540,prsc:2,v1:0.5,v2:0.5;n:type:ShaderForge.SFN_Panner,id:8500,x:33266,y:31827,varname:node_8500,prsc:2,spu:-1,spv:1|UVIN-3654-OUT,DIST-9000-OUT;n:type:ShaderForge.SFN_Slider,id:7064,x:32590,y:32021,ptovrint:False,ptlb:Displacement Speed,ptin:_DisplacementSpeed,varname:node_7064,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.01,cur:0.5079523,max:10;n:type:ShaderForge.SFN_Time,id:9630,x:32710,y:32162,varname:node_9630,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9000,x:32977,y:32120,varname:node_9000,prsc:2|A-7064-OUT,B-9630-TSL;n:type:ShaderForge.SFN_Panner,id:1716,x:33270,y:32040,varname:node_1716,prsc:2,spu:1,spv:-1|UVIN-3549-OUT,DIST-9000-OUT;n:type:ShaderForge.SFN_Multiply,id:3549,x:33064,y:31926,varname:node_3549,prsc:2|A-4923-OUT,B-8485-OUT;n:type:ShaderForge.SFN_Vector1,id:8485,x:32910,y:32042,varname:node_8485,prsc:2,v1:0.75;n:type:ShaderForge.SFN_Vector1,id:8509,x:32614,y:31820,varname:node_8509,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Desaturate,id:7109,x:33930,y:31227,varname:node_7109,prsc:2|COL-6532-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:4134,x:32743,y:30773,ptovrint:False,ptlb:Foam Texture,ptin:_FoamTexture,varname:node_4134,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:837,x:32990,y:30643,varname:node_837,prsc:2,ntxv:0,isnm:False|UVIN-9810-UVOUT,TEX-4134-TEX;n:type:ShaderForge.SFN_Tex2d,id:2220,x:32998,y:30807,varname:node_2220,prsc:2,ntxv:0,isnm:False|UVIN-1263-UVOUT,TEX-4134-TEX;n:type:ShaderForge.SFN_Slider,id:3535,x:31956,y:30840,ptovrint:False,ptlb:Foam Scale,ptin:_FoamScale,varname:node_3535,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.01,cur:0.01,max:2;n:type:ShaderForge.SFN_ObjectScale,id:7286,x:31912,y:30601,varname:node_7286,prsc:2,rcp:False;n:type:ShaderForge.SFN_Append,id:1189,x:32148,y:30685,varname:node_1189,prsc:2|A-7286-X,B-7286-Z;n:type:ShaderForge.SFN_Multiply,id:2254,x:32440,y:30511,varname:node_2254,prsc:2|A-1189-OUT,B-1947-UVOUT,C-3535-OUT;n:type:ShaderForge.SFN_TexCoord,id:1947,x:32148,y:30529,varname:node_1947,prsc:2,uv:0;n:type:ShaderForge.SFN_Panner,id:9810,x:32815,y:30364,varname:node_9810,prsc:2,spu:-1,spv:1|UVIN-2254-OUT,DIST-827-OUT;n:type:ShaderForge.SFN_Panner,id:1263,x:32743,y:30588,varname:node_1263,prsc:2,spu:1,spv:-1|UVIN-3554-OUT,DIST-827-OUT;n:type:ShaderForge.SFN_Add,id:292,x:32395,y:30774,varname:node_292,prsc:2|A-2254-OUT,B-2242-OUT;n:type:ShaderForge.SFN_Vector2,id:2242,x:32301,y:30917,varname:node_2242,prsc:2,v1:0.5,v2:0.5;n:type:ShaderForge.SFN_Multiply,id:3554,x:32567,y:30923,varname:node_3554,prsc:2|A-292-OUT,B-7692-OUT;n:type:ShaderForge.SFN_Vector1,id:7692,x:32326,y:31031,varname:node_7692,prsc:2,v1:0.8;n:type:ShaderForge.SFN_Slider,id:4909,x:32323,y:30237,ptovrint:False,ptlb:Foam Speed,ptin:_FoamSpeed,varname:node_4909,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.01,cur:0.01,max:10;n:type:ShaderForge.SFN_Time,id:9204,x:32347,y:30351,varname:node_9204,prsc:2;n:type:ShaderForge.SFN_Multiply,id:827,x:32561,y:30318,varname:node_827,prsc:2|A-4909-OUT,B-9204-TSL;n:type:ShaderForge.SFN_Multiply,id:6257,x:33407,y:30801,varname:node_6257,prsc:2|A-837-RGB,B-2220-RGB,C-3702-OUT;n:type:ShaderForge.SFN_Multiply,id:8930,x:33950,y:31033,varname:node_8930,prsc:2|A-6257-OUT,B-7109-OUT,C-9305-OUT;n:type:ShaderForge.SFN_Relay,id:4856,x:33814,y:32243,cmnt:BASIC WATER,varname:node_4856,prsc:2|IN-6813-OUT;n:type:ShaderForge.SFN_LightAttenuation,id:3889,x:31441,y:31769,varname:node_3889,prsc:2;n:type:ShaderForge.SFN_LightColor,id:4892,x:31634,y:31375,varname:node_4892,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3702,x:31927,y:31596,varname:node_3702,prsc:2|A-2280-OUT,B-3889-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6100,x:33621,y:30456,ptovrint:False,ptlb:Shore Foam Distance,ptin:_ShoreFoamDistance,varname:node_6100,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_DepthBlend,id:739,x:33621,y:30537,varname:node_739,prsc:2|DIST-6100-OUT;n:type:ShaderForge.SFN_Multiply,id:3664,x:33918,y:30845,varname:node_3664,prsc:2|A-8395-OUT,B-6257-OUT,C-6400-OUT;n:type:ShaderForge.SFN_OneMinus,id:8395,x:33871,y:30441,varname:node_8395,prsc:2|IN-739-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6400,x:33814,y:30600,ptovrint:False,ptlb:Shore Foam Intensity,ptin:_ShoreFoamIntensity,varname:node_6400,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_DepthBlend,id:9087,x:34653,y:31554,varname:node_9087,prsc:2|DIST-2988-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2988,x:34653,y:31758,ptovrint:False,ptlb:Shore Line Opacity,ptin:_ShoreLineOpacity,varname:node_2988,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Tex2d,id:239,x:35679,y:29971,ptovrint:False,ptlb:Mask Waves Displacement,ptin:_MaskWavesDisplacement,varname:node_239,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:306f7b8087773cb42a665b9c5eeff8f1,ntxv:0,isnm:False|UVIN-3646-OUT;n:type:ShaderForge.SFN_Tex2d,id:6919,x:36380,y:30627,ptovrint:False,ptlb:Waves Texture,ptin:_WavesTexture,varname:node_6919,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:1494fa713d330b84b9ce081df7b5c0c0,ntxv:0,isnm:False|UVIN-2367-UVOUT;n:type:ShaderForge.SFN_ObjectScale,id:2430,x:36094,y:30192,varname:node_2430,prsc:2,rcp:False;n:type:ShaderForge.SFN_TexCoord,id:8571,x:35747,y:29166,varname:node_8571,prsc:2,uv:0;n:type:ShaderForge.SFN_Slider,id:6935,x:36137,y:30380,ptovrint:False,ptlb:Waves Amount,ptin:_WavesAmount,varname:node_6935,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:2188,x:36524,y:30194,varname:node_2188,prsc:2|A-7507-OUT,B-6311-OUT,C-6935-OUT,D-1169-OUT;n:type:ShaderForge.SFN_Append,id:6311,x:36334,y:30216,varname:node_6311,prsc:2|A-2430-X,B-2430-Z;n:type:ShaderForge.SFN_SwitchProperty,id:7507,x:36465,y:29678,ptovrint:False,ptlb:Radial Waves,ptin:_RadialWaves,varname:node_7507,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8571-UVOUT,B-9301-OUT;n:type:ShaderForge.SFN_RemapRange,id:9780,x:35732,y:29325,varname:node_9780,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-8571-UVOUT;n:type:ShaderForge.SFN_Length,id:175,x:35732,y:29483,varname:node_175,prsc:2|IN-9780-OUT;n:type:ShaderForge.SFN_OneMinus,id:4826,x:35743,y:29612,varname:node_4826,prsc:2|IN-175-OUT;n:type:ShaderForge.SFN_Append,id:3934,x:35743,y:29769,varname:node_3934,prsc:2|A-4826-OUT,B-4826-OUT;n:type:ShaderForge.SFN_Desaturate,id:7666,x:35861,y:29971,varname:node_7666,prsc:2|COL-239-RGB;n:type:ShaderForge.SFN_Multiply,id:7453,x:35979,y:29853,varname:node_7453,prsc:2|A-3934-OUT,B-7666-OUT;n:type:ShaderForge.SFN_TexCoord,id:9069,x:35340,y:29744,varname:node_9069,prsc:2,uv:0;n:type:ShaderForge.SFN_Vector1,id:1385,x:35340,y:29899,varname:node_1385,prsc:2,v1:-1;n:type:ShaderForge.SFN_Multiply,id:3646,x:35574,y:29836,varname:node_3646,prsc:2|A-9069-UVOUT,B-1385-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:9301,x:36063,y:29695,ptovrint:False,ptlb:Use Mask,ptin:_UseMask,varname:node_9301,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-3934-OUT,B-7453-OUT;n:type:ShaderForge.SFN_Vector1,id:1169,x:36524,y:30331,varname:node_1169,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Panner,id:2367,x:36939,y:30198,varname:node_2367,prsc:2,spu:1,spv:1|UVIN-2188-OUT,DIST-2370-OUT;n:type:ShaderForge.SFN_Slider,id:1612,x:36749,y:30557,ptovrint:False,ptlb:Waves Displacement Speed,ptin:_WavesDisplacementSpeed,varname:node_1612,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Time,id:3527,x:36832,y:30678,varname:node_3527,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2370,x:37098,y:30554,varname:node_2370,prsc:2|A-1612-OUT,B-3527-TSL,C-2576-OUT;n:type:ShaderForge.SFN_ToggleProperty,id:8221,x:37215,y:30317,ptovrint:False,ptlb:Inverse Direction,ptin:_InverseDirection,varname:node_8221,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False;n:type:ShaderForge.SFN_RemapRange,id:2576,x:37233,y:30424,varname:node_2576,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-8221-OUT;n:type:ShaderForge.SFN_Desaturate,id:8869,x:36380,y:30814,varname:node_8869,prsc:2|COL-6919-RGB;n:type:ShaderForge.SFN_Multiply,id:4642,x:36576,y:31068,varname:node_4642,prsc:2|A-8869-OUT,B-7658-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7658,x:36321,y:31153,ptovrint:False,ptlb:Waves Intensity,ptin:_WavesIntensity,varname:node_7658,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Add,id:5168,x:34159,y:31456,varname:node_5168,prsc:2|A-6532-OUT,B-1969-OUT;n:type:ShaderForge.SFN_Relay,id:1969,x:36306,y:31628,varname:node_1969,prsc:2|IN-4642-OUT;n:type:ShaderForge.SFN_Multiply,id:8941,x:34415,y:30447,varname:node_8941,prsc:2|A-8869-OUT,B-6257-OUT,C-9609-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9609,x:34398,y:30616,ptovrint:False,ptlb:Waves Displacement Foam Intensity,ptin:_WavesDisplacementFoamIntensity,varname:node_9609,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:9305,x:33671,y:31163,ptovrint:False,ptlb:Displacement Foam Intensity,ptin:_DisplacementFoamIntensity,varname:node_9305,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Add,id:8765,x:34199,y:30838,varname:node_8765,prsc:2|A-3664-OUT,B-8941-OUT;n:type:ShaderForge.SFN_Blend,id:7201,x:34605,y:31033,varname:node_7201,prsc:2,blmd:6,clmp:True|SRC-4856-OUT,DST-7909-OUT;n:type:ShaderForge.SFN_Add,id:7909,x:34236,y:30980,varname:node_7909,prsc:2|A-8765-OUT,B-8930-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:2280,x:31634,y:31543,varname:node_2280,prsc:2,min:0.01,max:1|IN-4892-RGB;n:type:ShaderForge.SFN_DepthBlend,id:1708,x:32168,y:34238,varname:node_1708,prsc:2|DIST-8619-OUT;n:type:ShaderForge.SFN_Multiply,id:8619,x:32178,y:34370,varname:node_8619,prsc:2|A-3660-OUT,B-3912-OUT;n:type:ShaderForge.SFN_Vector1,id:3912,x:32111,y:34559,varname:node_3912,prsc:2,v1:3.3;n:type:ShaderForge.SFN_OneMinus,id:3168,x:32357,y:34224,varname:node_3168,prsc:2|IN-1708-OUT;n:type:ShaderForge.SFN_RemapRange,id:7227,x:32589,y:34224,varname:node_7227,prsc:2,frmn:0,frmx:1,tomn:0.25,tomx:1|IN-3168-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6724,x:30947,y:34069,ptovrint:False,ptlb:Shore Water Opacity,ptin:_ShoreWaterOpacity,varname:node_6724,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.15;n:type:ShaderForge.SFN_Add,id:3434,x:34840,y:31209,varname:node_3434,prsc:2|A-930-OUT,B-3513-OUT;n:type:ShaderForge.SFN_Relay,id:8220,x:34170,y:31291,varname:node_8220,prsc:2|IN-3702-OUT;n:type:ShaderForge.SFN_Multiply,id:930,x:34605,y:31197,varname:node_930,prsc:2|A-7201-OUT,B-8220-OUT;proporder:141-8031-8643-6998-9833-8873-4105-4005-9712-3660-4511-6724-2988-4695-9092-6388-3311-7399-1355-4557-7064-4134-6100-3535-4909-6400-9305-9609-6919-239-7507-9301-8221-6935-1612-7658;pass:END;sub:END;*/

Shader "DCG/Water Shader/Water Surface OpenGL" {
    Properties {
        _ReflectionTex ("ReflectionTex", 2D) = "white" {}
        [MaterialToggle] _UseReflection ("Use Reflection", Float ) = 0.5
        _ReflectionColor ("Reflection Color", Color) = (0.5,0.5,0.5,1)
        _ReflectionFresnel ("Reflection Fresnel", Float ) = 1
        _ReflectionIntensity ("Reflection Intensity", Range(0, 1)) = 1
        _RefractionIntensity ("Refraction Intensity", Range(0, 2)) = 0
        _NormalTexture ("Normal Texture", 2D) = "bump" {}
        _NormalIntensity ("Normal Intensity", Range(0, 1)) = 1
        _WaterColor ("Water Color", Color) = (0.5,0.5,0.5,1)
        _WaterDensity ("Water Density", Float ) = 20
        _FadeLevel ("Fade Level", Float ) = 4
        _ShoreWaterOpacity ("Shore Water Opacity", Float ) = 0.15
        _ShoreLineOpacity ("Shore Line Opacity", Float ) = 1
        _WavesScale ("Waves Scale", Range(0.01, 1)) = 0.8
        _WavesSpeed ("Waves Speed", Range(0, 1)) = 0
        _Specular ("Specular", Float ) = 5
        _Gloss ("Gloss", Range(0, 1)) = 0.6
        _Displacement ("Displacement", 2D) = "white" {}
        _DisplacementIntensity ("Displacement Intensity", Float ) = 1
        _DisplacementScale ("Displacement Scale", Range(0.01, 1)) = 0.5
        _DisplacementSpeed ("Displacement Speed", Range(0.01, 10)) = 0.5079523
        _FoamTexture ("Foam Texture", 2D) = "white" {}
        _ShoreFoamDistance ("Shore Foam Distance", Float ) = 1
        _FoamScale ("Foam Scale", Range(0.01, 2)) = 0.01
        _FoamSpeed ("Foam Speed", Range(0.01, 10)) = 0.01
        _ShoreFoamIntensity ("Shore Foam Intensity", Float ) = 2
        _DisplacementFoamIntensity ("Displacement Foam Intensity", Float ) = 1
        _WavesDisplacementFoamIntensity ("Waves Displacement Foam Intensity", Float ) = 1
        _WavesTexture ("Waves Texture", 2D) = "white" {}
        _MaskWavesDisplacement ("Mask Waves Displacement", 2D) = "white" {}
        [MaterialToggle] _RadialWaves ("Radial Waves", Float ) = 0
        [MaterialToggle] _UseMask ("Use Mask", Float ) = -0.4142135
        [MaterialToggle] _InverseDirection ("Inverse Direction", Float ) = 0
        _WavesAmount ("Waves Amount", Range(0, 1)) = 0
        _WavesDisplacementSpeed ("Waves Displacement Speed", Range(0, 10)) = 1
        _WavesIntensity ("Waves Intensity", Float ) = 1
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
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma target 3.0
            #pragma glsl
            uniform sampler2D _GrabTexture;
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform sampler2D _ReflectionTex; uniform float4 _ReflectionTex_ST;
            uniform float _WaterDensity;
            uniform float _FadeLevel;
            uniform float _ReflectionFresnel;
            uniform float _WavesScale;
            uniform float _WavesSpeed;
            uniform float _NormalIntensity;
            uniform float _Gloss;
            uniform float _Specular;
            uniform float4 _WaterColor;
            uniform float _ReflectionIntensity;
            uniform fixed _UseReflection;
            uniform float4 _ReflectionColor;
            uniform float _RefractionIntensity;
            uniform sampler2D _NormalTexture; uniform float4 _NormalTexture_ST;
            uniform sampler2D _Displacement; uniform float4 _Displacement_ST;
            uniform float _DisplacementIntensity;
            uniform float _DisplacementScale;
            uniform float _DisplacementSpeed;
            uniform sampler2D _FoamTexture; uniform float4 _FoamTexture_ST;
            uniform float _FoamScale;
            uniform float _FoamSpeed;
            uniform float _ShoreFoamDistance;
            uniform float _ShoreFoamIntensity;
            uniform float _ShoreLineOpacity;
            uniform sampler2D _MaskWavesDisplacement; uniform float4 _MaskWavesDisplacement_ST;
            uniform sampler2D _WavesTexture; uniform float4 _WavesTexture_ST;
            uniform float _WavesAmount;
            uniform fixed _RadialWaves;
            uniform fixed _UseMask;
            uniform float _WavesDisplacementSpeed;
            uniform fixed _InverseDirection;
            uniform float _WavesIntensity;
            uniform float _WavesDisplacementFoamIntensity;
            uniform float _DisplacementFoamIntensity;
            uniform float _ShoreWaterOpacity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
                float4 projPos : TEXCOORD6;
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                float4 node_9630 = _Time + _TimeEditor;
                float node_9000 = (_DisplacementSpeed*node_9630.r);
                float2 node_3654 = (float2(objScale.r,objScale.b)*_DisplacementScale*o.uv0*0.1);
                float2 node_8500 = (node_3654+node_9000*float2(-1,1));
                float4 node_2033 = tex2Dlod(_Displacement,float4(TRANSFORM_TEX(node_8500, _Displacement),0.0,0));
                float2 node_1716 = (((node_3654+float2(0.5,0.5))*0.75)+node_9000*float2(1,-1));
                float4 node_7404 = tex2Dlod(_Displacement,float4(TRANSFORM_TEX(node_1716, _Displacement),0.0,0));
                float3 node_6532 = lerp(node_2033.rgb,node_7404.rgb,0.5);
                float4 node_3527 = _Time + _TimeEditor;
                float node_4826 = (1.0 - length((o.uv0*2.0+-1.0)));
                float2 node_3934 = float2(node_4826,node_4826);
                float2 node_3646 = (o.uv0*(-1.0));
                float4 _MaskWavesDisplacement_var = tex2Dlod(_MaskWavesDisplacement,float4(TRANSFORM_TEX(node_3646, _MaskWavesDisplacement),0.0,0));
                float2 node_2367 = ((lerp( o.uv0, lerp( node_3934, (node_3934*dot(_MaskWavesDisplacement_var.rgb,float3(0.3,0.59,0.11))), _UseMask ), _RadialWaves )*float2(objScale.r,objScale.b)*_WavesAmount*0.1)+(_WavesDisplacementSpeed*node_3527.r*(_InverseDirection*2.0+-1.0))*float2(1,1));
                float4 _WavesTexture_var = tex2Dlod(_WavesTexture,float4(TRANSFORM_TEX(node_2367, _WavesTexture),0.0,0));
                float node_8869 = dot(_WavesTexture_var.rgb,float3(0.3,0.59,0.11));
                v.vertex.xyz += ((node_6532+(node_8869*_WavesIntensity))*float3(0,1,0)*_DisplacementIntensity);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
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
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float4 node_2375 = _Time + _TimeEditor;
                float node_8229 = (_WavesSpeed*node_2375.r*1.61803398875);
                float2 node_5986 = (i.uv0*float2(objScale.r,objScale.b)*_WavesScale);
                float2 node_8298 = (node_5986+node_8229*float2(1,-1));
                float3 node_6123 = UnpackNormal(tex2D(_NormalTexture,TRANSFORM_TEX(node_8298, _NormalTexture)));
                float2 node_9836 = ((node_5986+float2(0.5,0.5))*0.8);
                float2 node_7614 = (node_9836+node_8229*float2(-1,1));
                float3 node_7755 = UnpackNormal(tex2D(_NormalTexture,TRANSFORM_TEX(node_7614, _NormalTexture)));
                float node_60 = (node_6123.r+node_7755.r);
                float node_6221 = (node_6123.g+node_7755.g);
                float node_5594 = (node_8229*0.6);
                float node_6903 = 0.1;
                float2 node_7203 = ((node_6903*node_5986)+node_5594*float2(-1,1));
                float3 node_3810 = UnpackNormal(tex2D(_NormalTexture,TRANSFORM_TEX(node_7203, _NormalTexture)));
                float2 node_117 = ((node_6903*node_9836)+node_5594*float2(1,-1));
                float3 node_2963 = UnpackNormal(tex2D(_NormalTexture,TRANSFORM_TEX(node_117, _NormalTexture)));
                float node_1671 = 1.0;
                float3 node_6060 = lerp(float3(0,0,1),float3((float2(node_60,node_6221)+(float2((node_3810.r+node_2963.r),(node_3810.g+node_2963.g))*0.5)),node_1671),_NormalIntensity);
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (node_6060.rg*_RefractionIntensity*0.1);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = node_6060;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = 1;
                float node_8267 = 1.0;
                float4 node_7838 = tex2D(_ReflectionTex,TRANSFORM_TEX(sceneUVs.rg, _ReflectionTex));
                float4 node_9204 = _Time + _TimeEditor;
                float node_827 = (_FoamSpeed*node_9204.r);
                float2 node_2254 = (float2(objScale.r,objScale.b)*i.uv0*_FoamScale);
                float2 node_9810 = (node_2254+node_827*float2(-1,1));
                float4 node_837 = tex2D(_FoamTexture,TRANSFORM_TEX(node_9810, _FoamTexture));
                float2 node_1263 = (((node_2254+float2(0.5,0.5))*0.8)+node_827*float2(1,-1));
                float4 node_2220 = tex2D(_FoamTexture,TRANSFORM_TEX(node_1263, _FoamTexture));
                float3 node_3702 = (clamp(_LightColor0.rgb,0.01,1)*attenuation);
                float3 node_6257 = (node_837.rgb*node_2220.rgb*node_3702);
                float4 node_3527 = _Time + _TimeEditor;
                float node_4826 = (1.0 - length((i.uv0*2.0+-1.0)));
                float2 node_3934 = float2(node_4826,node_4826);
                float2 node_3646 = (i.uv0*(-1.0));
                float4 _MaskWavesDisplacement_var = tex2D(_MaskWavesDisplacement,TRANSFORM_TEX(node_3646, _MaskWavesDisplacement));
                float2 node_2367 = ((lerp( i.uv0, lerp( node_3934, (node_3934*dot(_MaskWavesDisplacement_var.rgb,float3(0.3,0.59,0.11))), _UseMask ), _RadialWaves )*float2(objScale.r,objScale.b)*_WavesAmount*0.1)+(_WavesDisplacementSpeed*node_3527.r*(_InverseDirection*2.0+-1.0))*float2(1,1));
                float4 _WavesTexture_var = tex2D(_WavesTexture,TRANSFORM_TEX(node_2367, _WavesTexture));
                float node_8869 = dot(_WavesTexture_var.rgb,float3(0.3,0.59,0.11));
                float4 node_9630 = _Time + _TimeEditor;
                float node_9000 = (_DisplacementSpeed*node_9630.r);
                float2 node_3654 = (float2(objScale.r,objScale.b)*_DisplacementScale*i.uv0*0.1);
                float2 node_8500 = (node_3654+node_9000*float2(-1,1));
                float4 node_2033 = tex2D(_Displacement,TRANSFORM_TEX(node_8500, _Displacement));
                float2 node_1716 = (((node_3654+float2(0.5,0.5))*0.75)+node_9000*float2(1,-1));
                float4 node_7404 = tex2D(_Displacement,TRANSFORM_TEX(node_1716, _Displacement));
                float3 node_6532 = lerp(node_2033.rgb,node_7404.rgb,0.5);
                float3 finalColor = ((saturate((1.0-(1.0-saturate((1.0-(1.0-(pow(saturate((node_8267 + ( (saturate((sceneZ-partZ)/_WaterDensity) - _ShoreWaterOpacity) * (0.0 - node_8267) ) / ((_WaterColor.rgb*9.0+1.0) - _ShoreWaterOpacity))),_FadeLevel)*sceneColor.rgb*((1.0 - saturate((sceneZ-partZ)/(_WaterDensity*3.3)))*0.75+0.25)))*(1.0-(lerp( _ReflectionColor.rgb, node_7838.rgb, _UseReflection )*pow(1.0-max(0,dot(normalDirection, viewDirection)),_ReflectionFresnel)*_ReflectionIntensity)))))*(1.0-((((1.0 - saturate((sceneZ-partZ)/_ShoreFoamDistance))*node_6257*_ShoreFoamIntensity)+(node_8869*node_6257*_WavesDisplacementFoamIntensity))+(node_6257*dot(node_6532,float3(0.3,0.59,0.11))*_DisplacementFoamIntensity)))))*node_3702)+(_Specular*pow(max(0,dot(lightDirection,viewReflectDirection)),exp((_Gloss*9.0+1.0)))*node_3702));
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,saturate((sceneZ-partZ)/_ShoreLineOpacity)),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
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
            #include "Lighting.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma target 3.0
            #pragma glsl
            uniform sampler2D _GrabTexture;
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform sampler2D _ReflectionTex; uniform float4 _ReflectionTex_ST;
            uniform float _WaterDensity;
            uniform float _FadeLevel;
            uniform float _ReflectionFresnel;
            uniform float _WavesScale;
            uniform float _WavesSpeed;
            uniform float _NormalIntensity;
            uniform float _Gloss;
            uniform float _Specular;
            uniform float4 _WaterColor;
            uniform float _ReflectionIntensity;
            uniform fixed _UseReflection;
            uniform float4 _ReflectionColor;
            uniform float _RefractionIntensity;
            uniform sampler2D _NormalTexture; uniform float4 _NormalTexture_ST;
            uniform sampler2D _Displacement; uniform float4 _Displacement_ST;
            uniform float _DisplacementIntensity;
            uniform float _DisplacementScale;
            uniform float _DisplacementSpeed;
            uniform sampler2D _FoamTexture; uniform float4 _FoamTexture_ST;
            uniform float _FoamScale;
            uniform float _FoamSpeed;
            uniform float _ShoreFoamDistance;
            uniform float _ShoreFoamIntensity;
            uniform float _ShoreLineOpacity;
            uniform sampler2D _MaskWavesDisplacement; uniform float4 _MaskWavesDisplacement_ST;
            uniform sampler2D _WavesTexture; uniform float4 _WavesTexture_ST;
            uniform float _WavesAmount;
            uniform fixed _RadialWaves;
            uniform fixed _UseMask;
            uniform float _WavesDisplacementSpeed;
            uniform fixed _InverseDirection;
            uniform float _WavesIntensity;
            uniform float _WavesDisplacementFoamIntensity;
            uniform float _DisplacementFoamIntensity;
            uniform float _ShoreWaterOpacity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
                float4 projPos : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                float4 node_9630 = _Time + _TimeEditor;
                float node_9000 = (_DisplacementSpeed*node_9630.r);
                float2 node_3654 = (float2(objScale.r,objScale.b)*_DisplacementScale*o.uv0*0.1);
                float2 node_8500 = (node_3654+node_9000*float2(-1,1));
                float4 node_2033 = tex2Dlod(_Displacement,float4(TRANSFORM_TEX(node_8500, _Displacement),0.0,0));
                float2 node_1716 = (((node_3654+float2(0.5,0.5))*0.75)+node_9000*float2(1,-1));
                float4 node_7404 = tex2Dlod(_Displacement,float4(TRANSFORM_TEX(node_1716, _Displacement),0.0,0));
                float3 node_6532 = lerp(node_2033.rgb,node_7404.rgb,0.5);
                float4 node_3527 = _Time + _TimeEditor;
                float node_4826 = (1.0 - length((o.uv0*2.0+-1.0)));
                float2 node_3934 = float2(node_4826,node_4826);
                float2 node_3646 = (o.uv0*(-1.0));
                float4 _MaskWavesDisplacement_var = tex2Dlod(_MaskWavesDisplacement,float4(TRANSFORM_TEX(node_3646, _MaskWavesDisplacement),0.0,0));
                float2 node_2367 = ((lerp( o.uv0, lerp( node_3934, (node_3934*dot(_MaskWavesDisplacement_var.rgb,float3(0.3,0.59,0.11))), _UseMask ), _RadialWaves )*float2(objScale.r,objScale.b)*_WavesAmount*0.1)+(_WavesDisplacementSpeed*node_3527.r*(_InverseDirection*2.0+-1.0))*float2(1,1));
                float4 _WavesTexture_var = tex2Dlod(_WavesTexture,float4(TRANSFORM_TEX(node_2367, _WavesTexture),0.0,0));
                float node_8869 = dot(_WavesTexture_var.rgb,float3(0.3,0.59,0.11));
                v.vertex.xyz += ((node_6532+(node_8869*_WavesIntensity))*float3(0,1,0)*_DisplacementIntensity);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
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
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float4 node_2375 = _Time + _TimeEditor;
                float node_8229 = (_WavesSpeed*node_2375.r*1.61803398875);
                float2 node_5986 = (i.uv0*float2(objScale.r,objScale.b)*_WavesScale);
                float2 node_8298 = (node_5986+node_8229*float2(1,-1));
                float3 node_6123 = UnpackNormal(tex2D(_NormalTexture,TRANSFORM_TEX(node_8298, _NormalTexture)));
                float2 node_9836 = ((node_5986+float2(0.5,0.5))*0.8);
                float2 node_7614 = (node_9836+node_8229*float2(-1,1));
                float3 node_7755 = UnpackNormal(tex2D(_NormalTexture,TRANSFORM_TEX(node_7614, _NormalTexture)));
                float node_60 = (node_6123.r+node_7755.r);
                float node_6221 = (node_6123.g+node_7755.g);
                float node_5594 = (node_8229*0.6);
                float node_6903 = 0.1;
                float2 node_7203 = ((node_6903*node_5986)+node_5594*float2(-1,1));
                float3 node_3810 = UnpackNormal(tex2D(_NormalTexture,TRANSFORM_TEX(node_7203, _NormalTexture)));
                float2 node_117 = ((node_6903*node_9836)+node_5594*float2(1,-1));
                float3 node_2963 = UnpackNormal(tex2D(_NormalTexture,TRANSFORM_TEX(node_117, _NormalTexture)));
                float node_1671 = 1.0;
                float3 node_6060 = lerp(float3(0,0,1),float3((float2(node_60,node_6221)+(float2((node_3810.r+node_2963.r),(node_3810.g+node_2963.g))*0.5)),node_1671),_NormalIntensity);
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (node_6060.rg*_RefractionIntensity*0.1);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = node_6060;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float node_8267 = 1.0;
                float4 node_7838 = tex2D(_ReflectionTex,TRANSFORM_TEX(sceneUVs.rg, _ReflectionTex));
                float4 node_9204 = _Time + _TimeEditor;
                float node_827 = (_FoamSpeed*node_9204.r);
                float2 node_2254 = (float2(objScale.r,objScale.b)*i.uv0*_FoamScale);
                float2 node_9810 = (node_2254+node_827*float2(-1,1));
                float4 node_837 = tex2D(_FoamTexture,TRANSFORM_TEX(node_9810, _FoamTexture));
                float2 node_1263 = (((node_2254+float2(0.5,0.5))*0.8)+node_827*float2(1,-1));
                float4 node_2220 = tex2D(_FoamTexture,TRANSFORM_TEX(node_1263, _FoamTexture));
                float3 node_3702 = (clamp(_LightColor0.rgb,0.01,1)*attenuation);
                float3 node_6257 = (node_837.rgb*node_2220.rgb*node_3702);
                float4 node_3527 = _Time + _TimeEditor;
                float node_4826 = (1.0 - length((i.uv0*2.0+-1.0)));
                float2 node_3934 = float2(node_4826,node_4826);
                float2 node_3646 = (i.uv0*(-1.0));
                float4 _MaskWavesDisplacement_var = tex2D(_MaskWavesDisplacement,TRANSFORM_TEX(node_3646, _MaskWavesDisplacement));
                float2 node_2367 = ((lerp( i.uv0, lerp( node_3934, (node_3934*dot(_MaskWavesDisplacement_var.rgb,float3(0.3,0.59,0.11))), _UseMask ), _RadialWaves )*float2(objScale.r,objScale.b)*_WavesAmount*0.1)+(_WavesDisplacementSpeed*node_3527.r*(_InverseDirection*2.0+-1.0))*float2(1,1));
                float4 _WavesTexture_var = tex2D(_WavesTexture,TRANSFORM_TEX(node_2367, _WavesTexture));
                float node_8869 = dot(_WavesTexture_var.rgb,float3(0.3,0.59,0.11));
                float4 node_9630 = _Time + _TimeEditor;
                float node_9000 = (_DisplacementSpeed*node_9630.r);
                float2 node_3654 = (float2(objScale.r,objScale.b)*_DisplacementScale*i.uv0*0.1);
                float2 node_8500 = (node_3654+node_9000*float2(-1,1));
                float4 node_2033 = tex2D(_Displacement,TRANSFORM_TEX(node_8500, _Displacement));
                float2 node_1716 = (((node_3654+float2(0.5,0.5))*0.75)+node_9000*float2(1,-1));
                float4 node_7404 = tex2D(_Displacement,TRANSFORM_TEX(node_1716, _Displacement));
                float3 node_6532 = lerp(node_2033.rgb,node_7404.rgb,0.5);
                float3 finalColor = ((saturate((1.0-(1.0-saturate((1.0-(1.0-(pow(saturate((node_8267 + ( (saturate((sceneZ-partZ)/_WaterDensity) - _ShoreWaterOpacity) * (0.0 - node_8267) ) / ((_WaterColor.rgb*9.0+1.0) - _ShoreWaterOpacity))),_FadeLevel)*sceneColor.rgb*((1.0 - saturate((sceneZ-partZ)/(_WaterDensity*3.3)))*0.75+0.25)))*(1.0-(lerp( _ReflectionColor.rgb, node_7838.rgb, _UseReflection )*pow(1.0-max(0,dot(normalDirection, viewDirection)),_ReflectionFresnel)*_ReflectionIntensity)))))*(1.0-((((1.0 - saturate((sceneZ-partZ)/_ShoreFoamDistance))*node_6257*_ShoreFoamIntensity)+(node_8869*node_6257*_WavesDisplacementFoamIntensity))+(node_6257*dot(node_6532,float3(0.3,0.59,0.11))*_DisplacementFoamIntensity)))))*node_3702)+(_Specular*pow(max(0,dot(lightDirection,viewReflectDirection)),exp((_Gloss*9.0+1.0)))*node_3702));
                fixed4 finalRGBA = fixed4(finalColor * saturate((sceneZ-partZ)/_ShoreLineOpacity),0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma target 3.0
            #pragma glsl
            uniform float4 _TimeEditor;
            uniform sampler2D _Displacement; uniform float4 _Displacement_ST;
            uniform float _DisplacementIntensity;
            uniform float _DisplacementScale;
            uniform float _DisplacementSpeed;
            uniform sampler2D _MaskWavesDisplacement; uniform float4 _MaskWavesDisplacement_ST;
            uniform sampler2D _WavesTexture; uniform float4 _WavesTexture_ST;
            uniform float _WavesAmount;
            uniform fixed _RadialWaves;
            uniform fixed _UseMask;
            uniform float _WavesDisplacementSpeed;
            uniform fixed _InverseDirection;
            uniform float _WavesIntensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                float4 node_9630 = _Time + _TimeEditor;
                float node_9000 = (_DisplacementSpeed*node_9630.r);
                float2 node_3654 = (float2(objScale.r,objScale.b)*_DisplacementScale*o.uv0*0.1);
                float2 node_8500 = (node_3654+node_9000*float2(-1,1));
                float4 node_2033 = tex2Dlod(_Displacement,float4(TRANSFORM_TEX(node_8500, _Displacement),0.0,0));
                float2 node_1716 = (((node_3654+float2(0.5,0.5))*0.75)+node_9000*float2(1,-1));
                float4 node_7404 = tex2Dlod(_Displacement,float4(TRANSFORM_TEX(node_1716, _Displacement),0.0,0));
                float3 node_6532 = lerp(node_2033.rgb,node_7404.rgb,0.5);
                float4 node_3527 = _Time + _TimeEditor;
                float node_4826 = (1.0 - length((o.uv0*2.0+-1.0)));
                float2 node_3934 = float2(node_4826,node_4826);
                float2 node_3646 = (o.uv0*(-1.0));
                float4 _MaskWavesDisplacement_var = tex2Dlod(_MaskWavesDisplacement,float4(TRANSFORM_TEX(node_3646, _MaskWavesDisplacement),0.0,0));
                float2 node_2367 = ((lerp( o.uv0, lerp( node_3934, (node_3934*dot(_MaskWavesDisplacement_var.rgb,float3(0.3,0.59,0.11))), _UseMask ), _RadialWaves )*float2(objScale.r,objScale.b)*_WavesAmount*0.1)+(_WavesDisplacementSpeed*node_3527.r*(_InverseDirection*2.0+-1.0))*float2(1,1));
                float4 _WavesTexture_var = tex2Dlod(_WavesTexture,float4(TRANSFORM_TEX(node_2367, _WavesTexture),0.0,0));
                float node_8869 = dot(_WavesTexture_var.rgb,float3(0.3,0.59,0.11));
                v.vertex.xyz += ((node_6532+(node_8869*_WavesIntensity))*float3(0,1,0)*_DisplacementIntensity);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
/////// Vectors:
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
