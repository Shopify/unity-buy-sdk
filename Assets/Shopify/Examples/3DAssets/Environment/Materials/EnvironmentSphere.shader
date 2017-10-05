// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33341,y:32816,varname:node_3138,prsc:2|emission-1304-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32232,y:32204,ptovrint:False,ptlb:Color1,ptin:_Color1,varname:_Color1,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:4983,x:32232,y:32644,ptovrint:False,ptlb:EnvironmentGradient,ptin:_EnvironmentGradient,varname:_EnvironmentGradient,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:20a88105409a94049acadd896320411f,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:8226,x:32684,y:32526,varname:node_8226,prsc:2|A-7241-RGB,B-2127-RGB,T-4983-RGB;n:type:ShaderForge.SFN_Color,id:2127,x:32232,y:32427,ptovrint:False,ptlb:Color2,ptin:_Color2,varname:_Color2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:3260,x:32790,y:33046,ptovrint:False,ptlb:EmissiveMultiplier,ptin:_EmissiveMultiplier,varname:_EmissiveMultiplier,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:1304,x:33070,y:32835,varname:node_1304,prsc:2|A-8226-OUT,B-3260-OUT;proporder:7241-2127-3260-4983;pass:END;sub:END;*/

Shader "Shader Forge/EnvironmentSphere" {
    Properties {
        _Color1 ("Color1", Color) = (1,1,1,1)
        _Color2 ("Color2", Color) = (0,0,0,1)
        _EmissiveMultiplier ("EmissiveMultiplier", Float ) = 1
        _EnvironmentGradient ("EnvironmentGradient", 2D) = "white" {}
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
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform float4 _Color1;
            uniform sampler2D _EnvironmentGradient; uniform float4 _EnvironmentGradient_ST;
            uniform float4 _Color2;
            uniform float _EmissiveMultiplier;
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
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 _EnvironmentGradient_var = tex2D(_EnvironmentGradient,TRANSFORM_TEX(i.uv0, _EnvironmentGradient));
                float3 emissive = (lerp(_Color1.rgb,_Color2.rgb,_EnvironmentGradient_var.rgb)*_EmissiveMultiplier);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
