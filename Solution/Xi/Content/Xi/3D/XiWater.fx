#include "XiLightReceiverBase.fx"

// transform from tangent space to world space
const float3x3 tangentToWorld =
{
	{ 1, 0, 0 },
	{ 0, 0,-1 },
	{ 0, 1, 0 }
};

// reflection
float4x4 xReflectionView;
Texture xReflectionMap;

// color
float4 xWaterColorMultiplier;
float4 xWaterColorAdditive;

// waves
Texture xWaveMap0;
Texture xWaveMap1;
float2 xWaveOffset0;
float2 xWaveOffset1;
float xWaveLength = 0.5f;
float xWaveHeight = 0.5f;

sampler xWaveMap0Sampler = sampler_state
{
	texture = <xWaveMap0>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = MIRROR;
	AddressV = MIRROR;
};

sampler xWaveMap1Sampler = sampler_state
{
	texture = <xWaveMap1>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = MIRROR;
	AddressV = MIRROR;
};

sampler xReflectionMapSampler = sampler_state
{
	texture = <xReflectionMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = MIRROR;
	AddressV = MIRROR;
};

void NormalVertexShader(
	float4 inPosition : POSITION0,
	float3 inNormal : NORMAL0,
	float2 inTexCoords : TEXCOORD0,
	out float4 outPosition : POSITION0,
	out float4 outReflectionMapSamplingPosition : TEXCOORD0,
	out float4 outPositionWorld : TEXCOORD1,
	out float2 outTexCoords0 : TEXCOORD2,
	out float2 outTexCoords1 : TEXCOORD3)
{
	// position
	outPosition = mul(inPosition, xWorldViewProjection);
	
	// reflection and lighting
	outPositionWorld = mul(inPosition, xWorld);
	
	// reflection
	float4x4 preReflectionViewProjection = mul(xReflectionView, xProjection);
	float4x4 preWorldReflectionViewProjection = mul(xWorld, preReflectionViewProjection);
	outReflectionMapSamplingPosition = mul(inPosition, preWorldReflectionViewProjection);
	
	// waves
	float2 texCoords = inTexCoords / xWaveLength;
	outTexCoords0 = texCoords + xWaveOffset0;
	outTexCoords1 = texCoords + xWaveOffset1;
}

void NormalPixelShader(
	float4 inReflectionMapSamplingPosition : TEXCOORD0,
	float4 inPositionWorld : TEXCOORD1,
	float2 inTexCoords0 : TEXCOORD2,
	float2 inTexCoords1 : TEXCOORD3,
	out float4 outColor : COLOR0)
{
	// calculate the projective texturing coordinates
	float2 projTexCoords = float2(
		+inReflectionMapSamplingPosition.x / inReflectionMapSamplingPosition.w / 2 + 0.5f,
		-inReflectionMapSamplingPosition.y / inReflectionMapSamplingPosition.w / 2 + 0.5f);

	// pull sample from wave map 0 and transform to [-1, 1] range
	float3 wave0Sample = tex2D(xWaveMap0Sampler, inTexCoords0).xyz * 2 - 1;
	
	// pull sample from wave map 1 and transform to [-1, 1] range
	float3 wave1Sample = tex2D(xWaveMap1Sampler, inTexCoords1).xyz * 2 - 1;
	
	// average the samples
	float3 sampleAvg = (wave0Sample + wave1Sample) * 0.5f;
		
	// calculate the normal
	float3 normal = normalize(sampleAvg);
	
	// transform the normal to world space
	normal = mul(normal, tangentToWorld);
	
	// pick color from reflection sampler at the perturbed coords
	outColor = tex2D(xReflectionMapSampler, projTexCoords + normal.xz * xWaveHeight);
	
	// overwrite unintended transparency added by pink hues
	outColor.a = 1;
	
	// apply water color
	outColor *= xWaterColorMultiplier;
	outColor += xWaterColorAdditive;
	
	// apply lighting	
	if (xLightingEnabled)
	{	
		float3 lightColor = (float3)0;
		float specularEffect = 0;
		
		// apply ambient light
		if (xAmbientLightEnabled) lightColor += xAmbientLightColor;
		
		// apply all directional lights
		for (int i = 0; i < directionalLightCount; ++i)
		{
			if (xDirectionalLightEnableds[i])
			{			
				lightColor += CalculateDirectionalLightDiffuseColor(normal, i);
				
				float3 specularColor = CalculateDirectionalLightSpecularColor(inPositionWorld, normal, i);
				lightColor += specularColor;
				
				specularEffect += CalculateSpecularEffect(specularColor);
			}
		}
		
		// apply all point lights
		for (int i = 0; i < pointLightCount; ++i)
		{
			if (xPointLightEnableds[i])
			{
				float pointLightIntensity = CalculatePointLightIntensity(inPositionWorld, i);
				lightColor += CalculatePointLightDiffuseColor(inPositionWorld, normal, pointLightIntensity, i);
				
				float3 specularColor = CalculatePointLightSpecularColor(inPositionWorld, normal, pointLightIntensity, i);
				lightColor += specularColor;
				
				specularEffect += CalculateSpecularEffect(specularColor);
			}
		}
		
		outColor.rgb *= lightColor;
		outColor.a += specularEffect;
	}
	
	// apply fog
	if (xFogEnabled)
	{
		float distance = distance(inPositionWorld, xCameraPosition);
		float fogRange = xFogEnd - xFogStart;
		float fogLerp = saturate((distance - xFogStart) / fogRange);	
		outColor.rgb = ApplyFog(outColor.rgb, fogLerp);
	}
}

technique Normal
{
	pass Normal
	{
		vertexShader = compile vs_3_0 NormalVertexShader();
		pixelShader = compile ps_3_0 NormalPixelShader();
	}
}
