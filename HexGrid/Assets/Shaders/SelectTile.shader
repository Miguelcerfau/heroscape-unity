Shader "Custom/SelectTile"
{
    Properties
    {
        _Color ("Color0", Color) = (1,1,1,1)
        _Color1 ("Color1", Color) = (1,1,1,1)
        _TeamR ("TeamR", Color) = (1,0,0,1)
        _TeamB ("TeamB", Color) = (0,0,1,1)
        _isStart("isStart", Integer) = 0 //0 means no team, 1 means teamRed, 2 means teamBlue
        _Selected("Sel", Integer) = 0
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _Color1;
        fixed4 _TeamB;
        fixed4 _TeamR;
        int _isStart;
        bool _Selected;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            if(!_Selected){
                if(_isStart == 1) c *= lerp (fixed4(1,1,1,1), _TeamR, 0.9);
                else if(_isStart == 2) c *= lerp (fixed4(1,1,1,1), _TeamB, 0.9);
                else c *= _Color;
            }
            else{
                if(_isStart == 1) c *= lerp(lerp (fixed4(1,1,1,1), _TeamR, 0.9), _Color1, 0.7);
                else if(_isStart == 2) c *= lerp(lerp (fixed4(1,1,1,1), _TeamB, 0.9), _Color1, 0.7);
                else c *= lerp(_Color, _Color1, 0.7);
            }
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
