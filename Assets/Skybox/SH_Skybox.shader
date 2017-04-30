// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:33234,y:32729,varname:node_3138,prsc:2|emission-8096-OUT;n:type:ShaderForge.SFN_Tex2d,id:6364,x:32153,y:32330,ptovrint:False,ptlb:T_Stars,ptin:_T_Stars,varname:node_6364,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:45c978df9fba408429da1763646ecbda,ntxv:0,isnm:False|UVIN-5305-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:1423,x:32153,y:32523,ptovrint:False,ptlb:T_Space,ptin:_T_Space,varname:node_1423,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:419198f66e0cdb247b69352b367d781e,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Blend,id:2086,x:32449,y:32429,varname:node_2086,prsc:2,blmd:7,clmp:True|SRC-6364-RGB,DST-1423-RGB;n:type:ShaderForge.SFN_TexCoord,id:5525,x:31584,y:32641,varname:node_5525,prsc:2,uv:0;n:type:ShaderForge.SFN_Panner,id:5305,x:31961,y:32330,varname:node_5305,prsc:2,spu:0.002,spv:0|UVIN-5525-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:9048,x:32153,y:32720,ptovrint:False,ptlb:T_Planet,ptin:_T_Planet,varname:node_9048,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:4d018f70b1f115d4cb1db1d1f29949ab,ntxv:0,isnm:False|UVIN-1711-UVOUT;n:type:ShaderForge.SFN_Add,id:6437,x:32408,y:32666,varname:node_6437,prsc:2|A-311-OUT,B-9048-RGB;n:type:ShaderForge.SFN_Panner,id:1711,x:31964,y:32720,varname:node_1711,prsc:2,spu:0.001,spv:0|UVIN-5525-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:3907,x:32153,y:32908,ptovrint:False,ptlb:T_Alpha01,ptin:_T_Alpha01,varname:node_3907,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:c4fc1e1c47f0603408697d57c4ed47f5,ntxv:0,isnm:False|UVIN-1711-UVOUT;n:type:ShaderForge.SFN_Lerp,id:9520,x:32656,y:32630,varname:node_9520,prsc:2|A-540-OUT,B-6437-OUT,T-3907-RGB;n:type:ShaderForge.SFN_Tex2d,id:317,x:32378,y:32954,ptovrint:False,ptlb:node_317,ptin:_node_317,varname:node_317,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:9de02d35c4685bc44aeaed318a44a904,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:1873,x:32378,y:33149,ptovrint:False,ptlb:node_1873,ptin:_node_1873,varname:node_1873,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:7ccd7e575f47c8945823aca21e18eb74,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:6132,x:32633,y:33012,varname:node_6132,prsc:2|A-1423-RGB,B-317-RGB,T-1873-RGB;n:type:ShaderForge.SFN_Add,id:9976,x:32673,y:32854,varname:node_9976,prsc:2|A-9520-OUT,B-6132-OUT,C-3907-RGB;n:type:ShaderForge.SFN_Color,id:2904,x:32174,y:33112,ptovrint:False,ptlb:node_2904,ptin:_node_2904,varname:node_2904,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2d,id:4772,x:32714,y:33364,ptovrint:False,ptlb:node_4772,ptin:_node_4772,varname:node_4772,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e4b56100ef536c74da351f290592f5d0,ntxv:0,isnm:False|UVIN-2117-UVOUT;n:type:ShaderForge.SFN_Lerp,id:2623,x:32735,y:33160,varname:node_2623,prsc:2|A-9520-OUT,B-4772-RGB,T-2904-RGB;n:type:ShaderForge.SFN_Panner,id:2117,x:31964,y:32908,varname:node_2117,prsc:2,spu:0.0008,spv:0|UVIN-5525-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:3172,x:32153,y:32136,ptovrint:False,ptlb:node_3172,ptin:_node_3172,varname:node_3172,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:45c978df9fba408429da1763646ecbda,ntxv:0,isnm:False|UVIN-4931-UVOUT;n:type:ShaderForge.SFN_Panner,id:4931,x:31957,y:32141,varname:node_4931,prsc:2,spu:0.003,spv:0|UVIN-5525-UVOUT;n:type:ShaderForge.SFN_Blend,id:540,x:32635,y:32403,varname:node_540,prsc:2,blmd:7,clmp:True|SRC-3172-RGB,DST-2086-OUT;n:type:ShaderForge.SFN_Tex2d,id:8832,x:31811,y:33362,ptovrint:False,ptlb:node_8832,ptin:_node_8832,varname:node_8832,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-2117-UVOUT;n:type:ShaderForge.SFN_Blend,id:311,x:31900,y:33099,varname:node_311,prsc:2,blmd:7,clmp:True|SRC-5497-OUT,DST-1423-RGB;n:type:ShaderForge.SFN_Lerp,id:5497,x:32050,y:33442,varname:node_5497,prsc:2|A-8832-RGB,B-6138-RGB,T-9161-RGB;n:type:ShaderForge.SFN_Color,id:6138,x:31811,y:33548,ptovrint:False,ptlb:node_6138,ptin:_node_6138,varname:node_6138,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Color,id:9161,x:31820,y:33734,ptovrint:False,ptlb:node_9161,ptin:_node_9161,varname:node_9161,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.6102941,c2:0.6102941,c3:0.6102941,c4:1;n:type:ShaderForge.SFN_Blend,id:6654,x:32944,y:32375,varname:node_6654,prsc:2,blmd:7,clmp:True|SRC-540-OUT,DST-6437-OUT;n:type:ShaderForge.SFN_Lerp,id:236,x:32944,y:32589,varname:node_236,prsc:2|A-6654-OUT,B-6437-OUT,T-3907-RGB;n:type:ShaderForge.SFN_Lerp,id:5675,x:33169,y:32555,varname:node_5675,prsc:2|A-9048-RGB,B-6654-OUT,T-9976-OUT;n:type:ShaderForge.SFN_Lerp,id:8096,x:33342,y:32471,varname:node_8096,prsc:2|A-6654-OUT,B-5675-OUT,T-2904-RGB;proporder:6364-1423-9048-3907-317-1873-2904-4772-3172-8832-6138-9161;pass:END;sub:END;*/

