Shader "Custom/PartialBillboard2D_WithCamera"
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

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;

            float _Columns;
            float _Rows;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
                // Дискретний кут (в градусах) для вибору кадру з атласу,
                // який також використовується для обертання геометрії
                float vDiscreteAngle : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;

                // 1) Перетворення вершини в світові координати
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 pivot = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;

                // 2) Обчислюємо напрямок від pivot до камери (горизонтально)
                float3 toCam = _WorldSpaceCameraPos - pivot;
                float3 toCamHoriz = toCam;
                toCamHoriz.y = 0;
                float dist = length(toCamHoriz);
                if(dist > 0.0001)
                    toCamHoriz = normalize(toCamHoriz);
                else
                    toCamHoriz = float3(0,0,1);

                // 3) Обчислюємо «фронт» об’єкта в світових координатах
                float3 objFwdWS = mul(unity_ObjectToWorld, float4(0,0,1,0)).xyz;
                objFwdWS.y = 0;
                float lenF = length(objFwdWS);
                if(lenF > 0.0001)
                    objFwdWS = normalize(objFwdWS);
                else
                    objFwdWS = float3(0,0,1);

                // 4) Обчислюємо кут між напрямком об’єкта та камерою (в градусах)
                float dotF = clamp(dot(objFwdWS, toCamHoriz), -1.0, 1.0);
                float angleRad = acos(dotF);
                float3 crossF = cross(objFwdWS, toCamHoriz);
                if(crossF.y < 0)
                    angleRad = -angleRad;
                float angleDeg = degrees(angleRad);
                // Приводимо кут до діапазону [0,360)
                float geoAngle = fmod(angleDeg + 360.0, 360.0);

                // 5) Розбиваємо кут на дискретні кроки, залежно від кількості кадрів в атласі
                float totalFrames = _Columns * _Rows;
                float anglePerFrame = 360.0 / totalFrames;
                float frameIndex = floor((geoAngle + anglePerFrame * 0.5) / anglePerFrame);
                frameIndex = fmod(frameIndex, totalFrames);
                float discreteAngle = frameIndex * anglePerFrame;

                // 6) Обертання геометрії за дискретним кутом (для уникнення ефекту "слайду")
                float discreteAngleRad = radians(discreteAngle);
                float c = cos(discreteAngleRad);
                float s = sin(discreteAngleRad);
                float3 shiftPos = worldPos - pivot;
                float3 newPos;
                newPos.x =  c * shiftPos.x + s * shiftPos.z;
                newPos.y =  shiftPos.y;
                newPos.z = -s * shiftPos.x + c * shiftPos.z;
                worldPos = newPos + pivot;

                o.vertex = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // Передаємо дискретний кут для вибору кадру у фрагментному шейдері
                o.vDiscreteAngle = discreteAngle;

                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // 7) Вибір кадру із атласу на основі дискретного кута
                float totalFrames = _Columns * _Rows;
                float anglePerFrame = 360.0 / totalFrames;
                float frameIndex = floor((i.vDiscreteAngle + anglePerFrame * 0.5) / anglePerFrame);
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
