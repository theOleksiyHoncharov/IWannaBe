﻿Shader "Custom/PartialBillboard2D_WithCamera_OneSprite"
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
            "DisableBatching"="True"
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
                float3 camForward = normalize(mul((float3x3)unity_CameraToWorld, float3(0,0,-1)));
                camForward.y = 0; // проєктуємо на горизонтальну площину, якщо потрібна лише горизонтальна орієнтація
                // 1) Перетворення вершини в світові координати
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 pivot = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;

                // 2) Обчислюємо напрямок до камери (горизонтально)
                float3 toCamHoriz = camForward;

                float dist = length(toCamHoriz);
                if(dist > 0.0001)
                    toCamHoriz = normalize(toCamHoriz);
                else
                    toCamHoriz = float3(0,0,1);

                // 3) Обчислюємо «фронт» об’єкта в світових координатах
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
                float2 uv = i.uv;
                float4 c = tex2D(_MainTex, uv);
                return c;
            }

            ENDCG
        }
    }
}