Shader "Shader Forge/SH_Skybox" {
    Properties {
        _T_Stars ("T_Stars", 2D) = "white" {}
        _T_Space ("T_Space", 2D) = "white" {}
        _T_Planet ("T_Planet", 2D) = "white" {}
        _T_Alpha01 ("T_Alpha01", 2D) = "white" {}
        _node_317 ("node_317", 2D) = "white" {}
        _node_1873 ("node_1873", 2D) = "white" {}
        _node_2904 ("node_2904", Color) = (0.5,0.5,0.5,1)
        _node_4772 ("node_4772", 2D) = "white" {}
        _node_3172 ("node_3172", 2D) = "white" {}
        _node_8832 ("node_8832", 2D) = "white" {}
        _node_6138 ("node_6138", Color) = (0,0,0,1)
        _node_9161 ("node_9161", Color) = (0.6102941,0.6102941,0.6102941,1)
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
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _T_Stars; uniform float4 _T_Stars_ST;
            uniform sampler2D _T_Space; uniform float4 _T_Space_ST;
            uniform sampler2D _T_Planet; uniform float4 _T_Planet_ST;
            uniform sampler2D _T_Alpha01; uniform float4 _T_Alpha01_ST;
            uniform sampler2D _node_317; uniform float4 _node_317_ST;
            uniform sampler2D _node_1873; uniform float4 _node_1873_ST;
            uniform float4 _node_2904;
            uniform sampler2D _node_3172; uniform float4 _node_3172_ST;
            uniform sampler2D _node_8832; uniform float4 _node_8832_ST;
            uniform float4 _node_6138;
            uniform float4 _node_9161;
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
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_8946 = _Time + _TimeEditor;
                float2 node_4931 = (i.uv0+node_8946.g*float2(0.003,0));
                float4 _node_3172_var = tex2D(_node_3172,TRANSFORM_TEX(node_4931, _node_3172));
                float2 node_5305 = (i.uv0+node_8946.g*float2(0.002,0));
                float4 _T_Stars_var = tex2D(_T_Stars,TRANSFORM_TEX(node_5305, _T_Stars));
                float4 _T_Space_var = tex2D(_T_Space,TRANSFORM_TEX(i.uv0, _T_Space));
                float3 node_540 = saturate((saturate((_T_Space_var.rgb/(1.0-_T_Stars_var.rgb)))/(1.0-_node_3172_var.rgb)));
                float2 node_2117 = (i.uv0+node_8946.g*float2(0.0008,0));
                float4 _node_8832_var = tex2D(_node_8832,TRANSFORM_TEX(node_2117, _node_8832));
                float2 node_1711 = (i.uv0+node_8946.g*float2(0.001,0));
                float4 _T_Planet_var = tex2D(_T_Planet,TRANSFORM_TEX(node_1711, _T_Planet));
                float3 node_6437 = (saturate((_T_Space_var.rgb/(1.0-lerp(_node_8832_var.rgb,_node_6138.rgb,_node_9161.rgb))))+_T_Planet_var.rgb);
                float3 node_6654 = saturate((node_6437/(1.0-node_540)));
                float4 _T_Alpha01_var = tex2D(_T_Alpha01,TRANSFORM_TEX(node_1711, _T_Alpha01));
                float3 node_9520 = lerp(node_540,node_6437,_T_Alpha01_var.rgb);
                float4 _node_317_var = tex2D(_node_317,TRANSFORM_TEX(i.uv0, _node_317));
                float4 _node_1873_var = tex2D(_node_1873,TRANSFORM_TEX(i.uv0, _node_1873));
                float3 emissive = lerp(node_6654,lerp(_T_Planet_var.rgb,node_6654,(node_9520+lerp(_T_Space_var.rgb,_node_317_var.rgb,_node_1873_var.rgb)+_T_Alpha01_var.rgb)),_node_2904.rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
