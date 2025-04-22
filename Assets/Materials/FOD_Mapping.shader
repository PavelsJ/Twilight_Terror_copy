Shader "Custom/FOD_Mapping"
{
    Properties
    {
        _FogTexture ("Fog Texture", 2D) = "white" {}
        _FogColor ("Fog Color", Color) = (0.1, 0.1, 0.1, 0.7)
        
        _DistortStrength ("Distortion Strength", Range(0, 1)) = 0.5
        _DistortScale ("Distortion Scale", Float) = 1.5
        _TimeValue ("Time", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _FogTexture;
            fixed4 _FogColor;
            float2 _TextureSize;

            float _DistortStrength;
            float _DistortScale;
            float _TimeValue;
            
            StructuredBuffer<float3> _Agents;
            
            int _AgentCount;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float metaballFunction(float2 uv, float invTexSize)
            {
                float intensity = 0.0;
                for (int j = 0; j < _AgentCount; j++)
                {
                    float2 agentPos = _Agents[j].xy;
                    float range = _Agents[j].z * invTexSize;

                    float2 diff = uv - agentPos;
                    float sqrDist = dot(diff, diff);
                    intensity += (range * range) / (sqrDist + 1e-4);
                }
                return intensity;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 invTexSize = 1.0 / _TextureSize;

                // Пикселизация
                float2 pixelCoord = floor(i.uv * _TextureSize) + 0.5;
                float2 snappedUV = pixelCoord * invTexSize;

                fixed4 fogColor = tex2D(_FogTexture, snappedUV);

                // Деформация
                float2 distortUV = snappedUV * _DistortScale + float2(_TimeValue, _TimeValue * 1.123) * 0.08;
                float2 gv = frac(distortUV) - 0.5;
                float2 id = floor(distortUV);
                float minDist = 10.0;

                [unroll]
                for (int y = -1; y <= 1; y++)
                {
                    [unroll]
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 offset = float2(x, y);
                        float2 rand = frac(sin(dot(id + offset, float2(127.1, 311.7))) * 43758.5453);
                        float2 r = offset + rand - gv;
                        float dist = dot(r, r);
                        minDist = min(minDist, dist);
                    }
                }

                float distortion = sin(minDist * 20 + _TimeValue * 0.08) * _DistortStrength;
                float intensity = metaballFunction(snappedUV + distortion * 0.01, invTexSize.x);

                float threshold = 1.0;
                if (intensity > threshold)
                    return fixed4(0, 0, 0, 0);
                else if (intensity > threshold * 0.7)
                    return fixed4(0, 0, 0, 0.8f);

                return lerp(fogColor, _FogColor, _FogColor.a);
            }
            ENDCG
        }
    }
}
