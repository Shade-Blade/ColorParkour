Shader "Custom/RainbowShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		TimeFactor("Time Factor", Float) = 1.5
		XFactor("X Factor", Float) = 30
		YFactor("Y Factor", Float) = -30
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

		float TimeFactor;
		float XFactor;
		float YFactor;

        half _Glossiness;
        half _Metallic;
        fixed4 color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float hue = _Time.y * TimeFactor + (IN.uv_MainTex.x * XFactor + IN.uv_MainTex.y * YFactor);
			while (hue < 0) {
				hue += 3;
			}
			while (hue > 3) {
				hue -= 3;
			}

			//0-0.5 (r +g)
			//0.5-1 (r- g)
			//1-1.5 (g +b)
			//1.5-2 (g- b)
			//2-2.5 (b r+)
			//2.5-3 (b- r)
			if (hue < 0.5) {
				color.r = 1;
				color.g = hue * 2;
				color.b = 0;
			}
			else if (hue > 0.5 && hue < 1.0) {
				color.r = 2 - (hue * 2);
				color.g = 1;
				color.b = 0;
			}
			else if (hue > 1.0 && hue < 1.5) {
				color.r = 0;
				color.g = 1;
				color.b = (hue * 2) - 2;
			}
			else if (hue > 1.5 && hue < 2.0) {
				color.r = 0;
				color.g = 4 - (hue * 2);
				color.b = 1;
			}
			else if (hue > 2.0 && hue < 2.5) {
				color.r = (hue * 2) - 4;
				color.g = 0;
				color.b = 1;
			}
			else if (hue > 2.5 && hue < 3.0) {
				color.r = 1;
				color.g = 0;
				color.b = 6 - (hue * 2);
			}

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * color;
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
