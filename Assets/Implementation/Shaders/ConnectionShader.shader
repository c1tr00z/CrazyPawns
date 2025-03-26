Shader "Custom/ConnectionShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Thickness ("Thickness", Range(0, 1)) = 0.07
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
            };
            
            struct v2g
            {
                float4 vertex : POSITION;
            };
            
            struct g2f
            {
                float4 vertex : SV_POSITION;
            };
            
            float _Thickness;
            fixed4 _Color;
            
            v2g vert (appdata v)
            {
                v2g o;
                o.vertex = v.vertex;
                return o;
            }
            
            [maxvertexcount(4)]
            void geom(line v2g input[2], inout TriangleStream<g2f> stream)
            {
                float4 p0 = input[0].vertex;
                float4 p1 = input[1].vertex;
                
                float3 dir = normalize(p1.xyz - p0.xyz);
                float3 perpendicular = normalize(cross(dir, UNITY_MATRIX_IT_MV[2].xyz));
                
                // Толщина линии
                float3 offset = perpendicular * _Thickness;
                
                g2f v[4];
                
                // Генерация квада
                v[0].vertex = UnityObjectToClipPos(p0 + float4(-offset, 0));
                v[1].vertex = UnityObjectToClipPos(p0 + float4(offset, 0));
                v[2].vertex = UnityObjectToClipPos(p1 + float4(-offset, 0));
                v[3].vertex = UnityObjectToClipPos(p1 + float4(offset, 0));
                
                stream.Append(v[0]);
                stream.Append(v[1]);
                stream.Append(v[2]);
                stream.Append(v[3]);
            }
            
            fixed4 frag (g2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}