// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:33498,y:33067,varname:node_4013,prsc:2|diff-7791-OUT,emission-9969-RGB,alpha-5947-OUT,clip-5947-OUT;n:type:ShaderForge.SFN_Tex2d,id:1555,x:32416,y:32045,ptovrint:False,ptlb:Layer 1,ptin:_Layer1,varname:node_1555,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:5dd09cae55293e147817ac93510194c7,ntxv:0,isnm:False|UVIN-3515-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:2932,x:32416,y:32228,ptovrint:False,ptlb:Layer 2,ptin:_Layer2,varname:node_2932,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:d37b7d354a030a041af417b0a908547c,ntxv:0,isnm:False|UVIN-5482-UVOUT;n:type:ShaderForge.SFN_Lerp,id:6428,x:32675,y:32209,varname:node_6428,prsc:2|A-1555-RGB,B-2932-RGB,T-3892-RGB;n:type:ShaderForge.SFN_Tex2d,id:3892,x:32416,y:32419,ptovrint:False,ptlb:Alpha L2,ptin:_AlphaL2,varname:node_3892,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:8b0db89cb010faa469d097ab10d911d8,ntxv:0,isnm:False|UVIN-5482-UVOUT;n:type:ShaderForge.SFN_Panner,id:5482,x:32181,y:32228,varname:node_5482,prsc:2,spu:0.5,spv:0.3|UVIN-6709-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:6709,x:31935,y:32228,varname:node_6709,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:1771,x:32416,y:32616,ptovrint:False,ptlb:Layer 3,ptin:_Layer3,varname:node_1771,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:aec8ee41990b93c489b0968f38b28b98,ntxv:0,isnm:False|UVIN-4922-UVOUT;n:type:ShaderForge.SFN_Panner,id:3515,x:32181,y:32045,varname:node_3515,prsc:2,spu:0,spv:0.1|UVIN-4224-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:4224,x:31935,y:32045,varname:node_4224,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:9258,x:32421,y:32808,ptovrint:False,ptlb:Alpha L3,ptin:_AlphaL3,varname:node_9258,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:6ada28538acd1f347833fe7234c68749,ntxv:0,isnm:False|UVIN-4922-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:216,x:32422,y:33003,ptovrint:False,ptlb:Layer 4,ptin:_Layer4,varname:node_216,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:480e16a2cb7ba6c49baf207f2debe457,ntxv:0,isnm:False|UVIN-3039-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:9117,x:32422,y:33196,ptovrint:False,ptlb:Alpha L4,ptin:_AlphaL4,varname:node_9117,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:23d933a35a16e984fa3649b72dc813b2,ntxv:0,isnm:False|UVIN-3039-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:6596,x:32422,y:33393,ptovrint:False,ptlb:Layer 5,ptin:_Layer5,varname:node_6596,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:fa4307b7e00c58f49b6b014b0c63b0fe,ntxv:0,isnm:False|UVIN-3076-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:4785,x:32422,y:33592,ptovrint:False,ptlb:Alpha L5,ptin:_AlphaL5,varname:node_4785,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:2cd528cf6816dc2429166f49a2ee1237,ntxv:0,isnm:False|UVIN-3076-UVOUT;n:type:ShaderForge.SFN_Lerp,id:1877,x:32668,y:32597,varname:node_1877,prsc:2|A-6428-OUT,B-1771-RGB,T-9258-RGB;n:type:ShaderForge.SFN_Lerp,id:8188,x:32668,y:32983,varname:node_8188,prsc:2|A-1877-OUT,B-216-RGB,T-9117-RGB;n:type:ShaderForge.SFN_Lerp,id:119,x:32672,y:33373,varname:node_119,prsc:2|A-8188-OUT,B-6596-RGB,T-4785-RGB;n:type:ShaderForge.SFN_Panner,id:4922,x:32180,y:32616,varname:node_4922,prsc:2,spu:0,spv:0.2|UVIN-2520-UVOUT;n:type:ShaderForge.SFN_Panner,id:3039,x:32183,y:33003,varname:node_3039,prsc:2,spu:1,spv:0.5|UVIN-1837-UVOUT;n:type:ShaderForge.SFN_Panner,id:3076,x:32187,y:33393,varname:node_3076,prsc:2,spu:0,spv:0.4|UVIN-3067-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:2520,x:31934,y:32616,varname:node_2520,prsc:2,uv:0;n:type:ShaderForge.SFN_TexCoord,id:1837,x:31933,y:33003,varname:node_1837,prsc:2,uv:0;n:type:ShaderForge.SFN_TexCoord,id:3067,x:31934,y:33393,varname:node_3067,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:5947,x:33147,y:33479,varname:node_5947,prsc:2|A-2810-OUT,B-4295-R;n:type:ShaderForge.SFN_ValueProperty,id:2810,x:32928,y:33479,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_2810,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Tex2d,id:4295,x:32928,y:33578,ptovrint:False,ptlb:Opatexture,ptin:_Opatexture,varname:node_4295,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:23733f148814b8b47a2a4a5eb9e0892c,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:9969,x:33202,y:33304,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_9969,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2d,id:295,x:33084,y:33816,ptovrint:False,ptlb:Neutral,ptin:_Neutral,varname:node_295,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:dde976bcdaaea1143b8a48a75b7557be,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:7791,x:33131,y:34026,varname:node_7791,prsc:2|A-295-RGB,B-119-OUT,T-8811-OUT;n:type:ShaderForge.SFN_Slider,id:8811,x:32744,y:34066,ptovrint:False,ptlb:Lerp,ptin:_Lerp,varname:node_8811,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;proporder:1555-2932-3892-1771-9258-216-9117-6596-4785-2810-4295-9969-295-8811;pass:END;sub:END;*/

