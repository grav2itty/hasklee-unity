Shader "Custom/Cursor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Overlay" }
        LOD 100
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            // make fog work
            // #pragma multi_compile_fog
            #pragma target 4.5

            #include "UnityCG.cginc"

            struct appdata
            {
                // uint : POSITION;
                float4 vertex : POSITION;
                // float2 uv : TEXCOORD0;
            };

            struct v2g
            {
                float4 vertex : SV_POSITION;
                uint vid : VERTEXID;
            };

            // struct v2f
            // {
            //   float4 vertex : SV_POSITION;
            //   float4 color : COLOR;
            // };

            struct g2f
            {
                // float2 uv : TEXCOORD0;
                // UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            // float4 _MainTex_ST;
            float4 position;

            uint icount;

            // StructuredBuffer<float4> ccbuffer;
            // StructuredBuffer<float4> sbuffer;
            // StructuredBuffer<float4x4> mbuffer;

            // v2f vert (appdata v, uint vid : SV_VertexID, uint iid : SV_InstanceID)
            // {
            //   v2f o;
            //   // o.vertex = UnityObjectToClipPos(sbuffer[iid * icount + vid]);
            //   float4 vv = sbuffer[iid * icount + vid];
            //   vv.w = 1.0f;
            //   float4x4 matr = mbuffer[iid];
            //   // o.vertex = mul(UNITY_MATRIX_VP, mul(matr, vv));
            //   o.vertex = mul(UNITY_MATRIX_VP, mul(matr, vv));
            //   o.color = ccbuffer[iid];
            //   return o;
            // }

            v2g vert (appdata v, uint vid : SV_VertexID)
            {
                v2g o;
                // o.vertex = v.vertex;
                o.vertex = position;
                o.vid = vid;
                // o.uv = v.uv;
                return o;
            }

            [maxvertexcount(3)]
            void geom(point v2g IN[1], inout TriangleStream<g2f> triStream)
            {
                g2f o;
                o.vertex = IN[0].vertex;
                triStream.Append(o);
                o.vertex = IN[0].vertex + float4(0.01,0,0,0);
                // o.vertex = IN[0].vertex + float4(0,0.1,0,0);
                triStream.Append(o);
                o.vertex = IN[0].vertex + float4(0,0.01,0,0);
                // o.vertex = IN[0].vertex + float4(0.1,0,0,0);
                triStream.Append(o);

                // for(int i = 0; i < 3; i++)
                // {
                //     // o.vertex = UnityObjectToClipPos(IN[i].vertex);
                //     o.vertex = UnityObjectToClipPos(sbuffer[IN[0].vid + i]);
                //     // UNITY_TRANSFER_FOG(o,o.vertex);
                //     // o.uv = TRANSFORM_TEX(IN[i].uv, _MainTex);
                //     triStream.Append(o);
                // }

                triStream.RestartStrip();
            }

            fixed4 frag (g2f i) : SV_Target
            {
                // sample the texture
                // fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 col = fixed4(1,1,1,1);
                // fixed4 col = i.color;
                // apply fog
                // UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
