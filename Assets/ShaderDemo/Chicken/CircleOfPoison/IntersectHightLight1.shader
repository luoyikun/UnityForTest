Shader "Unlit/IntersectHightLight"
{
    Properties
    {
		_MainTex ("Base (RGB)", 2D) = "white" {}//电弧
	    _MaskTex("MaskTexture", 2D) = "white" {}//电弧遮罩
	    _TurnTex("MaskTexture", 2D) = "white" {}//切换电弧
        _Color ("Color",Color) = (0,0,0,0)//毒圈颜色
        _IntersectWidth("Intersect Width",Range(0,5)) = 0.05 //相交宽度 Zintersect
        _IntersectColor("Intersect Color",Color) = (0,0,0,0) //交界颜色
		_Constrast("Constrast",Range(1,5))=3
		_lightNess("lightNess",Range(1,100))=50
		_viewAngle("ViewAngle",Range(1,500))=1
		_Mul("_Mul",Range(0,500))=1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
        LOD 100
		
        Pass
        {
            //为了方便观察相交情况，设置半透，双面渲染
            ZWrite Off
            Cull off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _MaskTex;
			sampler2D _TurnTex;
			fixed4 _MainTex_ST ;
		    fixed4 _MaskTex_ST ;
		    fixed4 _TurnTex_ST ;
			fixed _Constrast;
			fixed _lightNess;
			fixed _viewAngle;//来自unity脚本的视野放大倍数，倍数越大越近，越能看见电弧
			fixed _Mul;
            struct appdata
            {
                fixed4 vertex : POSITION;
				fixed4 texcoordMain : TEXCOORD0;
				fixed4 texcoordMask : TEXCOORD1;
				fixed4 texcoordTurn : TEXCOORD2;

            };

            struct v2f
            {
                fixed2 Z0 : TEXCOORD0;
                fixed4 scrPos : TEXCOORD1;
                fixed4 vertex : SV_POSITION;
				fixed4 uvMain : TEXCOORD3;
				fixed4 uvMask : TEXCOORD4;
				fixed2 uvTurn : TEXCOORD5;
				fixed2 uvTurnFast : TEXCOORD6;
				 fixed4 vert2world : TEXCOORD7;
				//fixed uvMain_y:TEXCOORD8;
            };

            fixed4 _Color;
            //获得深度图，注意摄像机需要设置 Camera.main.depthTextureMode = DepthTextureMode.Depth;
            
            sampler2D _CameraDepthTexture;
            fixed _IntersectWidth;
            fixed4 _IntersectColor;
           
            v2f vert (appdata v)
            {
                v2f o;
				//以下是边缘交界高亮
                o.vertex = UnityObjectToClipPos(v.vertex);
                COMPUTE_EYEDEPTH(o.Z0.x);// 在相机空间把顶点的深度存入ZO
                o.scrPos = ComputeScreenPos(o.vertex);//先把顶点在屏幕空间的位置存起来，用于片元着色器中获取深度图的深度

				o.vert2world=mul(unity_ObjectToWorld ,v.vertex);
				o.Z0.y=v.texcoordMain.y;
				//以下是电弧和频率
				_MainTex_ST.x=_Mul;//由于毒圈高度固定，所以x方向UV要随毒圈大小改变，以免电弧走形
				o.uvMain.xy = v.texcoordMain.xy * _MainTex_ST.xy + _MainTex_ST.zw;//电弧UV(空间)
				_MainTex_ST.w=-_Time.x*2;
				o.uvMain.zw = v.texcoordMain.xy * _MainTex_ST.xy*16 + _MainTex_ST.zw;//电弧UV（交界处）
				_MaskTex_ST.x=_Mul;
				o.uvMask.xy = v.texcoordMask.xy * _MaskTex_ST.xy + _MaskTex_ST.zw ;//遮罩1 UV
				_MaskTex_ST.zw=fixed2(666,666);
				o.uvMask.zw = v.texcoordMask.xy * _MaskTex_ST.xy + _MaskTex_ST.zw ;//遮罩2 UV
				_TurnTex_ST.zw += _Time.x*3;
				o.uvTurn = fixed2(0.1,0.1) * _TurnTex_ST.xy + _TurnTex_ST.zw;//用固定点按直线扫描(慢)
				_TurnTex_ST.z += _Time.w;
				o.uvTurnFast= fixed2(0.4,0.4) * _TurnTex_ST.xy + _TurnTex_ST.zw;//用固定点按直线扫描(快)
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
				//以下是电弧频率
				fixed4 albedo1 = fixed4(tex2D(_MainTex, i.uvMain.xy).rgb,0);//电弧采样
				fixed4 albedo5 =fixed4(tex2D(_MainTex, i.uvMain.zw).rgb,0);//交界处电弧采样
				fixed4 albedo2 = fixed4(tex2D(_MaskTex, i.uvMask.xy).rgb,0);//加上遮罩1
				fixed4 albedo4= fixed4(tex2D(_MaskTex, i.uvMask.zw).rgb,0);//加上遮罩2
				fixed4 albedo3 = tex2D(_MaskTex, i.uvTurnFast);//获得快闪动
				fixed3 turnRGB=step(fixed3(0.3,0.3,0.3), tex2D(_TurnTex, i.uvTurn).rgb);//根据采集rgb控制3种闪电交替闪烁


				fixed4 albedoA=pow( albedo1*albedo2*(albedo3.r+2),_Constrast)*_lightNess*15;//电弧1亮度对比度
				fixed4 albedoB=pow( albedo1*albedo4*(albedo3.g+2),_Constrast)*_lightNess*15;//电弧2亮度对比度

				fixed3 DistDir=i.vert2world-_WorldSpaceCameraPos;//受倍镜影响
				fixed sqDist= dot(DistDir,DistDir);

				                //以下是边缘交界高亮
                fixed ZdepthTexture = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD(i.scrPos)));
                fixed Zdiff = saturate(_IntersectWidth - abs(ZdepthTexture - i.Z0.x));//用模型的改片元的深度值 和 深度图的深度值 取差
				fixed4 junction=(albedo3+0.7).b*albedo5*Zdiff*5; //交界高亮
				
               float4 OutColor= _Color+junction+(albedoA*turnRGB.r+albedoB*turnRGB.g)*_viewAngle*_viewAngle*_IntersectColor/sqDist; //
			  // OutColor.a=(1-i.Z0.y)*(1-i.Z0.y)*(1-i.Z0.y)*(1-i.Z0.y)*0.4;
			   OutColor.a=pow(1-i.Z0.y,6)*0.4;
			   return OutColor;
            }
            ENDCG
        }
    }
}