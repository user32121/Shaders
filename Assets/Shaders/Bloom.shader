Shader "Custom/Post/Bloom"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Dist ("Distance", float) = 1
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

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Dist;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                for (float x = -_Dist; x <= _Dist; x += 1)
                    for (float y = -_Dist; y <= _Dist; y += 1)
                        col = max(col, (tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x*x, _MainTex_TexelSize.y*y))) * pow(0.9, abs(x)+abs(y)));
                return col;
            }
            ENDCG
        }
    }
}
