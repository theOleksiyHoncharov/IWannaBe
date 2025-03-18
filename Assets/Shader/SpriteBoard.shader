Shader "Custom/PartialBillboard2D_WithCamera_OneSprite"
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
            // Прибираємо "DisableBatching"="True" щоб дозволити батчінг
        }
        LOD 100

        // Залишаємо Cull Front, якщо це потрібно саме вам
        Cull Front
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            // Додаємо директиви для інстансингу
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;

            float _Columns;
            float _Rows;

            // Оголошуємо інстансовану властивість _Pivot (позиція екземпляра)
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Pivot)
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
                // Дискретний кут (в градусах) для вибору кадру з атласу
                float vDiscreteAngle : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v2f o;

                // 1) Визначаємо напрям камери по горизонталі
                float3 camForward = normalize(mul((float3x3)unity_CameraToWorld, float3(0,0,-1)));
                camForward.y = 0;
                camForward = normalize(camForward);

                // 2) Перетворюємо вершину в світові координати
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                // 3) Pivot тепер беремо з інстансованої властивості
                float3 pivot = UNITY_ACCESS_INSTANCED_PROP(Props, _Pivot).xyz;

                // 4) «Фронт» об’єкта (0,0,1 у локальному просторі), спроєктований на горизонт
                float3 objFwdWS = mul(unity_ObjectToWorld, float4(0,0,1,0)).xyz;
                objFwdWS.y = 0;
                objFwdWS = normalize(objFwdWS);

                // 5) Обчислюємо кут між objFwdWS та camForward
                float dotF = clamp(dot(objFwdWS, camForward), -1.0, 1.0);
                float angleRad = acos(dotF);
                float3 crossF = cross(objFwdWS, camForward);
                if(crossF.y < 0)
                    angleRad = -angleRad;
                float angleDeg = degrees(angleRad);
                float geoAngle = fmod(angleDeg + 360.0, 360.0);

                // 6) Дискретизація кута (залежно від _Columns * _Rows)
                float totalFrames = _Columns * _Rows;
                float anglePerFrame = 360.0 / totalFrames;
                float frameIndex = floor((geoAngle + anglePerFrame * 0.5) / anglePerFrame);
                frameIndex = fmod(frameIndex, totalFrames);
                float discreteAngle = frameIndex * anglePerFrame;

                // 7) Обертаємо геометрію навколо pivot на discreteAngle
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
                // Передаємо дискретний кут у фрагментний шейдер
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
