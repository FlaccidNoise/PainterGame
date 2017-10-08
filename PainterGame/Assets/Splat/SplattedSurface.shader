Shader "Custom/SplattedSurface" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)		
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	    _Splatmap("Splatmap", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_SplatmapSize ("Splatmap Size", float) = 512.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _Splatmap;
		sampler2D _Noise;

		struct Input {
			float2 uv_MainTex;
			float2 uv2_Splatmap;
			float2 uv2_Noise;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Provided as a parameter for tuning, but for efficiency you can
		// replace this with a constant once you've found the right density.
		float _SplatmapSize;

		// Colour array, so our splatmap only needs a single channel.
		// Alternatively, we could use a full RGBA splatmap.
		static fixed4 _InkColors[7] = {
			fixed4(0, 0, 0, 0),
			fixed4(1, 0.30980, 0, 1),
			fixed4(1, 0.6313725, 0.250980, 1),
			fixed4(1, 0.917647, 0.470588, 1),
			fixed4(0.270588, 1, 0.4862745, 1),
			fixed4(0.00784, 0.75294, 1, 1),
			fixed4(0.478431, 0.2745098, 0.898039, 1)
		};		

		void surf (Input IN, inout SurfaceOutputStandard o) {
			
			// Sample our noise texture, to use to make the effect more organic-looking.
			float noise = tex2D(_Noise, IN.uv2_Noise).r;

			// Apply "domain warping" effect from the beginning - this distorts the grid
			// and lets colours appear to flow & swirl together instead of rigidly cross-fading.
			IN.uv2_Splatmap += 2.0f * (noise - 0.5f) / _SplatmapSize;

			// Construct our local coordinates within the nearest a quad of 4 splatmap texels.
			float4 splatFraction = frac((IN.uv2_Splatmap * _SplatmapSize + 0.5f)).xyxy;

			// Round our texture coordinates so our sampling agrees with our map.
			// (Division can be pre-computed to a multiply when _SplatmapSize is constant)			
			IN.uv2_Splatmap -= splatFraction / _SplatmapSize;

			// Convert local coordinates to a blending weight for each of the four corners.
			// (one minus squared distance from the corner)
			// x --  y -+  z +- w ++
			splatFraction.zw = 1.0f - splatFraction.zw;
			splatFraction = (splatFraction * splatFraction);
			splatFraction = saturate(1.0f - splatFraction.xxzz - splatFraction.ywyw);
			
			// Divide by the total weight to normalize them so they sum to one.
			splatFraction /= dot(splatFraction, 1.0f);

			// Compute texture offsets to nearest samples in splatmap (again, can be pre-computed for constant size)
			float2 offsets = float2(0, 1.0f) / _SplatmapSize;

			// Accumulate ink colours from the four samples, weighted by each corner's blend weight.
			float4 ink = (float4)0;
			ink += splatFraction.x * _InkColors[(int)(tex2D(_Splatmap, IN.uv2_Splatmap + offsets.xx).a * 255.0f)];
			ink += splatFraction.y * _InkColors[(int)(tex2D(_Splatmap, IN.uv2_Splatmap + offsets.xy).a * 255.0f)];
			ink += splatFraction.z * _InkColors[(int)(tex2D(_Splatmap, IN.uv2_Splatmap + offsets.yx).a * 255.0f)];
			ink += splatFraction.w * _InkColors[(int)(tex2D(_Splatmap, IN.uv2_Splatmap + offsets.yy).a * 255.0f)];

			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			
			// Threshold the ink amount to a sharp cutoff,
			// adding noise for a little extra variation at the edges.
			float inkiness = saturate((ink.a - noise + 0.1f) * 100.0f);

			// Where ink is present, blend its colour in.
			if (ink.a > 0.0f) {				
				ink.rgb /= ink.a;
				c = lerp(c, ink, inkiness);
			}

			// Apply surface parameters (possibly different for inky & non-inky parts).
			o.Albedo = c.rgb;			
			o.Metallic = lerp(_Metallic, 0.4f, inkiness);
			o.Smoothness = lerp(_Glossiness, 0.7f, inkiness);
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
