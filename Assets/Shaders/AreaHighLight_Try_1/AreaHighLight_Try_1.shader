// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Shader "Unlit/AreaHighLight_2"
//Highlights intersections with other objects

Shader "Custom/AreaHighLight_Try_1"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _IntersectionDepth("IntersectionDepth", float) = 0.5

        _RegularColor("Main Color", Color) = (1, 1, 1, .5)
    }

    CGINCLUDE
    // Shared code goes here
    float remap(float val, float in1, float in2, float out1, float out2)
    {
         return out1 + (out2 - out1) * (val - in1) / (in2 - in1);
    }
    ENDCG

        SubShader
        {
            //Tags { "RenderType" = "Opaque" }
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent"  }

            Pass
            {
                Blend SrcAlpha OneMinusSrcAlpha
                ZWrite Off

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                // make fog work
                //#pragma multi_compile_fog

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    //UNITY_FOG_COORDS(1)
                    float4 vertex : SV_POSITION;
                    float4 screenSpace : TEXCOORD1;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float _IntersectionDepth;
                sampler2D _CameraDepthTexture; //Depth Texture

                float4 _RegularColor;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);

                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                    //float4 shift = o.vertex;
                    //shift.xy += 0.5f;

                    //o.screenSpace = ComputeScreenPos(shift);
                    o.screenSpace = ComputeScreenPos(o.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // sample the texture
                    //fixed4 col = tex2D(_MainTex, i.uv);
                    float2 screenSpaceUV = i.screenSpace.xy / i.screenSpace.w;

                    //float4 shift = i.screenSpace;
                    //shift.xy += 0.5f;

                    //float depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(shift)).r);
                    float depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenSpace)).r);

                    //float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenSpaceUV));

                    float reMappedIntersectionDepth = remap(_IntersectionDepth, 0, 1, 1, 0);
                    float subtractedScreenPos = i.screenSpace.a - reMappedIntersectionDepth;

                    float finalAlpha = depth - subtractedScreenPos;
                    //fixed4 col = tex2D(_MainTex, i.uv);

                    fixed4 col = _RegularColor;
                    col.a = finalAlpha;

                    //return fixed4(screenSpaceUV, 0, 1);
                    //return fixed4(depth, 0, 0, 1);
                    return col;
                }

                ENDCG
            }
        }
            FallBack "VertexLit"
}