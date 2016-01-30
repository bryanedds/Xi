#include "XiLightReceiverBase.fx"

void NormalVertexShader(
	float4 inPosition : POSITION0,
	float3 inNormal : NORMAL0,
	float2 inTexCoords : TEXCOORD0,
	out float4 outPosition : POSITION0,
	out float3 outNormalWorld : NORMAL0,
	out float2 outTexCoords : TEXCOORD0,
	out float3 outPositionWorld : TEXCOORD1,
	out float outFogLerp : TEXCOORD2)
{
	// position
	outPosition = mul(inPosition, xWorldViewProjection);
	
	// normal
	outNormalWorld = normalize(mul(inNormal, xWorld));
	
	// texture coords
	outTexCoords = inTexCoords;
	
	// lighting
	outPositionWorld = mul(inPosition, xWorld);
	
	// fog
	float distance = distance(outPositionWorld, xCameraPosition);
	float fogRange = xFogEnd - xFogStart;
	float distanceIntoFogStart = distance - xFogStart;
	outFogLerp = saturate(distanceIntoFogStart / fogRange);
}

void NormalPixelShader(
	float3 inNormalWorld : NORMAL0,
	float2 inTexCoords : TEXCOORD0,
	float3 inPositionWorld : TEXCOORD1,
	float inFogLerp : TEXCOORD2,
	out float4 outColor : COLOR0)
{	
	// always normalize a pixel shader's incoming normals
	inNormalWorld = normalize(inNormalWorld);
	
	// grab the color from the texture
	outColor = tex2D(xDiffuseMapSampler, inTexCoords);
	
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
	
	// apply diffuse alpha
	outColor.a *= xDiffuseColor.a;
	
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
