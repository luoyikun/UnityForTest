Shader "Hidden/DCG/Water Shader/Water Fog"
{
    CGINCLUDE  
    #include "UnityCG.cginc" 
    sampler2D _CameraDepthTexture;
	#define PI 3.1416
    float4 _FogColor, _SunLightColor, _BoxMin, _BoxMax, _LightColor, VolumeSize;
 
    float _sunIntensity = 1.2, 
    sunRadius, 
    _FogDensity, 
    sunDistance = 100, 
	sunDistanceFactor = 500,
	_Ray, gain=1, 
	threshold=0, 
	Exposure;

   float3 LightTransform = float3(0, 0, 1);

 float hit (float3 start, float3 dir, float3 M1, float3 M2, inout float hitMin, inout float hitMax)
 {
    float yMin, yMax, zMin, zMax;
    float flag = 1.0;
        
    if (dir.x > 0) 
    {
       hitMin = (M1.x - start.x) / dir.x;
       hitMax = (M2.x - start.x) / dir.x;
    }
    else 
    {
       hitMin = (M2.x - start.x) / dir.x;
       hitMax = (M1.x - start.x) / dir.x;
    }
    if (dir.y > 0) 
    {
       yMin = (M1.y - start.y) / dir.y;
       yMax = (M2.y - start.y) / dir.y;
    }
    else 
    {
       yMin = (M2.y - start.y) / dir.y;
       yMax = (M1.y - start.y) / dir.y;
    }
    if ((hitMin > yMax) || (yMin > hitMax)) flag = -1.0;
        if (yMin > hitMin) hitMin = yMin;
        if (yMax < hitMax) hitMax = yMax;
        if (dir.z > 0) 
    {
        zMin = (M1.z - start.z) / dir.z;
        zMax = (M2.z - start.z) / dir.z;
    }
    else 
    {
        zMin = (M2.z - start.z) / dir.z;
    	zMax = (M1.z - start.z) / dir.z;
    }
    if ((hitMin > zMax) || (zMin > hitMax)) flag = -1.0;
        if (zMin > hitMin) hitMin = zMin;
        if (zMax < hitMax) hitMax = zMax;
        return (flag > 0.0);
    }
	float He(float3 E, float3 L, float mieDirectionalG)
	{
        float theta=saturate(dot(E, L));
        return (1.0 / (4.0 * PI)) * ((1.0 - mieDirectionalG * mieDirectionalG) / pow(1.0 - 2.0 * mieDirectionalG * theta + mieDirectionalG * mieDirectionalG, 1.5)) ;
    }
    struct v2f
    {
        float4 SampleCoordinates         : SV_POSITION;
        float3 Wpos        : TEXCOORD0;
        float4 ScreenUVs   : TEXCOORD1;
        float3 LocalPos    : TEXCOORD2;
        float3 ViewPos     : TEXCOORD3;
        float3 LocalEyePos : TEXCOORD4;
        float3 LightLocalDir : TEXCOORD5;
    };
    
    half Threshold(float a, float Gain, float Contrast)
	{
        float Guy = a * Gain;
        float thresh =  Guy - Contrast;
        return saturate(lerp(0.0f ,Guy , thresh ));
    }
    
    v2f vert (appdata_full i)
    {
        v2f o;
        o.SampleCoordinates = UnityObjectToClipPos(i.vertex);
        o.Wpos = mul((float4x4)unity_ObjectToWorld, float4(i.vertex.xyz, 1)).xyz;
        o.ScreenUVs = ComputeScreenPos(o.SampleCoordinates);
        o.ViewPos = mul((float4x4)UNITY_MATRIX_MV, float4(i.vertex.xyz, 1)).xyz;
        o.LocalPos = i.vertex.xyz;
        o.LocalEyePos = mul((float4x4)unity_WorldToObject, (float4(_WorldSpaceCameraPos, 1))).xyz;
        o.LightLocalDir =  mul((float4x4)unity_WorldToObject, (float4(LightTransform.xyz, 1))).xyz;
        return o;
    }
        float4 frag (v2f i) : COLOR
        {
        float3 direction = normalize(i.LocalPos - i.LocalEyePos);
        float hitMin, hitMax;
        float Volume = hit(i.LocalEyePos, direction, _BoxMin.xyz, _BoxMax.xyz, hitMin, hitMax);
		int Inside[3] = {0, 0, 0}, bOutside;
        Inside[0] = step(0, abs(i.LocalEyePos.x) - _BoxMax.x);
        Inside[1] = step(0, abs(i.LocalEyePos.y) - _BoxMax.y);
        Inside[2] = step(0, abs(i.LocalEyePos.z) - _BoxMax.z);
        bOutside  = min(1,(float)(Inside[0] + Inside[1] + Inside[2]));
        hitMin*=bOutside;
        float2 ScreenUVs = i.ScreenUVs.xy/i.ScreenUVs.w;
        float Depth =  length(DECODE_EYEDEPTH(tex2D(_CameraDepthTexture, ScreenUVs).r )/normalize(i.ViewPos).z);
        float lDepth = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D (_CameraDepthTexture, ScreenUVs)));
        float MinMax[2] = {max(hitMin, hitMax), min(hitMin, hitMax)};
        float thickness = min(MinMax[0], Depth) - min(MinMax[1], Depth);
        float Fog = thickness / _FogDensity;
        Fog = 1.0 - exp2( -Fog );
        Fog *= Volume;
        float4 ReturnFog=0;
        float3 Normalized_CameraWorldDir = normalize(i.Wpos - _WorldSpaceCameraPos);
        float3 Normalized_CameraLocalDir = normalize(i.LocalPos - i.LocalEyePos);
        float3 CameraLocalDir = (i.LocalPos - i.LocalEyePos);
        half DistanceClamp = saturate( Depth / sunDistanceFactor - sunDistance);
		#define STEPS  50
		float inv_STEPS = 1.0f/(float)STEPS;
        float4 Noise=0, ShadowColor=0;
        float3 rayStart = i.LocalEyePos + Normalized_CameraLocalDir * hitMin;
        float3 rayStop = i.LocalEyePos + Normalized_CameraLocalDir * hitMax;	
		float3 SampleDirection = rayStop - rayStart;
        float3 step = normalize(SampleDirection) * _Ray;
        float3 stepLocal = normalize(direction) * _Ray;
        float Ray = distance(rayStop, rayStart);
        half jitter = 1;
		float3 SampleCoordinates = rayStart;
		#ifdef _FOG_SUNLIGHT
		float Inscattering = He(Normalized_CameraWorldDir, LightTransform, sunRadius);
		Inscattering *= DistanceClamp * Fog;
        ReturnFog = float4( _FogColor.rgb +  _FogColor.rgb * _SunLightColor.rgb * _sunIntensity * Inscattering, _FogColor.a);
        #else
				ReturnFog = _FogColor;
        #endif																
				ReturnFog.rgb *= Exposure;
				ReturnFog.a *= (Fog * _FogColor.a);
				return ReturnFog;
   		 }  
            ENDCG
            SubShader
			{
        Tags {
        "Queue"="Overlay" 
        "IgnoreProjector"="True" 
        "RenderType"="Transparent "
        }

        Blend SrcAlpha OneMinusSrcAlpha   
        Cull Front  
		ZWrite Off  
		ZTest Always
		
		Lighting Off 
		
        Pass{Fog {Mode Off }
        
            CGPROGRAM
            
            #pragma multi_compile _ _FOG_SUNLIGHT
			#pragma glsl
			#pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            ENDCG
        }
	} 
}