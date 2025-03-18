Shader "Custom/PartialBillboard2D_WithCamera_InstancedZSort"
{
    Properties
    {
        _MainTex ("Sprite Atlas", 2D) = "white" {}
        _Columns ("Columns", Range(1,8)) = 4
        _Rows ("Rows", Range(1,8)) = 2
    }
    SubShader
    {
        Tags {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }
        LOD 100

        Cull Front
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;

            float _Columns;
            float _Rows;

            // Додаємо інстансовані змінні
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Pivot)
                UNITY_DEFINE_INSTANCED_PROP(float, _DepthLevel) // Нове значення глибини
            UNITY_INSTANCING_BUFFER_END(Props)

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
                float vDiscreteAngle : TEXCOORD1;
                float depthOffset : TEXCOORD2; // Для Z-Ordering
            };

            v2f vert (appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v2f o;

                float3 camForward = normalize(mul((float3x3)unity_CameraToWorld, float3(0,0,-1)));
                camForward.y = 0;
                camForward = normalize(camForward);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 pivot = UNITY_ACCESS_INSTANCED_PROP(Props, _Pivot).xyz;
                float depthLevel = UNITY_ACCESS_INSTANCED_PROP(Props, _DepthLevel); 

                float3 objFwdWS = mul(unity_ObjectToWorld, float4(0,0,1,0)).xyz;
                objFwdWS.y = 0;
                objFwdWS = normalize(objFwdWS);

                float dotF = clamp(dot(objFwdWS, camForward), -1.0, 1.0);
                float angleRad = acos(dotF);
                float3 crossF = cross(objFwdWS, camForward);
                if(crossF.y < 0)
                    angleRad = -angleRad;
                float angleDeg = degrees(angleRad);
                float geoAngle = fmod(angleDeg + 360.0, 360.0);

                float totalFrames = _Columns * _Rows;
                float anglePerFrame = 360.0 / totalFrames;
                float frameIndex = floor((geoAngle + anglePerFrame * 0.5) / anglePerFrame);
                frameIndex = fmod(frameIndex, totalFrames);
                float discreteAngle = frameIndex * anglePerFrame;

                float discreteAngleRad = radians(discreteAngle);
                float c = cos(discreteAngleRad);
                float s = sin(discreteAngleRad);

                float3 shiftPos = worldPos - pivot;
                float3 newPos;
                newPos.x =  c * shiftPos.x + s * shiftPos.z;
                newPos.y =  shiftPos.y;
                newPos.z = -s * shiftPos.x + c * shiftPos.z;
                worldPos = newPos + pivot;

                // **Z-Sorting** (чим більше depthLevel, тим далі від камери)
                worldPos.z += depthLevel * 0.01; 

                o.vertex = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vDiscreteAngle = discreteAngle;
                o.depthOffset = depthLevel;

                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float totalFrames = _Columns * _Rows;
                float anglePerFrame = 360.0 / totalFrames;
                float frameIndex = floor((i.vDiscreteAngle + anglePerFrame * 0.5) / anglePerFrame);
                frameIndex = fmod(frameIndex, totalFrames);
                frameIndex += totalFrames / 2.0;
                frameIndex = fmod(frameIndex, totalFrames);

                float row = floor(frameIndex / _Columns);
                float col = fmod(frameIndex, _Columns);

                float2 uvScale  = float2(1.0 / _Columns, 1.0 / _Rows);
                float2 uvOffset = float2(col / _Columns, row / _Rows);

                float2 uv = i.uv * uvScale + uvOffset;
                float4 c = tex2D(_MainTex, uv);

                return c;
            }
            ENDCG
        }
    }
}
