Shader "Hidden/Diamonds1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Density ("Density", float) = 10
        _Fill ("Fill", Range(0, 1)) = 0.5
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
            float _Density;
            float _Fill;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                    
                float2 c = (floor(i.uv * _Density) + 0.5) / _Density;
                if (abs(i.uv.x - c.x) + abs(i.uv.y - c.y) < 1 / _Density * _Fill)
                    return 1 - col;

                return col;
            }
            ENDCG
        }
    }
}
