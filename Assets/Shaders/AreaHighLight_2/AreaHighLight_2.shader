// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Shader "Unlit/AreaHighLight_2"
//Highlights intersections with other objects

Shader "Custom/AreaHighLight_2"
{
    Properties
    {
        _RegularColor("Main Color", Color) = (1, 1, 1, .5) //Color when not intersecting
        _HighlightColor("Highlight Color", Color) = (1, 1, 1, .5) //Color when intersecting
        _HighlightThresholdMax("Highlight Threshold Max", Float) = 1 //Max difference for intersections
    }
        SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent"  }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            //Cull Off

            CGPROGRAM
            //#pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float4 projPos : TEXCOORD1; //Screen position of pos
            };

            sampler2D _CameraDepthTexture; //Depth Texture
            float4 _RegularColor;
            float4 _HighlightColor;
            float _HighlightThresholdMax;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.projPos = ComputeScreenPos(o.pos);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target            //COLOR
            {
                float4 finalColor = _RegularColor;

                float2 screenSpaceUV = i.projPos.xy / i.projPos.w;

                //Get the distance to the camera from the depth buffer for this point
                float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenSpaceUV));
                //float sceneZ = LinearEyeDepth(tex2Dproj(_CameraDepthTexture,
                //                                         UNITY_PROJ_COORD(i.projPos)).r);

                //Actual distance to the camera
                float partZ = i.projPos.w;

                //If the two are similar, then there is an object intersecting with our object
                float diff = (abs(sceneZ - partZ)) /
                    _HighlightThresholdMax;

                /*if (diff <= 1)
                {
                    finalColor = lerp(_HighlightColor,
                                      _RegularColor,
                                      float4(diff, diff, diff, diff));
                }

                fixed4 c;
                c.r = finalColor.r;
                c.g = finalColor.g;
                c.b = finalColor.b;
                c.a = finalColor.a;*/

                //c = _RegularColor;
                finalColor.a = diff;

                return finalColor;
                //return c;
            }

            ENDCG
        }
    }
        FallBack "VertexLit"
}