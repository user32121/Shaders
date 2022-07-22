Shader "Custom/Post/EdgeDetection"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Distance ("Distance", Range(-1, 1)) = 0.1
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
            float _Distance;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 colM  = tex2D(_MainTex, i.uv);
                fixed4 colT  = tex2D(_MainTex, i.uv - float2(0, _Distance));
                fixed4 colTR = tex2D(_MainTex, i.uv + float2(_Distance, _Distance));
                fixed4 colR  = tex2D(_MainTex, i.uv + float2(_Distance, 0));
                fixed4 colBR = tex2D(_MainTex, i.uv + float2(_Distance, -_Distance));
                fixed4 colB  = tex2D(_MainTex, i.uv + float2(0, -_Distance));
                fixed4 colBL = tex2D(_MainTex, i.uv + float2(-_Distance, -_Distance));
                fixed4 colL  = tex2D(_MainTex, i.uv + float2(-_Distance, 0));
                fixed4 colTL = tex2D(_MainTex, i.uv + float2(-_Distance, _Distance));
                
                return 8 * colM - colT - colTR - colR - colBR - colB - colBL - colL - colTL;
            }
            ENDCG
        }
    }
}
