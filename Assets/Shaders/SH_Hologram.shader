// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:32754,y:32596,varname:node_4013,prsc:2|emission-6669-OUT,alpha-6413-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9114,x:30173,y:33031,ptovrint:False,ptlb:Scanline Density,ptin:_ScanlineDensity,varname:node_9114,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Vector1,id:1888,x:30173,y:32935,varname:node_1888,prsc:2,v1:1;n:type:ShaderForge.SFN_Append,id:5673,x:30379,y:32976,varname:node_5673,prsc:2|A-1888-OUT,B-9114-OUT;n:type:ShaderForge.SFN_Multiply,id:1498,x:30597,y:32884,varname:node_1498,prsc:2|A-3361-OUT,B-5673-OUT;n:type:ShaderForge.SFN_Append,id:3361,x:30379,y:32806,varname:node_3361,prsc:2|A-7956-X,B-7956-Y;n:type:ShaderForge.SFN_FragmentPosition,id:7956,x:30173,y:32745,varname:node_7956,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:3928,x:30167,y:33263,ptovrint:False,ptlb:Scanline Speed _copy,ptin:_ScanlineSpeed_copy,varname:_ScanlineSpeed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:5;n:type:ShaderForge.SFN_Append,id:6965,x:30379,y:33203,varname:node_6965,prsc:2|A-4645-OUT,B-3928-OUT;n:type:ShaderForge.SFN_Multiply,id:783,x:30598,y:33203,varname:node_783,prsc:2|A-6965-OUT,B-5120-TSL;n:type:ShaderForge.SFN_Time,id:5120,x:30379,y:33378,varname:node_5120,prsc:2;n:type:ShaderForge.SFN_Add,id:5376,x:30852,y:33034,varname:node_5376,prsc:2|A-1498-OUT,B-783-OUT;n:type:ShaderForge.SFN_OneMinus,id:5338,x:31048,y:33034,varname:node_5338,prsc:2|IN-5376-OUT;n:type:ShaderForge.SFN_ComponentMask,id:7436,x:31244,y:33034,varname:node_7436,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-5338-OUT;n:type:ShaderForge.SFN_Frac,id:2355,x:31461,y:32970,varname:node_2355,prsc:2|IN-7436-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2430,x:31461,y:33134,ptovrint:False,ptlb:Scanline Exp,ptin:_ScanlineExp,varname:node_2430,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;n:type:ShaderForge.SFN_Power,id:794,x:31667,y:33040,varname:node_794,prsc:2|VAL-2355-OUT,EXP-2430-OUT;n:type:ShaderForge.SFN_ScreenPos,id:6385,x:30843,y:32482,varname:node_6385,prsc:2,sctp:0;n:type:ShaderForge.SFN_Time,id:104,x:30843,y:32643,varname:node_104,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1836,x:31045,y:32558,varname:node_1836,prsc:2|A-6385-UVOUT,B-104-TSL;n:type:ShaderForge.SFN_Noise,id:2329,x:31235,y:32558,varname:node_2329,prsc:2|XY-1836-OUT;n:type:ShaderForge.SFN_RemapRange,id:9569,x:31427,y:32558,varname:node_9569,prsc:2,frmn:0,frmx:1,tomn:0.5,tomx:1|IN-2329-OUT;n:type:ShaderForge.SFN_Tex2d,id:7286,x:30843,y:32272,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:node_7286,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3cb0d5724e887a24588dd5e87a1dcd8e,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:4883,x:30843,y:32087,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_4883,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:1,c3:0.9586205,c4:1;n:type:ShaderForge.SFN_Desaturate,id:8050,x:31209,y:32214,varname:node_8050,prsc:2|COL-7286-RGB,DES-7695-OUT;n:type:ShaderForge.SFN_Slider,id:7695,x:31052,y:32413,ptovrint:False,ptlb:Desaturation Amount,ptin:_DesaturationAmount,varname:node_7695,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:9242,x:31444,y:32155,varname:node_9242,prsc:2|A-4883-RGB,B-8050-OUT;n:type:ShaderForge.SFN_Multiply,id:840,x:31709,y:32646,varname:node_840,prsc:2|A-9569-OUT,B-8575-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8575,x:31427,y:32762,ptovrint:False,ptlb:Noise Power,ptin:_NoisePower,varname:node_8575,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Lerp,id:9805,x:31709,y:32453,varname:node_9805,prsc:2|A-4564-OUT,B-840-OUT,T-8575-OUT;n:type:ShaderForge.SFN_Vector1,id:4564,x:31427,y:32473,varname:node_4564,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:1485,x:31709,y:32272,varname:node_1485,prsc:2|A-9242-OUT,B-9805-OUT;n:type:ShaderForge.SFN_Multiply,id:2061,x:32058,y:32455,varname:node_2061,prsc:2|A-1485-OUT,B-983-OUT;n:type:ShaderForge.SFN_ValueProperty,id:983,x:31864,y:32522,ptovrint:False,ptlb:Brightness,ptin:_Brightness,varname:node_983,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:5012,x:31942,y:32972,varname:node_5012,prsc:2|A-794-OUT,B-3910-OUT;n:type:ShaderForge.SFN_Slider,id:3910,x:31864,y:33152,ptovrint:False,ptlb:Scanline Opacity,ptin:_ScanlineOpacity,varname:node_3910,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Add,id:6413,x:32293,y:32745,varname:node_6413,prsc:2|A-8462-OUT,B-5012-OUT;n:type:ShaderForge.SFN_Slider,id:8462,x:31785,y:32890,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_8462,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Add,id:3718,x:32293,y:32588,varname:node_3718,prsc:2|A-2061-OUT,B-5012-OUT;n:type:ShaderForge.SFN_Multiply,id:6669,x:32524,y:32655,varname:node_6669,prsc:2|A-3718-OUT,B-6413-OUT;n:type:ShaderForge.SFN_Vector1,id:4645,x:30167,y:33160,varname:node_4645,prsc:2,v1:0;proporder:9114-3928-2430-7286-4883-7695-8575-983-3910-8462;pass:END;sub:END;*/

