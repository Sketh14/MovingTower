// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SimpleDepthCheckShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _IntersectionDepth ("IntersectionDepth", float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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

            v2f vert (appdata v)
            {
                v2f o;             
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenSpace = ComputeScreenPos(o.vertex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float2 screenSpaceUV = i.screenSpace.xy / i.screenSpace.w;

                //float depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenSpace)).r);
                float depth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenSpaceUV));

                //float subtractedScreenPos = i.screenSpace.a - _IntersectionDepth;

                //float finalAlpha = depth - subtractedScreenPos;
                //fixed4 col = tex2D(_MainTex, i.uv);

                //fixed4 col = _RegularColor;
                //col.a = finalAlpha;

                //return fixed4(screenSpaceUV, 0, 1);
                return fixed4(depth, 0, 0, 1);
                //return col;
            }
            ENDCG
        }
    }
}
