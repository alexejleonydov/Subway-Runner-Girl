Shader "Hidden/Unlit/Sprite Glitter Colored 1" {
	Properties {
		_MainTex ("RGB", 2D) = "black" {}
		_FlowLightTex ("FlowLight Texture", 2D) = "white" {}
		_FlowLightPower ("FlowLightPower", Float) = 1
		_IsOpenFlowLight ("IsOpenFlowLight", Float) = 0
		_FlowLightOffset ("FlowLight Offset", Float) = 0
		_WidthRate ("Sprite Width / Altas Width", Float) = 0
		_HeightRate ("Sprite Height / Altas Height", Float) = 0
		_OffsetXRate ("Sprite Offset X / Altas Width", Float) = 0
		_OffsetYRate ("Sprite Offset Y / Altas Height", Float) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}