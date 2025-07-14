Shader "Unlit/Transparent Colored (Packed) (TextureClip)" {
	Properties {
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
	}
	SubShader {
		LOD 200
		Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass {
			LOD 200
			Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Cull Off
			Offset -1, -1
			Fog {
				Mode 0
			}
			GpuProgramID 22220
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float4 color : COLOR0;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float4 _MainTex_ST;
			// $Globals ConstantBuffers for Fragment Shader
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			
			// Keywords: 
			v2f vert(appdata_full v)
			{
                v2f o;
                float4 tmp0;
                float4 tmp1;
                tmp0 = v.vertex.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
                tmp0 = unity_ObjectToWorld._m00_m10_m20_m30 * v.vertex.xxxx + tmp0;
                tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * v.vertex.zzzz + tmp0;
                tmp0 = tmp0 + unity_ObjectToWorld._m03_m13_m23_m33;
                tmp1 = tmp0.yyyy * unity_MatrixVP._m01_m11_m21_m31;
                tmp1 = unity_MatrixVP._m00_m10_m20_m30 * tmp0.xxxx + tmp1;
                tmp1 = unity_MatrixVP._m02_m12_m22_m32 * tmp0.zzzz + tmp1;
                o.position = unity_MatrixVP._m03_m13_m23_m33 * tmp0.wwww + tmp1;
                o.color = v.color;
                o.texcoord1.xy = v.vertex.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.texcoord.xy = v.texcoord.xy;
                return o;
			}
			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                tmp0 = inp.color - float4(0.5, 0.5, 0.5, 0.5);
                tmp0 = ceil(tmp0);
                tmp1 = tex2D(0, inp.texcoord.xy);
                tmp1.xy = tmp0.xy * tmp1.xy;
                tmp1.x = tmp1.y + tmp1.x;
                tmp1.x = tmp1.z * tmp0.z + tmp1.x;
                tmp1.x = tmp1.w * tmp0.w + tmp1.x;
                tmp0 = tmp0 * float4(0.51, 0.51, 0.51, 0.51) + -inp.color;
                tmp0 = saturate(tmp0 * float4(-2.040816, -2.040816, -2.040816, -2.040816));
                tmp1.yz = inp.texcoord1.xy * float2(0.5, 0.5) + float2(0.5, 0.5);
                tmp2 = tex2D(1, tmp1.yz);
                tmp0.w = tmp0.w * tmp2.w;
                o.sv_target.xyz = tmp0.xyz;
                o.sv_target.w = tmp1.x * tmp0.w;
                return o;
			}
			ENDCG
		}
	}
}