Shader "Shader Forge/LavaLitRed" {
    Properties {
        _Layer1 ("Layer 1", 2D) = "white" {}
        _Layer2 ("Layer 2", 2D) = "white" {}
        _AlphaL2 ("Alpha L2", 2D) = "white" {}
        _Layer3 ("Layer 3", 2D) = "white" {}
        _AlphaL3 ("Alpha L3", 2D) = "white" {}
        _Layer4 ("Layer 4", 2D) = "white" {}
        _AlphaL4 ("Alpha L4", 2D) = "white" {}
        _Layer5 ("Layer 5", 2D) = "white" {}
        _AlphaL5 ("Alpha L5", 2D) = "white" {}
        _Opacity ("Opacity", Float ) = 1
        _Opatexture ("Opatexture", 2D) = "white" {}
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Neutral ("Neutral", 2D) = "white" {}
        _Lerp ("Lerp", Range(0, 1)) = 0
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
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _Layer1; uniform float4 _Layer1_ST;
            uniform sampler2D _Layer2; uniform float4 _Layer2_ST;
            uniform sampler2D _AlphaL2; uniform float4 _AlphaL2_ST;
            uniform sampler2D _Layer3; uniform float4 _Layer3_ST;
            uniform sampler2D _AlphaL3; uniform float4 _AlphaL3_ST;
            uniform sampler2D _Layer4; uniform float4 _Layer4_ST;
            uniform sampler2D _AlphaL4; uniform float4 _AlphaL4_ST;
            uniform sampler2D _Layer5; uniform float4 _Layer5_ST;
            uniform sampler2D _AlphaL5; uniform float4 _AlphaL5_ST;
            uniform float _Opacity;
            uniform sampler2D _Opatexture; uniform float4 _Opatexture_ST;
            uniform float4 _Color;
            uniform sampler2D _Neutral; uniform float4 _Neutral_ST;
            uniform float _Lerp;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float4 _Opatexture_var = tex2D(_Opatexture,TRANSFORM_TEX(i.uv0, _Opatexture));
                float node_5947 = (_Opacity*_Opatexture_var.r);
                clip(node_5947 - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float4 _Neutral_var = tex2D(_Neutral,TRANSFORM_TEX(i.uv0, _Neutral));
                float4 node_3951 = _Time + _TimeEditor;
                float2 node_3515 = (i.uv0+node_3951.g*float2(0,0.1));
                float4 _Layer1_var = tex2D(_Layer1,TRANSFORM_TEX(node_3515, _Layer1));
                float2 node_5482 = (i.uv0+node_3951.g*float2(0.5,0.3));
                float4 _Layer2_var = tex2D(_Layer2,TRANSFORM_TEX(node_5482, _Layer2));
                float4 _AlphaL2_var = tex2D(_AlphaL2,TRANSFORM_TEX(node_5482, _AlphaL2));
                float2 node_4922 = (i.uv0+node_3951.g*float2(0,0.2));
                float4 _Layer3_var = tex2D(_Layer3,TRANSFORM_TEX(node_4922, _Layer3));
                float4 _AlphaL3_var = tex2D(_AlphaL3,TRANSFORM_TEX(node_4922, _AlphaL3));
                float2 node_3039 = (i.uv0+node_3951.g*float2(1,0.5));
                float4 _Layer4_var = tex2D(_Layer4,TRANSFORM_TEX(node_3039, _Layer4));
                float4 _AlphaL4_var = tex2D(_AlphaL4,TRANSFORM_TEX(node_3039, _AlphaL4));
                float2 node_3076 = (i.uv0+node_3951.g*float2(0,0.4));
                float4 _Layer5_var = tex2D(_Layer5,TRANSFORM_TEX(node_3076, _Layer5));
                float4 _AlphaL5_var = tex2D(_AlphaL5,TRANSFORM_TEX(node_3076, _AlphaL5));
                float3 diffuseColor = lerp(_Neutral_var.rgb,lerp(lerp(lerp(lerp(_Layer1_var.rgb,_Layer2_var.rgb,_AlphaL2_var.rgb),_Layer3_var.rgb,_AlphaL3_var.rgb),_Layer4_var.rgb,_AlphaL4_var.rgb),_Layer5_var.rgb,_AlphaL5_var.rgb),_Lerp);
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 emissive = _Color.rgb;
/// Final Color:
                float3 finalColor = diffuse + emissive;
                fixed4 finalRGBA = fixed4(finalColor,node_5947);
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
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _Layer1; uniform float4 _Layer1_ST;
            uniform sampler2D _Layer2; uniform float4 _Layer2_ST;
            uniform sampler2D _AlphaL2; uniform float4 _AlphaL2_ST;
            uniform sampler2D _Layer3; uniform float4 _Layer3_ST;
            uniform sampler2D _AlphaL3; uniform float4 _AlphaL3_ST;
            uniform sampler2D _Layer4; uniform float4 _Layer4_ST;
            uniform sampler2D _AlphaL4; uniform float4 _AlphaL4_ST;
            uniform sampler2D _Layer5; uniform float4 _Layer5_ST;
            uniform sampler2D _AlphaL5; uniform float4 _AlphaL5_ST;
            uniform float _Opacity;
            uniform sampler2D _Opatexture; uniform float4 _Opatexture_ST;
            uniform float4 _Color;
            uniform sampler2D _Neutral; uniform float4 _Neutral_ST;
            uniform float _Lerp;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float4 _Opatexture_var = tex2D(_Opatexture,TRANSFORM_TEX(i.uv0, _Opatexture));
                float node_5947 = (_Opacity*_Opatexture_var.r);
                clip(node_5947 - 0.5);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 _Neutral_var = tex2D(_Neutral,TRANSFORM_TEX(i.uv0, _Neutral));
                float4 node_3418 = _Time + _TimeEditor;
                float2 node_3515 = (i.uv0+node_3418.g*float2(0,0.1));
                float4 _Layer1_var = tex2D(_Layer1,TRANSFORM_TEX(node_3515, _Layer1));
                float2 node_5482 = (i.uv0+node_3418.g*float2(0.5,0.3));
                float4 _Layer2_var = tex2D(_Layer2,TRANSFORM_TEX(node_5482, _Layer2));
                float4 _AlphaL2_var = tex2D(_AlphaL2,TRANSFORM_TEX(node_5482, _AlphaL2));
                float2 node_4922 = (i.uv0+node_3418.g*float2(0,0.2));
                float4 _Layer3_var = tex2D(_Layer3,TRANSFORM_TEX(node_4922, _Layer3));
                float4 _AlphaL3_var = tex2D(_AlphaL3,TRANSFORM_TEX(node_4922, _AlphaL3));
                float2 node_3039 = (i.uv0+node_3418.g*float2(1,0.5));
                float4 _Layer4_var = tex2D(_Layer4,TRANSFORM_TEX(node_3039, _Layer4));
                float4 _AlphaL4_var = tex2D(_AlphaL4,TRANSFORM_TEX(node_3039, _AlphaL4));
                float2 node_3076 = (i.uv0+node_3418.g*float2(0,0.4));
                float4 _Layer5_var = tex2D(_Layer5,TRANSFORM_TEX(node_3076, _Layer5));
                float4 _AlphaL5_var = tex2D(_AlphaL5,TRANSFORM_TEX(node_3076, _AlphaL5));
                float3 diffuseColor = lerp(_Neutral_var.rgb,lerp(lerp(lerp(lerp(_Layer1_var.rgb,_Layer2_var.rgb,_AlphaL2_var.rgb),_Layer3_var.rgb,_AlphaL3_var.rgb),_Layer4_var.rgb,_AlphaL4_var.rgb),_Layer5_var.rgb,_AlphaL5_var.rgb),_Lerp);
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor * node_5947,0);
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
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float _Opacity;
            uniform sampler2D _Opatexture; uniform float4 _Opatexture_ST;
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
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 _Opatexture_var = tex2D(_Opatexture,TRANSFORM_TEX(i.uv0, _Opatexture));
                float node_5947 = (_Opacity*_Opatexture_var.r);
                clip(node_5947 - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
