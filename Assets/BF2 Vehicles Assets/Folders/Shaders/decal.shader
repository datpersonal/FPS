

    Shader "Decal/particle decalsilent"
    {
        Properties
        {
            _Texture("Texture", 2D) = "white" {}
            _scale("scale", float) = 1.0
            _range("Range", range(0.0,10.0)) = 1.0 
        }
        SubShader
        {
            Tags {
                "Queue"="Transparent-1"
                "RenderType"="Transparent"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Front ZWrite Off ZTest Always
            LOD 100
           
            //GrabPass
            //{
            //  Tags { "Queue"="Transparent+10" }
            //  "_GrabTexture2"
                //"_MainTex"
            //}
     
            Pass
            {
            
                Stencil {
                    Ref 17
                    Comp NotEqual
                    Pass Keep
                    ZFail Keep
            }
                
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
               
                #include "UnityCG.cginc"
     
                struct VertIn
                {
                    float4 vertex : POSITION;
                    float3 uv : TEXCOORD0;
                    float3 posWorld : TEXCOORD1;
                };
     
                struct VertOut
                {
                    float4 vertex : SV_POSITION;
                    float4 uv : TEXCOORD0;
                    float3 posWorld : TEXCOORD1;
                    float3 ray : TEXCOORD2;
                };

                float4 _Texture_ST;
     
     
                VertOut vert(VertIn v)
                {
                    VertOut o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                    half index = v.vertex.z;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    float4 screenPos = ComputeGrabScreenPos(o.vertex);
                    o.uv = screenPos;
                    //o.posWorld = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz + v.posWorld;
                    o.posWorld = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz + mul(unity_ObjectToWorld, v.posWorld);
                    o.ray = mul(UNITY_MATRIX_MV, v.vertex).xyz * float3(-1,-1,1);
                    o.ray = lerp(o.ray, v.uv, v.uv.z != 0);
                   
                    return o;
                }
     
                //sampler2D _GrabTexture2;
                sampler2D_float _CameraDepthTexture;
                sampler2D _Texture;
                half _range;
                half _scale;
     
     
                half4 frag (VertOut i) : SV_Target
                {
                   // half4 col = tex2Dproj(_GrabTexture2, i.uv);
     
                    float rawDepth = DecodeFloatRG(tex2Dproj(_CameraDepthTexture, i.uv));
                    float linearDepth = Linear01Depth(rawDepth);
                    half4 scannerCol = half4(0, 0, 0, 0);
                    i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
                    float4 vpos = float4(i.ray * linearDepth, 1);
                    float3 wpos = mul(unity_CameraToWorld, vpos).xyz;
                    float3 wposx = ddx(wpos);
                    float3 wposy = ddy(wpos);
                    float3 normal = normalize(cross(wposy,wposx));
                    float2 coords =  step(abs(normal.y), abs(normal.x))*step(abs(normal.z), abs(normal.x))*((wpos.zy - i.posWorld.zy)*_scale + float2(0.5, 0.5))
                                    +step(abs(normal.x), abs(normal.y))*step(abs(normal.z), abs(normal.y))*((wpos.xz - i.posWorld.xz)*_scale + float2(0.5, 0.5))
                                    +step(abs(normal.x), abs(normal.z))*step(abs(normal.y), abs(normal.z))*((wpos.xy - i.posWorld.xy)*_scale + float2(0.5, 0.5));
     
                    return tex2D(_Texture,TRANSFORM_TEX(coords, _Texture))*step(distance(wpos,i.posWorld), _range);
                    //return float4(1,1,1,1);
                    //return smoothstep(-0.1, 0.1, abs(normal.x)-abs(normal.y))*smoothstep(-0.1, 0.1, abs(normal.x)-abs(normal.z))*tex2D(_Texture, wpos.yz)
                    //  +smoothstep(-0.05, 0.05, abs(normal.y)-abs(normal.x))*smoothstep(-0.05, 0.05, abs(normal.y)-abs(normal.z))*tex2D(_Texture, wpos.xz)
                    //  +smoothstep(-0.05, 0.05, abs(normal.z)-abs(normal.x))*smoothstep(-0.05, 0.05, abs(normal.z)-abs(normal.y))*tex2D(_Texture, wpos.xy);
                }
                ENDCG
            }
        }
    }

