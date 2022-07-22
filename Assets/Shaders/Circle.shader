Shader "Custom/Post/Circle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Multiplier ("Multiplier", Color) = (1,1,1,1)
        _OuterRadius ("Outer Radius", float) = 0.6  //note: values are relative to screen
        _InnerRadius ("Inner Radius", float) = 0.5
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
            fixed4 _Multiplier;
            float _OuterRadius;
            float _InnerRadius;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float screenSize = (_ScreenParams.x + _ScreenParams.y) / 2;
                float x2 = (i.vertex.x / _ScreenParams.x - 0.5) * _ScreenParams.x / screenSize;
                float y2 = (i.vertex.y / _ScreenParams.y - 0.5) * _ScreenParams.y / screenSize;
                float distSqr = x2 * x2 + y2 * y2;
                if(distSqr > _InnerRadius * _InnerRadius && distSqr < _OuterRadius * _OuterRadius)
                    // invert
                    col.rgb = (1 - col.rgb) * _Multiplier;
                return col;
            }
            ENDCG
        }
    }
}
