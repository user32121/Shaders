Shader "Unlit/Rainbow"
{
    Properties
    {
        _Frequency ("Frequency", Vector) = (1,1,1,0)
        _Speed ("Speed", float) = 1
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
            #pragma multi_compile_fog

            #include "UnityCG.cginc"


            fixed4 hsvToRgb(fixed h, fixed s, fixed v) {
                fixed r=0,g=0,b=0;

                fixed i = floor(h * 6);
                fixed f = h * 6 - i;
                fixed p = v * (1 - s);
                fixed q = v * (1 - f * s);
                fixed t = v * (1 - (1 - f) * s);

                switch (i % 6) {
                    case 0: r = v, g = t, b = p; break;
                    case 1: r = q, g = v, b = p; break;
                    case 2: r = p, g = v, b = t; break;
                    case 3: r = p, g = q, b = v; break;
                    case 4: r = t, g = p, b = v; break;
                    case 5: r = v, g = p, b = q; break;
                }

                return fixed4(r, g, b, 1);
            }


            fixed4 _Frequency;
            fixed _Speed;

            struct vertIn
            {
                float4 vertex : POSITION;
            };

            struct fragIn
            {
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION0;
                float4 vertex2 : POSITION1;
            };

            fragIn vert (vertIn i)
            {
                fragIn o;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.vertex2 = mul(unity_ObjectToWorld, i.vertex);
                float4 clipVertex = UnityObjectToClipPos(i.vertex);
                UNITY_TRANSFER_FOG(o, clipVertex);
                return o;
            }

            fixed4 frag(fragIn i) : SV_Target
            {
                // color based on position
                i.vertex2 *= _Frequency;
                fixed4 col = hsvToRgb(fmod(i.vertex2.x + i.vertex2.y + i.vertex2.z + _Time.x*_Speed + 1000, 1), 1, 1);
                col.a = 1;
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
