// Shader 的路径名称  默认为文件名,也可以与文件名不同
Shader "Unlit/HiShader"
{
	// 属性 
	// Material Inspector显示的所有参数都在需要在这里进行声明
	Properties
	{
		// 通常所有属性名都以下划线字符开头 _MainTex
		_MainTex("Texture", 2D) = "white" {}

	// 比较常见的属性类型
	// ————————————————————————————————————————————————
	_Integer("整数(新版)", Integer) = 1
	_Int("整数(旧版)", Int) = 1
	_Float("浮点数", Float) = 0.5
	_FloatRange("浮点数滑动条", Range(0.0, 1.0)) = 0.5
		// Unity包含以下内置纹理, 可以直接填充
		// “white”（RGBA：1,1,1,1）
		// “black”（RGBA：0,0,0,1）
		// “gray”（RGBA：0.5,0.5,0.5,1）
		// “bump”（RGBA：0.5,0.5,1,0.5）
		// “red”（RGBA：1,0,0,1）
		_Texture2D("2D纹理贴图", 2D) = "red" {}
	// 字符串留空或输入无效值，则它默认为 “gray”
	_DefaultTexture2D("2D纹理贴图", 2D) = "" {}
	// 默认值为 “gray”（RGBA：0.5,0.5,0.5,1）
	_Texture3D("3D纹理贴图", 3D) = "" {}
	_Cubemap("立方体贴图", Cube) = "" {}
	// Inspector会显示四个单独的浮点数字段
	_Vector("Example vector", Vector) = (0.25, 0.5, 0.5, 1)
		// Inspector会显示拾色器拾取色彩RGBA值
		_Color("色彩", Color) = (0.25, 0.5, 0.5, 1)
		// ————————————————————————————————————————————————

		// 除此之外 属性声明还可以具有一个可选特性 用来告知Unity如何处理它们
		// HDR可以使色彩亮度的值超过1
		[HDR]_HDRColor("HDR色彩", Color) = (1,1,1,1)
		// Inspector隐藏此属性
		[HideInInspector]_Hide("看不见我~", Color) = (1,1,1,1)
		// Inspector隐藏此纹理属性的Scale Offset字段
		[NoScaleOffset]_HideScaleOffset("隐藏ScaleOffset", 2D) = "" {}
	// 指示纹理属性为法线贴图，如果分配了不兼容的纹理，编辑器则会显示警告。
	[Normal]_Normal("法线贴图", 2D) = "" {}
	}

		// 子着色器 
		// 一个Shader至少有一个或者多个子着色器SubShader，这些子着色器互不干扰，且只有一个会运行
		// 在加载shader时Unity会遍历所有SubShader列表，并最终选择用户机器支持的第一个
		SubShader
	{
		// 可以通过Tags来向子着色器分配标签
		// 只可以写在SubShader语块内,不可写在Pass内
		/* 以键值对的形式存在,可以出现多个键值对
		Tags {
			"TagName1" = "Value1"
			"TagName2" = "Value2"
			"TagName3" = "Value3"
			...
			}
		*/

		// RenderPipeline: 声明子着色器是否与通用渲染管线 (URP) 或高清渲染管线 (HDRP) 兼容
		// 仅与 URP 兼容
		// Tags { "RenderPipeline"="UniversalRenderPipeline" }
		// 仅与 HDRP 兼容
		// Tags { "RenderPipeline"="HighDefinitionRenderPipeline" }
		// RenderPipeline不声明或任何其他值表示与 URP 和 HDRP 不兼容
		// ————————————————————————————————————————————————

		// Queue: 声明渲染队列
		// Tags { "Queue"="Background" } // 最早被调用的渲染，用来渲染天空盒或者背景
		// Tags { "Queue"="Geometry" }   // 这是默认值，用来渲染非透明物体（普通情况下，场景中的绝大多数物体应该是非透明的）
		// Tags { "Queue"="AlphaTest" }  // 用来渲染经过Alpha Test的像素，单独为AlphaTest设定一个Queue是出于对效率的考虑
		// Tags { "Queue"="Transparent" }// 以从后往前的顺序渲染透明物体
		// Tags { "Queue"="Overlay" }    // 用来渲染叠加的效果，是渲染的最后阶段（比如镜头光晕等特效）
		// ————————————————————————————————————————————————

		// RenderType: 用来区别这个Shader要渲染的对象是属于什么类别的。
		// 设置渲染类型 用一种称为着色器替换的技术在运行时交换子着色器,用来区别这个Shader要渲染的对象是属于什么类别的
		// 这里表示非透明物体渲染
		Tags { "RenderType" = "Opaque" }
		// 更多详细内容可参考官网文档 https://docs.unity.cn/cn/2021.3/Manual/SL-SubShaderTags.html

		// LOD (Level of Detail)
		LOD 100

		// 每个子着色器由多个通道组成，许多简单的着色器只使用一个通道，但想要一些更复杂的效果，着色器可能需要更多通道
		// 一个Pass就是一次绘制，可以看成是一个Draw Call而Pass的意义在于多次渲染，
		// 如果你有一个Pass，那么着色器只会被调用一次，如果你有多个Pass的话，
		// 那么就相当于执行多次SubShader了，这就叫双通道或者多通道。

		// Draw Call：其实就是CPU调用图像编程接口的渲染命令，CPU每次调用DrawCall，都需要向GPU发送许多数据啊、渲染状态等等，
		// 一旦CPU执行完应用阶段，GPU就会开始执行这次的渲染流程。而GPU渲染的速度比CPU提交命令的速度要快的多，
		// 所以如果DrawCall数量过多的情况下，CPU需要进行大量的计算，进而就会导致CPU过载，影响游戏的运行效率。
		Pass
		{
			CGPROGRAM
			// 声明顶点着色器
			#pragma vertex vert
			// 声明像素着色器
			#pragma fragment frag
			// 使雾生效
			#pragma multi_compile_fog

			// 引用CG的核心代码库
			#include "UnityCG.cginc"

			// 应用程序阶段结构体
			struct appdata
			{
		// 参考：https://docs.microsoft.com/zh-cn/windows/win32/direct3dhlsl/dx-graphics-hlsl-semantics
		// POSITION 着色器语言的语义，用来限定着色器的输入输出值的类型
		// 模型空间的顶点坐标
		float4 vertex : POSITION;
		// 模型的第一套UV坐标
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		// UV
		float2 uv : TEXCOORD0;
		UNITY_FOG_COORDS(1)
			// SV_POSITION 当这个值需要作为输出值输出给系统用的时候 前面需要加SV_前缀
			// 当然因为有向下兼容的机制 不加也没啥太大问题
			float4 vertex : SV_POSITION;
		};

	// 在Properties中声明的参数要在这里相对应的定义后才可以使用
	sampler2D _MainTex;
	float4 _MainTex_ST;

	// 定义顶点着色器函数 函数名要与声明顶点着色器名称相同
	v2f vert(appdata v)
	{
		v2f o;
		// 将顶点坐标从模型空间变换到裁剪空间
		o.vertex = UnityObjectToClipPos(v.vertex);
		// Transforms 2D UV by scale/bias property
		// #define TRANSFORM_TEX(tex,name) (tex.xy * name##_ST.xy + name##_ST.zw)
		// 等价于v.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
		// 简单来说，TRANSFORM_TEX主要作用是拿顶点的uv去和材质球的tiling和offset作运算，
		// 确保材质球里的缩放和偏移设置是正确的
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		UNITY_TRANSFER_FOG(o,o.vertex);
		return o;
	}

	// SV_Target可以视为COLOR ，虽说他也是作为输出值输出给系统的
	// 但它其实是告诉系统把输出的颜色值存储到RenderTarget中
	// 所以这里我们用SV_Target
	fixed4 frag(v2f i) : SV_Target
	{
		// 采样2D纹理贴图
		fixed4 col = tex2D(_MainTex, i.uv);
	// 应用雾
	UNITY_APPLY_FOG(i.fogCoord, col);
	// 返回经过处理后的最终色彩
	return col;
}
ENDCG
}
	}
}