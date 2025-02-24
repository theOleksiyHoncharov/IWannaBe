// Upgrade NOTE: commented out 'float3 _WorldSpaceCameraPos', a built-in variable
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/PartialBillboard2D_WithCamera"
{
    Properties
    {
        _MainTex ("Sprite Atlas", 2D) = "white" {}
        _Columns ("Columns", Range(1,8)) = 4
        _Rows ("Rows", Range(1,8)) = 2

        // Параметр для анімації/вибору кадру (за бажанням)
        _Rotation("Rotation", Range(0,360)) = 0
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

            // Підключаємо базові include-файли Unity
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;

            float _Columns;
            float _Rows;
            float _Rotation; // параметр для вибору кадру

            // Камера в світових координатах
            // float3 _WorldSpaceCameraPos;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
                // Передаємо розрахований кут (в градусах) між напрямком об'єкта та камерою
                float vRelativeAngle : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                // 1) Перетворюємо вершину в світові координати
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                // Визначаємо pivot – центр об'єкта (можна змінити при потребі)
                float3 pivot = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;

                // 2) Обчислюємо напрямок від pivot до камери
                float3 toCam = _WorldSpaceCameraPos - pivot;
                // Для горизонтальних розрахунків обнуляємо y
                float3 toCamHoriz = toCam;
                toCamHoriz.y = 0;
                float dist = length(toCamHoriz);
                if(dist > 0.0001)
                    toCamHoriz = normalize(toCamHoriz);
                else
                    toCamHoriz = float3(0,0,1);

                // 3) Обчислюємо «фронт» об’єкта: локальний (0,0,1) у світових координатах,
                //    але від pivot (так, щоб не брати вплив позиції pivot)
                float3 objFwdWS = (mul(unity_ObjectToWorld, float4(0,0,1,0)).xyz);
                // Відкидаємо вертикаль для розрахунку кута
                objFwdWS.y = 0;
                float lenF = length(objFwdWS);
                if(lenF > 0.0001)
                    objFwdWS = normalize(objFwdWS);
                else
                    objFwdWS = float3(0,0,1);

                // 4) Обчислюємо кут між напрямом об'єкта та камерою (в радіанах)
                float dotF = dot(objFwdWS, toCamHoriz);
                dotF = clamp(dotF, -1.0, 1.0);
                float angleRad = acos(dotF);
                // Визначаємо знак кута через cross-продукт (за y-компонентою)
                float3 crossF = cross(objFwdWS, toCamHoriz);
                if(crossF.y < 0)
                    angleRad = -angleRad;

                // Перетворюємо кут у градуси
                float angleDeg = degrees(angleRad);
                // Передаємо цей кут у фрагментний шейдер для вибору кадру
                o.vRelativeAngle = angleDeg;

                // 5) Робимо частковий білборд: обертаємо геометрію так, щоб вона завжди була «поворотна» по горизонталі до камери,
                //    але зберігаємо вертикальний нахил з transform
                float3 shiftPos = worldPos - pivot;
                float c = cos(angleRad);
                float s = sin(angleRad);
                float3 newPos;
                newPos.x =  c * shiftPos.x + s * shiftPos.z;
                newPos.y =  shiftPos.y;   // вертикаль залишається без змін
                newPos.z = -s * shiftPos.x + c * shiftPos.z;
                worldPos = newPos + pivot;

                // 6) Стандартна трансформація у кліп-простір
                o.vertex = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0));
                // Передаємо базові UV
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Обчислюємо ефективний кут, враховуючи як _Rotation, так і кут від камери (vRelativeAngle)
                float effectiveAngle = fmod(_Rotation + i.vRelativeAngle + 360.0, 360.0);

                // Рахуємо кадри з атласу
                float totalFrames = _Columns * _Rows;
                float anglePerFrame = 360.0 / totalFrames;
                // Обираємо кадр, додаючи половину сектора для «центрованості»
                float frameIndex = floor((effectiveAngle + anglePerFrame * 0.5) / anglePerFrame);
                frameIndex = fmod(frameIndex, totalFrames);

                float row = floor(frameIndex / _Columns);
                float col = fmod(frameIndex, _Columns);

                float2 uvScale  = float2(1.0 / _Columns, 1.0 / _Rows);
                float2 uvOffset = float2(col / _Columns, row / _Rows);

                // Коригуємо UV для вибору потрібного кадру
                float2 uv = i.uv * uvScale + uvOffset;
                float4 c = tex2D(_MainTex, uv);
                return c;
            }
            ENDCG
        }
    }
}
