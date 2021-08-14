//
// Author:
//   Based on the Unity3D built-in shaders
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2013 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

Shader "Decal/Colored/Shadow/Transparent Diffuse Colored Shadow" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}

	SubShader {
		Tags {
			"Queue" = "AlphaTest"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		Offset -1, -1

		CGPROGRAM
		#pragma exclude_renderers flash
		#pragma target 3.0
		#pragma surface surf Lambert fullforwardshadows dualforward decal:blend exclude_path:prepass

		sampler2D _MainTex;
		fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
			float4 color: Color;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			UNITY_INITIALIZE_OUTPUT (SurfaceOutput, o);
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color  * IN.color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}

	Fallback "Decal/Transparent VertexLit"
}
