﻿Shader "Hidden/Glitch1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
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
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            int _RectangleLength = 12;
            float _Rectangles[12];

            fixed4 frag (v2f i) : SV_Target
            {
                for (int it = 0; it < _RectangleLength; it += 4)
                    if (i.uv.x >= _Rectangles[it] && i.uv.y >= _Rectangles[it + 1] && i.uv.x <= _Rectangles[it] + _Rectangles[it + 2] && i.uv.y <= _Rectangles[it + 1] + _Rectangles[it + 3])
                        return fixed4(0,0,0,1);

                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
