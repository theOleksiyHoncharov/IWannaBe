Shader "Custom/SlimeWithFakePhysicalShadow"
{
    Properties
    {
        // -------- ��������� ��������� (Slime) Pass --------
        _WaveFrequency("Wave Frequency", Range(0,10)) = 4
        _WaveAmplitude("Wave Amplitude", Range(0,1)) = 0.2
        _WaveSpeed("Wave Speed", Range(0,10)) = 1
        _MinExpand("Min Expand", Range(0,2)) = 0.10

        _SlimeColor("Slime Color", Color) = (0.5,1.0,0.5,1.0)
        _MainTex("Noise Texture (Distort Map)", 2D) = "white" {}
        _DistortionStrength("Base Distortion Strength", Range(0,1)) = 0.05
        _DistortionIntensity("Distortion Intensity", Range(0,5)) = 1

        _FillOrigin("Fill Origin (World)", Vector) = (0,0,0,0)
        _FillDistance("Fill Distance", Range(0,10)) = 1
        _IsFull("Fully Covered", Range(0,1)) = 0

        _FresnelColor("Fresnel Color", Color) = (1,1,1,1)
        _FresnelPower("Fresnel Power", Range(0,8)) = 1
        _FresnelStrength("Fresnel Strength", Range(0,2)) = 1

        _MixFactor("Slime Mix Factor", Range(0,1)) = 0.3
        _DistanceScale("Distance Scale Factor", Range(0,5)) = 1

        // -------- ��������� "�������" �� --------
        _FakeShadowGray("Base Shadow Color (Gray)", Color) = (0,0,0,0.5)
        _ShadowAlpha("Shadow Transparency", Range(0,1)) = 0.4
        _ShadowBlend("SlimeShadow Blend", Range(0,1)) = 0.3
        //   0 => ����� ��� ���
        //   1 => ���� �� = SlimeColor
    }

    SubShader
    {
        // ========== Pass 1: �������� Slime (��������) ==========
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        GrabPass { "_GrabTex" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // ---------- ����� ��� ----------
            struct appdata
            {
                float4 vertex : POSITION;  
                float3 normal : NORMAL;
                float2 uv     : TEXCOORD0;
            };

            // ---------- ������ ��� ----------
            struct v2f
            {
                float4 pos       : SV_POSITION;
                float2 uv        : TEXCOORD0;
                float2 grabUV    : TEXCOORD1;

                float slimeMask  : TEXCOORD2; // 1 => �����, 0 => �
                float waveVal    : TEXCOORD3; // �������� �������� (� ����������� distCam)

                float3 worldPos  : TEXCOORD4;
                float3 worldNormal : TEXCOORD5;
            };

            // ---------- ���� ----------
            sampler2D _MainTex;
            sampler2D _GrabTex;

            float _WaveFrequency;
            float _WaveAmplitude;
            float _WaveSpeed;
            float _MinExpand;

            float4 _SlimeColor;
            float _DistortionStrength;
            float _DistortionIntensity;

            float4 _FillOrigin;
            float _FillDistance;

            float _IsFull;  
            float _MixFactor;
            float _DistanceScale;

            // Fresnel
            float4 _FresnelColor;
            float _FresnelPower;
            float _FresnelStrength;

            // ---------- ��������� ������ (Slime) ----------
            v2f vert(appdata v)
            {
                v2f o;

                // (1) �������� �������
                float3 localPos = v.vertex.xyz;

                // (2) �����
                float waveX = sin(localPos.x * _WaveFrequency + _Time.y * _WaveSpeed);
                float waveZ = sin(localPos.z * _WaveFrequency + _Time.y * _WaveSpeed);
                float waveValRaw = (waveX + waveZ)*0.5 * _WaveAmplitude;

                // (3) ���������
                float distLocal = length(localPos);
                float3 dirLocal = (distLocal>0.0001) ? (localPos/distLocal) : float3(0,1,0);

                float offset = waveValRaw + _MinExpand;
                float distNew = distLocal + offset;
                distNew = max(distNew, 0.0001);
                localPos = dirLocal * distNew;

                // (4) � world space
                float4 worldPos = mul(unity_ObjectToWorld, float4(localPos,1));

                // (4.1) ������� � world space
                float3 localNrm = normalize(v.normal);
                float3 worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, localNrm));

                // (5) ³������ �� ������
                float3 camPos = _WorldSpaceCameraPos.xyz;
                float distCam = distance(camPos, worldPos.xyz);
                float camFactor = _DistanceScale / max(distCam, 0.001);
                float waveVal = waveValRaw * camFactor;

                // (6) Hard boundary
                float distFill = distance(worldPos.xyz, _FillOrigin.xyz);
                float slimeMask = step(distFill, _FillDistance);
                slimeMask = max(slimeMask, _IsFull);

                o.pos = mul(UNITY_MATRIX_VP, worldPos);
                o.uv  = v.uv;
                float4 screenPos = ComputeScreenPos(o.pos);
                o.grabUV = screenPos.xy / screenPos.w;

                o.slimeMask = slimeMask;
                o.waveVal   = waveVal;
                o.worldPos    = worldPos.xyz;
                o.worldNormal = worldNormal;

                return o;
            }

            // ---------- ����������� ������ (Slime) ----------
            fixed4 frag(v2f i) : SV_Target
            {
                // (A) ��� (� GrabPass)
                fixed4 background = tex2D(_GrabTex, i.grabUV);

                // (B) ���� slimeMask=1 => ������ ������� ��������
                float mask01 = saturate(i.slimeMask); // 0 ��� 1 (hard boundary)
                
                // ��� �������� ���� ����� ���, �� slimeMask=1, 
                // ������� baseDistortion �� mask01
                fixed4 noiseSample = tex2D(_MainTex, i.uv);
                float2 baseDistortion = (noiseSample.xy - 0.5) * _DistortionStrength;

                float dynamicFactor = saturate(abs(i.waveVal) * _DistortionIntensity);
                float2 finalOffset = baseDistortion * dynamicFactor * mask01; 
                // ���� mask01=0 => finalOffset=0 => ��� ������� ���� �������

                finalOffset = clamp(finalOffset, -0.05, 0.05);

                float2 slimeUV = i.grabUV + finalOffset;

                // (C) ���� ������
                fixed4 slimeBG = tex2D(_GrabTex, slimeUV);
                fixed4 slimeColor = lerp(slimeBG, _SlimeColor, _MixFactor * mask01);
                // ���� mask01=0 => �� ������� ����

                // (D) ����� �� background � slimeColor �� mask01
                fixed4 finalColor = lerp(background, slimeColor, mask01);

                // (E) Fresnel ���� � ��� ������
                if (mask01>0.5)
                {
                    float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                    float fresnelTerm = 1 - saturate(dot(i.worldNormal, viewDir));
                    fresnelTerm = pow(fresnelTerm, _FresnelPower);
                    finalColor.rgb += _FresnelColor.rgb * fresnelTerm * _FresnelStrength;
                }

                finalColor.a = 1.0;
                return finalColor;
            }
            ENDCG
        }

        // ========== Pass 2: Fake "Physical" Shadow ==========
        Pass
        {
            Name "FakeShadow"
            Tags { "Queue"="Transparent-1" "IgnoreProjector"="True" }
            ZWrite Off
            ZTest LEqual
            Offset -1, -1
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vertShadow
            #pragma fragment fragShadow
            #include "UnityCG.cginc"

            float4 _SlimeColor;
            float4 _FakeShadowGray;  
            float _ShadowAlpha;      
            float _ShadowBlend;

            float4 _FillOrigin;
            float _FillDistance;
            float _IsFull;

            struct appdata_shadow
            {
                float4 vertex : POSITION;
            };

            struct v2f_shadow
            {
                float4 pos : SV_POSITION;
                float slimeMask : TEXCOORD1; // ��������� slimeMask
            };

            // � ����� Pass ��� ���������� slimeMask, ��� �������� ��� ���� ���, �� slimeMask=1
            v2f_shadow vertShadow(appdata_shadow v)
            {
                v2f_shadow o;
                // � world space
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

                // ���������� distFill
                float distFill = distance(worldPos.xyz, _FillOrigin.xyz);
                float sMask = step(distFill, _FillDistance);
                sMask = max(sMask, _IsFull);

                // �������� �� y=0 (��� "�� �� �����")
                worldPos.y = 0; 

                o.pos = mul(UNITY_MATRIX_VP, worldPos);
                o.slimeMask = sMask;
                return o;
            }

            // � ������������ ������, ���� slimeMask=0 => clip
            fixed4 fragShadow(v2f_shadow i) : SV_Target
            {
                if (i.slimeMask < 0.5)
                {
                    clip(-1); // ������� (�� �������) 
                }

                // ������ ������� ���� � ���� ������
                fixed4 mixColor = lerp(_FakeShadowGray, _SlimeColor, _ShadowBlend);
                // ����������� ���������
                mixColor.a = _ShadowAlpha;
                return mixColor;
            }
            ENDCG
        }
    }
}
