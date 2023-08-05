// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Shader "Unlit/AreaHighLight_2"
//Highlights intersections with other objects

Shader "Custom/AHL_Try_2"
{
    Properties
    {
        _Offset("Offset", float) = 0.5

        _RegularColor("Main Color", Color) = (1, 1, 1, .5)
        _HighlightColor("Highlight Color", Color) = (1, 1, 1, .5) //Color when intersecting
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
                    float4 vertex : SV_POSITION;
                    float4 screenSpace : TEXCOORD1;
                };

                float4 _MainTex_ST;
                float _Offset;
                sampler2D _CameraDepthTexture; //Depth Texture

                float4 _RegularColor;
                float4 _HighlightColor;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);

                    o.screenSpace = ComputeScreenPos(o.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // sample the texture
                    //fixed4 col = tex2D(_MainTex, i.uv);
                    float4 finalColor = _RegularColor;
                    float2 screenSpaceUV = i.screenSpace.xy / i.screenSpace.w;

                    float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenSpaceUV));
                    //float depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenSpace)).r);
                    //float depth = Linear01Depth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenSpace)).r);

                    //Actual distance to the camera
                    float screenSpaceZ = i.screenSpace.w;

                    //float subtractedOffset = screenSpaceZ - _Offset;

                    //float diff = abs(depth - screenSpaceZ);
                    //float diff = abs(depth - screenSpaceZ) / _Offset;
                    //float diff = abs(depth - subtractedOffset);
                     
                    //float diff = depth - screenSpaceZ;
                    float diff = depth - screenSpaceZ / _Offset;            //Works perfectly
                    //float diff = depth - subtractedOffset;                    //Can work
                    
                    float oneMinusDiff = 1 - diff;
                    float smoothDiff = smoothstep(0, 1, oneMinusDiff);

                    finalColor.a = smoothDiff;

                    return finalColor;
                    //return col;
                }/**/

                ENDCG
            }
        }
            FallBack "VertexLit"
}