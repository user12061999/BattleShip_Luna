Shader "Luna/Water"
{
    Properties
    {
        _Color ("Water Color", Color) = (0.0, 0.5, 0.8, 0.6)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Cubemap ("Environment Cubemap", Cube) = "_Skybox" {}
        _ReflectionStrength ("Reflection Strength", Range(0.0, 1.0)) = 0.5
        _WaveSpeed ("Wave Speed", Range(0.01, 5.0)) = 1.0
        _WaveHeight ("Wave Height", Range(0.01, 0.5)) = 0.1
        _FresnelPower ("Fresnel Power", Range(0.5, 10.0)) = 3.0
        _EmissionColor ("Emission Color", Color) = (0.05, 0.15, 0.3, 0.0)
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
        LOD 300

        // Dùng ZWrite để tránh hiện tượng trong suốt chồng lấn
        Pass {
            Name "BASE"
            Tags { "LightMode" = "Always" }

            ZWrite On
            ColorMask 0
            Blend SrcAlpha OneMinusSrcAlpha
        }

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0
        #pragma shader_feature _NORMALMAP

        sampler2D _MainTex;
        sampler2D _BumpMap;
        samplerCUBE _Cubemap;
        fixed4 _Color;
        fixed4 _EmissionColor;
        float _ReflectionStrength;
        float _WaveSpeed;
        float _WaveHeight;
        float _FresnelPower;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 worldNormal;
            float3 viewDir;
            float3 worldPos;
            INTERNAL_DATA
        };

        // Tạo sóng nhẹ bằng sin theo thời gian và vị trí
        float2 WaveUV(float2 uv, float speed, float height)
        {
            uv.x += sin((uv.y + _Time.y * speed) * 5) * height;
            uv.y += cos((uv.x + _Time.y * speed) * 5) * height;
            return uv;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Cuộn UV + thêm hiệu ứng sóng
            float2 uvMain = WaveUV(IN.uv_MainTex, _WaveSpeed, _WaveHeight);
            float2 uvBump = WaveUV(IN.uv_BumpMap, _WaveSpeed * 1.2, _WaveHeight * 1.2);

            // Màu nền
            fixed4 c = tex2D(_MainTex, uvMain) * _Color;
            o.Albedo = c.rgb;

            // Normal map (nếu có)
            #ifdef _NORMALMAP
                o.Normal = UnpackScaleNormal(tex2D(_BumpMap, uvBump), 1.0);
            #endif

            // Phản xạ môi trường
            float3 worldViewDir = normalize(UnityWorldSpaceViewDir(IN.worldPos));
            float3 worldNormalDir = WorldNormalVector(IN, o.Normal);
            float3 reflectionDir = reflect(-worldViewDir, worldNormalDir);
            fixed4 reflColor = texCUBE(_Cubemap, reflectionDir);
            float fresnel = 1.0 - saturate(dot(worldViewDir, worldNormalDir));
            fresnel = pow(fresnel, _FresnelPower);
            o.Emission = lerp(_EmissionColor.rgb, reflColor.rgb * _ReflectionStrength, fresnel);

            // Trong suốt
            o.Alpha = c.a;
        }
        ENDCG
    }

    FallBack "Transparent/Diffuse"
CustomEditor "ShaderGUI"
}