Shader "Shader Forge/SH_Hologram" {
    Properties {
        _ScanlineDensity ("Scanline Density", Float ) = 1
        _ScanlineSpeed_copy ("Scanline Speed _copy", Float ) = 5
        _ScanlineExp ("Scanline Exp", Float ) = 10
        _Texture ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0,1,0.9586205,1)
        _DesaturationAmount ("Desaturation Amount", Range(0, 1)) = 1
        _NoisePower ("Noise Power", Float ) = 1
        _Brightness ("Brightness", Float ) = 1
        _ScanlineOpacity ("Scanline Opacity", Range(0, 1)) = 1
        _Opacity ("Opacity", Range(0, 1)) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
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
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float _ScanlineDensity;
            uniform float _ScanlineSpeed_copy;
            uniform float _ScanlineExp;
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform float4 _Color;
            uniform float _DesaturationAmount;
            uniform float _NoisePower;
            uniform float _Brightness;
            uniform float _ScanlineOpacity;
            uniform float _Opacity;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
////// Lighting:
////// Emissive:
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(i.uv0, _Texture));
                float4 node_104 = _Time + _TimeEditor;
                float2 node_1836 = (i.screenPos.rg*node_104.r);
                float2 node_2329_skew = node_1836 + 0.2127+node_1836.x*0.3713*node_1836.y;
                float2 node_2329_rnd = 4.789*sin(489.123*(node_2329_skew));
                float node_2329 = frac(node_2329_rnd.x*node_2329_rnd.y*(1+node_2329_skew.x));
                float4 node_5120 = _Time + _TimeEditor;
                float node_5012 = (pow(frac((1.0 - ((float2(i.posWorld.r,i.posWorld.g)*float2(1.0,_ScanlineDensity))+(float2(0.0,_ScanlineSpeed_copy)*node_5120.r))).g),_ScanlineExp)*_ScanlineOpacity);
                float node_6413 = (_Opacity+node_5012);
                float3 emissive = (((((_Color.rgb*lerp(_Texture_var.rgb,dot(_Texture_var.rgb,float3(0.3,0.59,0.11)),_DesaturationAmount))*lerp(1.0,((node_2329*0.5+0.5)*_NoisePower),_NoisePower))*_Brightness)+node_5012)*node_6413);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,node_6413);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
