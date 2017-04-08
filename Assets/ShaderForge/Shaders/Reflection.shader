// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:33570,y:32795,varname:node_4013,prsc:2|diff-9313-OUT,spec-3475-OUT,gloss-1763-OUT,emission-9313-OUT,alpha-1865-OUT;n:type:ShaderForge.SFN_Tex2d,id:372,x:32274,y:32881,ptovrint:False,ptlb:node_372,ptin:_node_372,varname:node_372,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:23733f148814b8b47a2a4a5eb9e0892c,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Fresnel,id:8623,x:32465,y:32378,varname:node_8623,prsc:2;n:type:ShaderForge.SFN_Vector1,id:3475,x:32629,y:32702,varname:node_3475,prsc:2,v1:6;n:type:ShaderForge.SFN_Vector1,id:1763,x:32629,y:32775,varname:node_1763,prsc:2,v1:0.8;n:type:ShaderForge.SFN_Lerp,id:5392,x:32705,y:32914,varname:node_5392,prsc:2|A-98-OUT,B-372-RGB,T-9446-OUT;n:type:ShaderForge.SFN_Vector3,id:98,x:32413,y:32732,varname:node_98,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Slider,id:9446,x:32142,y:33187,ptovrint:False,ptlb:node_9446,ptin:_node_9446,varname:node_9446,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8,max:1;n:type:ShaderForge.SFN_Tex2d,id:8235,x:32663,y:33205,ptovrint:False,ptlb:node_8235,ptin:_node_8235,varname:node_8235,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:23733f148814b8b47a2a4a5eb9e0892c,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1865,x:32885,y:33117,varname:node_1865,prsc:2|A-4046-OUT,B-8235-R;n:type:ShaderForge.SFN_ValueProperty,id:4046,x:32688,y:33098,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_4046,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_ComponentMask,id:1730,x:32628,y:33399,varname:node_1730,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-372-RGB;n:type:ShaderForge.SFN_ComponentMask,id:2978,x:32914,y:33478,varname:node_2978,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-8235-R;n:type:ShaderForge.SFN_Multiply,id:917,x:32914,y:33307,varname:node_917,prsc:2|A-1730-OUT,B-1167-OUT;n:type:ShaderForge.SFN_Multiply,id:3336,x:33144,y:33213,varname:node_3336,prsc:2|A-917-OUT,B-2978-OUT;n:type:ShaderForge.SFN_Multiply,id:1167,x:32677,y:33568,varname:node_1167,prsc:2|A-9446-OUT,B-2834-OUT;n:type:ShaderForge.SFN_Vector1,id:2834,x:32409,y:33535,varname:node_2834,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Rotator,id:8059,x:32051,y:32898,varname:node_8059,prsc:2|UVIN-9696-UVOUT,SPD-6750-OUT;n:type:ShaderForge.SFN_TexCoord,id:9696,x:31831,y:32762,varname:node_9696,prsc:2,uv:0;n:type:ShaderForge.SFN_ValueProperty,id:6750,x:31831,y:33040,ptovrint:False,ptlb:node_6750,ptin:_node_6750,varname:node_6750,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:-3;n:type:ShaderForge.SFN_Color,id:4575,x:32465,y:32540,ptovrint:False,ptlb:node_4575,ptin:_node_4575,varname:node_4575,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Multiply,id:9313,x:33233,y:32613,varname:node_9313,prsc:2|A-5807-OUT,B-4586-OUT;n:type:ShaderForge.SFN_Vector1,id:4586,x:33037,y:32694,varname:node_4586,prsc:2,v1:10;n:type:ShaderForge.SFN_Lerp,id:5807,x:32774,y:32467,varname:node_5807,prsc:2|A-8623-OUT,B-4575-RGB,T-372-RGB;proporder:372-9446-8235-4046-6750-4575;pass:END;sub:END;*/

Shader "Shader Forge/Portal" {
    Properties {
        _node_372 ("node_372", 2D) = "white" {}
        _node_9446 ("node_9446", Range(0, 1)) = 0.8
        _node_8235 ("node_8235", 2D) = "white" {}
        _Opacity ("Opacity", Float ) = 0.1
        _node_6750 ("node_6750", Float ) = -3
        _node_4575 ("node_4575", Color) = (0,0,0,1)
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
            #pragma only_renderers d3d9 d3d11 glcore gles n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _node_372; uniform float4 _node_372_ST;
            uniform sampler2D _node_8235; uniform float4 _node_8235_ST;
            uniform float _Opacity;
            uniform float4 _node_4575;
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
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float node_8623 = (1.0-max(0,dot(normalDirection, viewDirection)));
                float4 _node_372_var = tex2D(_node_372,TRANSFORM_TEX(i.uv0, _node_372));
                float3 node_9313 = (lerp(float3(node_8623,node_8623,node_8623),_node_4575.rgb,_node_372_var.rgb)*10.0);
                float3 emissive = node_9313;
                float3 finalColor = emissive;
                float4 _node_8235_var = tex2D(_node_8235,TRANSFORM_TEX(i.uv0, _node_8235));
                fixed4 finalRGBA = fixed4(finalColor,(_Opacity*_node_8235_var.r));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
