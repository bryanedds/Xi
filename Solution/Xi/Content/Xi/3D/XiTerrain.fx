#include "XiShadowReceiverBase.fx"

// terrain height max NOTE: assumes the terrain's bottom is at 0
float xHeightMax;

// terrain diffuse maps
texture xDiffuseMap0;
texture xDiffuseMap1;
texture xDiffuseMap2;
texture xDiffuseMap3;

sampler xDiffuseMapSampler0 = sampler_state
{
	texture = <xDiffuseMap0>;
	minFilter = LINEAR;
	magFilter = LINEAR;
	mipFilter = LINEAR;
	addressU = MIRROR;
	addressV = MIRROR;
};

sampler xDiffuseMapSampler1 = sampler_state
{
	texture = <xDiffuseMap1>;
	minFilter = LINEAR;
	magFilter = LINEAR;
	mipFilter = LINEAR;
	addressU = MIRROR;
	addressV = MIRROR;
};

sampler xDiffuseMapSampler2 = sampler_state
{
	texture = <xDiffuseMap2>;
	minFilter = LINEAR;
	magFilter = LINEAR;
	mipFilter = LINEAR;
	addressU = MIRROR;
	addressV = MIRROR;
};

sampler xDiffuseMapSampler3 = sampler_state
{
	texture = <xDiffuseMap3>;
	minFilter = LINEAR;
	magFilter = LINEAR;
	mipFilter = LINEAR;
	addressU = MIRROR;
	addressV = MIRROR;
};

void NormalVertexShader(
	float4 inPosition : POSITION0,
	float3 inNormal : NORMAL0,
	float4 inTexCoords : TEXCOORD0,
	out float4 outPosition : POSITION0,
	out float3 outNormalWorld : NORMAL0,
	out float2 outTexCoords : TEXCOORD0,
	out float3 outPositionWorld : TEXCOORD1,
	out float4 outDirectionalShadow0ProjTex : TEXCOORD2,
	out float outDirectionalShadow0RealDistance : TEXCOORD3,
	out float outFogLerp : TEXCOORD4)
{
	// position
	outPosition = mul(inPosition, xWorldViewProjection);
	
	// normal
	outNormalWorld = normalize(mul(inNormal, xWorld));
	
	// texture coords
	outTexCoords = inTexCoords;
	
	// lighting and shadowing
	outPositionWorld = mul(inPosition, xWorld);
	
	// shadowing
	outDirectionalShadow0ProjTex = mul(inPosition, xDirectionalShadowWorldViewProjections[0]);
	outDirectionalShadow0RealDistance = outDirectionalShadow0ProjTex.z / outDirectionalShadow0ProjTex.w;
	
	// fog
	float distance = distance(outPositionWorld, xCameraPosition);
	float fogRange = xFogEnd - xFogStart;
	float distanceIntoFogStart = distance - xFogStart;
	outFogLerp = saturate(distanceIntoFogStart / fogRange);
}

float ExtractRange(float number, float low, float high)
{
	float range = high - low;
	float average = (low + high) / 2;
	float similarity = range - abs(number - average);	
	return saturate(similarity / range);
}

void NormalPixelShader(
	float3 inNormalWorld : NORMAL0,
	float2 inTexCoords : TEXCOORD0,
	float3 inPositionWorld : TEXCOORD1,
	float4 inDirectionalShadow0ProjTex : TEXCOORD2,
	float inDirectionShadow0RealDistance : TEXCOORD3,
	float inFogLerp : TEXCOORD4,
	out float4 outColor : COLOR0)
{	
	// always normalize a pixel shader's incoming normals
	inNormalWorld = normalize(inNormalWorld);

	// calculate the blendMapValue
	float blendMapValue = inPositionWorld.y / xHeightMax;

	// calculate the color of the combined terrain diffuse maps
	outColor = tex2D(xDiffuseMapSampler0, inTexCoords) * ExtractRange(blendMapValue, 0, 0.25f);
	outColor += tex2D(xDiffuseMapSampler1, inTexCoords) * ExtractRange(blendMapValue, 0.25f, 0.5f);
	outColor += tex2D(xDiffuseMapSampler2, inTexCoords) * ExtractRange(blendMapValue, 0.5f, 0.75f);
	outColor += tex2D(xDiffuseMapSampler3, inTexCoords) * ExtractRange(blendMapValue, 0.75f, 1);
	
	// apply lighting
	if (xLightingEnabled)
	{
		float3 lightColor = (float3)0;
		
		// apply ambient light
		if (xAmbientLightEnabled) lightColor += xAmbientLightColor;
		
		// apply all directional lights
		for (int i = 0; i < directionalLightCount; ++i)
		{
			if (xDirectionalLightEnableds[i])
			{
				if (i != 0 || // only the first directional shadow can be handled on current hardware
					!xDirectionalShadowEnableds[0] || // cannot specify 'i' here since compiler can't deduce its range
					!InShadow(inDirectionalShadow0ProjTex, inDirectionShadow0RealDistance, xDirectionalShadow0Sampler))
					lightColor += CalculateDirectionalLightDiffuseColor(inNormalWorld, i);
					
				lightColor += CalculateDirectionalLightSpecularColor(inPositionWorld, inNormalWorld, i);
			}
		}
		
		// apply all point lights
		for (int i = 0; i < pointLightCount; ++i)
		{
			if (xPointLightEnableds[i])
			{
				float pointLightIntensity = CalculatePointLightIntensity(inPositionWorld, i);
				lightColor += CalculatePointLightDiffuseColor(inPositionWorld, inNormalWorld, pointLightIntensity, i);
				lightColor += CalculatePointLightSpecularColor(inPositionWorld, inNormalWorld, pointLightIntensity, i);
			}
		}
		
		outColor.rgb *= lightColor;
	}
	
	// apply fog
	if (xFogEnabled) outColor.rgb = ApplyFog(outColor.rgb, inFogLerp);
}

technique Normal
{
	pass Normal
	{
		vertexShader = compile vs_3_0 NormalVertexShader();
		pixelShader = compile ps_3_0 NormalPixelShader();
	}
}

technique DirectionalShadow
{
	pass DirectionalShadow
	{
		vertexShader = compile vs_2_0 DirectionalShadowVertexShader();
		pixelShader = compile ps_2_0 DirectionalShadowPixelShader();
	}
}
