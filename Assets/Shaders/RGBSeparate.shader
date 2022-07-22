Shader "Custom/Post/RGBSeparate"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Dist ("Distance", Vector) = (0.1,0.1,0,0)
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
            float4 _Dist;

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
                col.r = tex2D(_MainTex, i.uv - _Dist).r;
                col.b = tex2D(_MainTex, i.uv + _Dist).b;
                return col;
            }
            ENDCG
        }
    }
}
