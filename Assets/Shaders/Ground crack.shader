Shader "Lineage/URP/Ground crack"
{
    Properties
    {
        [Header(Texture)]
        [Enum(Off,0,On,1)]_Zwrite("Zwrite",int)=0

        _MainTex("MainTex", 2D) = "white" {}
        _Normal("Normal",2D)="bump"{}
        _SpecTex("SpecTex",2D)="white"{}
        _EmiTex("RGB:EmiTex",2D)="black"{}
        // _Cubmap("环境天空球",Cube)="_skybox"{}

        [Header(Diffuse)]
        _MainCol("MainCol",color) = (1,1,1,1)
        _EnvDiffInt("EnvDiffInt",range(0,1))=0.2
        // _EnvUpCol("环境顶部颜色",color)=(1,1,1,1)
        // _EnvSideCol("环境水平颜色",color)=(0.5,0.5,0.5,1)
        // _EnvDownCol("环境地面颜色",color)=(1,1,1,1)

        [Header(specular)]
        [Toggle]_specularToggle("specularToggle",float)=0
        [PowerSlider(0.5)]_SpecPow("SpecPow",range(1,200))=10
        // _EnvSpecInt("环境镜面反射强度",range(0,5))=1
        // _FresnelPow("菲尼尔强度",range(0,10))=1
        // _CubemapMip("环境球Mip",range(1,7))=0
        [Header(Emission)]
        [HDR] _EmiCol("EmiTionCol",color)=(1,1,1,1)
        _EmitInt("EmitInt",Range(0,10))=1
        _mask("mask",2D) = "white" {}
        _dissolve("dissolveTex",2D) = "white" {}
        _CutOffIntensity("CutOffIntensity",range(0,1))=0
        _CufOffSoft("CufOffSoft",range(0,0.5))=0

        _NormalScale("NormalScale",range(0,1.5))=1
        _Light("LightPosition",vector)=(0,0,0,0)




    }

    SubShader
    {
        Tags { "Queue"="Transparent"  "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        // 
        Blend SrcAlpha OneMinusSrcAlpha


        Pass
        {
            ZWrite [_Zwrite]
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma shader_feature _ _SPECULARTOGGLE_ON
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS       : POSITION;
                float2 uv               : TEXCOORD0;
                float4 normal :NORMAL;
                float4 tangent:TANGENT;
                half4 color:COLOR;
            };

            struct Varyings
            {
                float4 positionCS       : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float4 posWS : TEXCOORD1;//世界空间顶点坐标
                float4 nDirWS : TEXCOORD2;//法线
                float3 tDirWS : TEXCOORD3;//切线
                float3 bDirWS : TEXCOORD4;//负切线
                float4 uv2:TEXCOORD5;
                float2 normaluv:TEXCOORD6;
                half4 color:TEXCOORD7;


            };

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST,_dissolve_ST,_Normal_ST;
                half4 _MainCol,_EnvUpCol,_EnvSideCol,_EnvDownCol,_EmiCol;
                half _EnvDiffInt,_CubemapMip;
                half4 _Light;
                float _SpecPow,_EnvSpecInt,_FresnelPow,_EmitInt,_CutOffIntensity,_CufOffSoft,_NormalScale;
                

            CBUFFER_END
            TEXTURE2D (_MainTex);SAMPLER(sampler_MainTex);
            TEXTURE2D (_Normal);SAMPLER(sampler_Normal);
            TEXTURE2D (_SpecTex);SAMPLER(sampler_SpecTex);
            TEXTURE2D (_mask);SAMPLER(sampler_mask);
            TEXTURE2D (_dissolve);SAMPLER(sampler_dissolve);

            

            
            TEXTURE2D (_EmiTex);SAMPLER(sampler_EmiTex);
            TEXTURECUBE (_Cubmap);SAMPLER(sampler_Cubmap);


            



            Varyings vert(Attributes v)
            {
                Varyings o = (Varyings)0;
                o.uv2.xy=v.uv;
                o.uv2.zw= TRANSFORM_TEX(v.uv, _dissolve);
                o.color=v.color;


                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normaluv=TRANSFORM_TEX(v.uv, _Normal);
                o.posWS.xyz=TransformObjectToWorld(v.positionOS.xyz);//位置OS>WS
                o.nDirWS.xyz=TransformObjectToWorldNormal(v.normal.xyz);
                o.tDirWS.xyz = TransformObjectToWorldDir(v.tangent.xyz);//本地切线>世界
                half sign = v.tangent.w * GetOddNegativeScale();
                o.bDirWS.xyz =normalize (cross(o.nDirWS.xyz, o.tDirWS) * sign);



                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half3 normalMap = UnpackNormalScale(SAMPLE_TEXTURE2D(_Normal, sampler_Normal, i.normaluv),_NormalScale);
                half3 normalWS = mul(normalMap,half3x3(i.tDirWS.xyz, i.bDirWS.xyz, i.nDirWS.xyz));
                
                float3 VdirWS=normalize(_WorldSpaceCameraPos.xyz - i.posWS.xyz);
                float3 VRdirWS=reflect(-VdirWS,normalWS);//反射

                Light mainLight = GetMainLight();
                // half3 lDirWS=normalize(mainLight.direction);//灯光位置
                half3 lDirWS= _Light;
                half3 lrDirWS=reflect(-lDirWS,normalWS); 

                float NdotL=dot(normalWS,lDirWS);
                float vDotr=dot(VdirWS,lrDirWS);
                float vDotN=dot(VdirWS,normalWS);



                half4 cutoffTex = SAMPLE_TEXTURE2D(_dissolve, sampler_dissolve, i.uv2.zw); 
                half4 mask = SAMPLE_TEXTURE2D(_mask, sampler_mask, i.uv);

                half4 var_MainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half4 var_SpecTex = SAMPLE_TEXTURE2D(_SpecTex, sampler_SpecTex, i.uv);

                half4 var_EmiTex = SAMPLE_TEXTURE2D(_EmiTex, sampler_EmiTex, i.uv2.xy);
                float3 baseCol=var_MainTex.xyz*_MainCol.xyz;
                float3 speCol=var_SpecTex.rgb;
                float lambert=max(00.2,NdotL);
                float specPow=lerp(1,_SpecPow,var_SpecTex.a);

                


                float3 envDiff=baseCol*_EnvDiffInt;


                float frsnel= pow(max(0,1-vDotN),_FresnelPow);
                float3 envSpec=0;



                float occlusion=var_MainTex.a;
                
                float3 envLighting=(envDiff+envSpec)*occlusion;
                float emisSinTime=(sin((_Time.z))+1.5);
                float3 emission=var_EmiTex*_EmitInt*_EmiCol*emisSinTime;

                float  alpha2=smoothstep((_CutOffIntensity-0.2),_CutOffIntensity-0.2+_CufOffSoft,cutoffTex.r);


                float3 finaRGB=0;
                float3   dirLighting=0;
                #if _SPECULARTOGGLE_ON
                    float phong=pow(max(0,vDotr),specPow);
                    dirLighting=((baseCol*lambert+phong*speCol)*mainLight.color);
                    finaRGB+=dirLighting;

                #endif
                finaRGB=dirLighting+envLighting+emission*1;

                
                half4 c=1;
                c.rgb=finaRGB*i.color.rgb;
                c.a*=mask*alpha2*i.color.a;

                return c;




            }
            ENDHLSL
        }

    }

}
