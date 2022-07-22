Shader "Hidden/Diamonds2"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Density("Density", float) = 10
        _Fill("Fill", Range(0, 1)) = 0.5
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

                float2 c = abs((i.uv / _Density + 1) % 2 - 1);

                if (min(abs(c.x) + abs(c.y), abs(c.x - 1) + abs(c.y - 1)) < _Fill)
                    return 1 - col;

                return col;
            }
            ENDCG
        }
    }
}
