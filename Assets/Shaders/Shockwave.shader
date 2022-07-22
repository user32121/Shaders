Shader "Hidden/Shockwave"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _InnerRadius ("Inner Radius", Range(0,2)) = 0.4
        _OuterRadius ("Outer Radius", Range(0,2)) = 0.5
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
            float _InnerRadius;
            float _OuterRadius;

            fixed4 frag(v2f i) : SV_Target
            {
                float screenSize = (_ScreenParams.x + _ScreenParams.y) / 2;
                float distX = (i.uv.x - 0.5) * _ScreenParams.x / screenSize;
                float distY = (i.uv.y - 0.5) * _ScreenParams.y / screenSize;
                float dist = sqrt(distX * distX + distY * distY) * 2;

                if (dist >= _InnerRadius && dist <= _OuterRadius)
                {
                    float dist2 = (dist - _InnerRadius) / (_OuterRadius - _InnerRadius);
                    float2 uv2 = i.uv + float2(distX, distY) * (dist2 - pow(dist2, 5));
                    return tex2D(_MainTex, uv2);
                }
                else
                    return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
