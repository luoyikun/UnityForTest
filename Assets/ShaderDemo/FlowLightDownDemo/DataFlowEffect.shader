//鍔熻兘闇€姹傦細妯℃嫙鏁版嵁浼犻€佹晥鏋滐紝楂樹寒鑹插潡浠庢ā鍨嬩笂鏂圭Щ鍔ㄥ埌涓嬫柟
//鍔熻兘鍒嗘瀽锛氳繖閲岄噰鐢║V鍔ㄧ敾鐨勬柟寮忔潵瀹炵幇锛屽埄鐢ˋlpha璐村浘鑾峰彇娴佸姩鐨勫舰鐘?
//			鍒╃敤Alpha閬僵璐村浘锛屾彁鍑轰笉闇€瑕佹樉绀烘祦鍔ㄧ殑鍦版柟

Shader "Custom/DataFlowEffect"
{
	Properties
	{
		_MainColor("Main Color",Color) = (1,1,1,1)
		_MainTex("Main Texture",2D) = "white"{}
		_Specular("Specular",Color) = (1,1,1,1)
		_Gloss("Gloss",Range(0,255)) = 20.0
		_FlowTex("Flow Tex (A)",2D) = "black"{}
		_FlowColor("Flow Color (RGBA)",Color)=(1,1,1,1)
		_FlowIdleTime("FlowInternal",Range(0,10))=1.0
		_FlowDuring("FlowDuring",Range(0,10))=1.0
		_FlowMaskTex("FlowMasking (A)",2D)="white"{}
		_FlowDirection("FlowDirection",Int)= 0
		_FlowBeginTime("Flow Begin Time",Float)=0
	}

	SubShader
	{
		Tags{"RenderType" = "Opaque" "Queue"="Geometry"}
		
		Pass
		{
			Tags{"LightMode"="ForwardBase"}
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			sampler2D _MainTex;		//棰滆壊璐村浘
			half4 _MainTex_ST;		//棰滆壊UV 缂╂斁鍜屽亸绉?
			fixed3 _MainColor;		//婕弽灏勯鑹?
			fixed3 _Specular;		//楂樺厜棰滆壊
			fixed _Gloss;			//楂樺厜搴?
			sampler2D _FlowTex;		//鏁版嵁娴佸浘鐗?
			fixed4 _FlowColor;		//鏁版嵁娴侀鑹插彔鍔?
			half4 _FlowTex_ST;		//鏁版嵁娴佽创鍥綰V鐨勭缉鏀惧拰鍋忕Щ
			fixed _FlowIdleTime;	//娴佸姩鍔ㄧ敾闂存瓏鏃堕棿
			fixed _FlowDuring;		//娴佸姩鍔ㄧ敾鎾斁鏃堕棿
			sampler2D _FlowMaskTex;	//娴佸姩閬僵
			fixed _FlowDirection;	//娴佸姩鏂瑰悜
			float _FlowBeginTime;	//娴佸姩鏁堟灉寮€濮嬬殑鏃堕棿

			struct a2v
			{
				half4 pos: POSITION;
				half3 normal :NORMAL;
				half4 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				half4 position : SV_POSITION;
				half2 uv : TEXCOORD0;
				half3 worldNormal : TEXCOORD1;
				half3 worldPos : TEXCOORD2;
				half2 flowUV : TEXCOORD3;
			};

			v2f vert(a2v i)
			{
				v2f v;
				v.position = UnityObjectToClipPos(i.pos);
				v.uv = i.texcoord * _MainTex_ST.xy + _MainTex_ST.zw; //
				v.worldNormal = mul(unity_ObjectToWorld,i.normal);
				v.worldPos = mul(unity_ObjectToWorld,i.pos);
				v.flowUV = i.texcoord * _FlowTex_ST.xy + _FlowTex_ST.zw;
				return v;
			}

			//uv - vert鐨剈v鍧愭爣
			//scale - 璐村浘缂╂斁
			//idleTime - 姣忔寰幆寮€濮嬪悗澶氶暱鏃堕棿锛屽紑濮嬫祦鍔?
			//loopTime - 鍗曟娴佸姩鏃堕棿
			fixed4 getFlowColor(half2 uv,int scale,fixed idleTime,fixed loopTime)
			{
				//褰撳墠杩愯鏃堕棿
				half flowTime_ = _Time.y - _FlowBeginTime;

				//涓婁竴娆″惊鐜紑濮嬶紝鍒版湰娆″惊鐜紑濮嬬殑鏃堕棿闂撮殧
				half internal = idleTime + loopTime;
				
				//褰撳墠寰幆鎵ц鏃堕棿
				half curLoopTime = fmod(flowTime_,internal);

				//姣忔寮€濮嬫祦鍔ㄤ箣鍓嶏紝鏈変釜鍋滄闂撮殧锛屾娴嬫槸鍚﹀彲浠ユ祦鍔ㄤ簡
				if(curLoopTime > idleTime)
				{
					//宸茬粡娴佸姩鏃堕棿
					half actionTime = curLoopTime - idleTime;

					//娴佸姩杩涘害鐧惧垎姣?
					half actionPercentage = actionTime / loopTime;

					half length = 1.0 / scale;

					//浠庝笅寰€涓婃祦鍔?
					//璁＄畻鏂瑰紡锛氳锛歽 = ax + b锛屽叾涓瓂涓轰笅杈圭晫鍊硷紝x涓烘祦鍔ㄨ繘搴?
					//鏍规嵁鎴戜滑瑕佹眰鍙互锛寈=0鏃秠=-length锛泋=1鏃秠=1锛涘甫鍏ヨВ鏂圭▼
					half bottomBorder = actionPercentage * (1+length) - length;
					half topBorder = bottomBorder + length;

					//浠庝笂寰€涓嬫祦鍔?
					//姹傝В鏂规硶涓庝笂闈㈢被浼?
					if(_FlowDirection < 0)
					{
						topBorder = (-1-length) * actionPercentage + 1 + length;
						bottomBorder = topBorder - length;
					}

					if(uv.y < topBorder && uv.y > bottomBorder)
					{
						half y = (uv.y - bottomBorder) / length;
						return tex2D(_FlowTex,fixed2(uv.x,y)); 
					}
				}

				return fixed4(1,1,1,0);
			}

			fixed4 frag(v2f v):SV_Target
			{
				//璁＄畻婕弽灏勭郴鏁?
				fixed3 albedo = tex2D(_MainTex,v.uv) * _MainColor;
				
				//璁＄畻鐜鍏?
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				

				fixed3 worldNormal = normalize(v.worldNormal);								//涓栫晫鍧愭爣鐨勬硶绾挎柟鍚?
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(v.worldPos));		//涓栫晫鍧愭爣鐨勫厜鐓ф柟鍚?
				fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(v.worldPos));		//涓栫晫鍧愭爣鐨勮瑙掓柟鍚?

				//璁＄畻婕弽灏勯鑹?閲囩敤Half-Lambert妯″瀷
				fixed3 lightColor = _LightColor0.rgb;
				fixed3 diffuse = lightColor * albedo * max(0,0.5*dot(worldNormal,worldLightDir)+0.5);

				//璁＄畻楂樺厜,閲囩敤Blinn-Phone楂樺厜妯″瀷
				fixed3 halfDir = normalize(worldViewDir + worldLightDir);
				fixed3 specColor = _Specular * lightColor * pow(max(0,dot(worldNormal,halfDir)),_Gloss);

				//鍙犲姞娴佸姩璐村浘				
				fixed4 flowColor = getFlowColor(v.uv,_FlowTex_ST.y,_FlowIdleTime,_FlowDuring);	
				fixed4 flowMaskColor = tex2D(_FlowMaskTex,v.uv);

				//涓庨伄缃╄创鍥捐繘琛屾贩鍚堬紝鍙樉绀洪伄缃╄创鍥句笉閫忔槑鐨勯儴鍒?
				flowColor.a = flowMaskColor.a * flowColor.a * _FlowColor.a;

				fixed3 finalDiffuse = lerp(diffuse,_FlowColor,flowColor.a);

				return fixed4(ambient + finalDiffuse+specColor,1);
			}

			ENDCG
		}
	}
	Fallback "Diffuse"
}