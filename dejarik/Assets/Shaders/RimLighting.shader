// Upgrade NOTE: replaced 'PositionFog()' with transforming position into clip space.
// Upgrade NOTE: replaced 'V2F_POS_FOG' with 'float4 pos : SV_POSITION'
// Upgrade NOTE: replaced '_PPLAmbient' with 'UNITY_LIGHTMODEL_AMBIENT'

// http://wiki.unity3d.com/index.php/BumpSpecRim

Shader "BumpSpecRim" {

     Properties {
        _Color ("Main Color", Color) = (1,1,1,0.5)
        _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
        _Shininess ("Shininess", Range (0.03, 1)) = 0.078125
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _BumpMap ("Bumpmap (RGB) Rim-Light Ramp (A)", 2D) = "bump" {}
        _RimLightColor ("Rimlight Color", Color) = (0.6, 0.6, 0.7, 1.0)
        _RimLightRamp ("Rimlight Ramp", 2D) = "white" {}

     }
     SubShader {
          //Self-Illumination Depending on Facing Rotation
          Pass {
               Tags {"LightMode" = "Always" /* Upgrade NOTE: changed from PixelOrNone to Always */}
               /* Upgrade NOTE: commented out, possibly part of old style per-pixel lighting: Blend AppSrcAdd AppDstAdd */
               Color [_PPLAmbient]

               CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members uv,tangentSpaceLightDir)
#pragma exclude_renderers d3d11
               // profiles arbfp1
               #pragma vertex vert
               #pragma fragment frag
               #pragma fragmentoption ARB_fog_exp2

               #include "UnityCG.cginc"

               sampler2D _BumpMap;
               sampler2D _RimLightRamp;
               sampler2D _MainTex;
               float4 _RimLightColor;
               float4 _Color;

               struct v2f {
                    float4 pos : SV_POSITION;
                    float2 uv;
                    float3 tangentSpaceLightDir;
               };

               v2f vert (appdata_tan v)
               {
                    v2f o;
                    o.pos = UnityObjectToClipPos (v.vertex);
                    o.uv = TRANSFORM_UV(0);

                    float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));

                    float3 binormal = cross( normalize(v.normal), normalize(v.tangent.xyz) );
                    float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal );

                    o.tangentSpaceLightDir = mul(rotation, viewDir);

                    return o;
               }
               
               half4 frag (v2f i) : COLOR
               {
                    half3 tangentSpaceNormal = (tex2D(_BumpMap, i.uv).rgb * 2.0) - 1.0;
                    
                    half4 result = float4(0, 0, 0, 1);
                    
                    //You might want to normalize tangentSpaceNormal and i.tangentSpaceLightDir,
                    //but for most meshes this will most likely have minimal, if any, impact on quality.
                    float rampSample = dot(tangentSpaceNormal, i.tangentSpaceLightDir);
                    float intensity = tex2D(_RimLightRamp, rampSample.xx).r;
                    
                    
                    result.rgb = intensity * _RimLightColor.rgb;
                    result.rgb += tex2D(_MainTex, i.uv).rgb * UNITY_LIGHTMODEL_AMBIENT.rgb;
                    
                    return result;
               }
         
               ENDCG
          }
       
          UsePass "Bumped Specular/PPL"
    }

    FallBack "Bumped Specular", 1
}