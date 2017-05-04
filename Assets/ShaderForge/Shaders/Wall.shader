// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:32885,y:32676,varname:node_4795,prsc:2|emission-742-RGB;n:type:ShaderForge.SFN_Tex2d,id:742,x:32558,y:32695,ptovrint:False,ptlb:node_742,ptin:_node_742,varname:node_742,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:f91d0546989feea46885776d4580755e,ntxv:0,isnm:False|UVIN-9410-UVOUT;n:type:ShaderForge.SFN_Panner,id:9410,x:32365,y:32695,varname:node_9410,prsc:2,spu:0.05,spv:0|UVIN-677-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:677,x:32163,y:32695,varname:node_677,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:9093,x:32519,y:32950,ptovrint:False,ptlb:node_9093,ptin:_node_9093,varname:node_9093,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:cb8407e0f6549f042ad3acb5fa70a62b,ntxv:0,isnm:False|UVIN-5368-UVOUT;n:type:ShaderForge.SFN_Panner,id:5368,x:32323,y:32950,varname:node_5368,prsc:2,spu:0.5,spv:0|UVIN-5138-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:5138,x:32121,y:32950,varname:node_5138,prsc:2,uv:0;proporder:742-9093;pass:END;sub:END;*/

Shader "Shader Forge/Wall" {
    Properties {
        _node_742 ("node_742", 2D) = "white" {}
        _node_9093 ("node_9093", 2D) = "white" {}
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
            Blend One One
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
            uniform float4 _TimeEditor;
            uniform sampler2D _node_742; uniform float4 _node_742_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_4430 = _Time + _TimeEditor;
                float2 node_9410 = (i.uv0+node_4430.g*float2(0.05,0));
                float4 _node_742_var = tex2D(_node_742,TRANSFORM_TEX(node_9410, _node_742));
                float3 emissive = _node_742_var.rgb;
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0,0,0,1));
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
