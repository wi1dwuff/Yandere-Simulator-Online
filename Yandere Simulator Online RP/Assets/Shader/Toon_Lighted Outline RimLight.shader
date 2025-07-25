Shader "Toon/Lighted Outline RimLight" {
	Properties {
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Range(0.002, 0.03)) = 0.005
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Saturation ("Texture Saturation", Range(0, 1)) = 1
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		_RimLightIntensity ("RimLightIntensity", Range(0, 3)) = 1
		_RimCrisp ("RimCrisp", Range(0, 0.5)) = 0.3
		_RimAdditive ("RimAdditive", Range(0, 0.5)) = 0.2
		_RimColor ("Rim Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		UsePass "Toon/Lighted RimLight/FORWARD"
		UsePass "Toon/Basic Outline/OUTLINE"
	}
	Fallback "Toon/Lighted"
}