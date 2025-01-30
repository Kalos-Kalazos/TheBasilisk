Shader "Custom/TextGlitchShader"
{
    Properties
    {
        _Color ("Font Color", Color) = (1, 1, 1, 1)
        _GlitchAmount ("Glitch Amount", Range(0, 1)) = 0.2
        _TimeSpeed ("Time Speed", Range(0, 10)) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Overlay" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            uniform float _GlitchAmount;
            uniform float _TimeSpeed;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                // Modificar las coordenadas UV para crear el efecto glitch
                o.uv = v.uv + float2(sin(_Time.y * _TimeSpeed) * _GlitchAmount, cos(_Time.y * _TimeSpeed) * _GlitchAmount);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Aquí tomamos el color de la propiedad y aplicamos a la superficie
                return _Color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
