#include "XiShadowReceiverBase.fx"

// normal map
texture xNormalMap;
bool xNormalMapEnabled;

// normal map sampler
sampler xNormalMapSampler = sampler_state
{
	texture = <xNormalMap>;
	minFilter = LINEAR;
	magFilter = LINEAR;
	mipFilter = LINEAR;
	addressU = CLAMP;
	addressV = CLAMP;
};

void NormalVertexShader(
	float4 inPosition : POSITION0,
	float3 inNormal : NORMAL0,
	float2 inTexCoords : TEXCOORD0,
	float3 inTangent : TANGENT0,
	float3 inBinormal : BINORMAL0,
	out float4 outPosition : POSITION0,
	out float2 outTexCoords : TEXCOORD0,
	out float3 outPositionWorld : TEXCOORD1,
	out float4 outDirectionalShadow0ProjTex : TEXCOORD2,
	out float outDirectionalShadow0RealDistance : TEXCOORD3,
	out float3 outTangent : TEXCOORD4,
	out float3 outBinormal : TEXCOORD5,
	out float3 outNormal : TEXCOORD6,
	out float outFogLerp : TEXCOORD7)
{
	// position
	outPosition = mul(inPosition, xWorldViewProjection);
	
	// texture coords
	outTexCoords = inTexCoords;
	
	// lighting and shadowing
	outPositionWorld = mul(inPosition, xWorld);
	
	// shadowing
	outDirectionalShadow0ProjTex = mul(inPosition, xDirectionalShadowWorldViewProjections[0]);
	outDirectionalShadow0RealDistance = outDirectionalShadow0ProjTex.z / outDirectionalShadow0ProjTex.w;
	
	// normal mapping
	float3x3 tangentSpace = float3x3(inTangent, inBinormal, inNormal);
	float3x3 world = { xWorld[0].xyz, xWorld[1].xyz, xWorld[2].xyz };
	float3x3 tangentToWorld = mul(tangentSpace, world);
	outTangent = tangentToWorld[0];
	outBinormal = tangentToWorld[1];
	outNormal = tangentToWorld[2];
	
	// fog
	float distance = distance(outPositionWorld, xCameraPosition);
	float fogRange = xFogEnd - xFogStart;
	float distanceIntoFogStart = distance - xFogStart;
	outFogLerp = saturate(distanceIntoFogStart / fogRange);
}

void NormalPixelShader(
	float2 inTexCoords : TEXCOORD0,
	float3 inPositionWorld : TEXCOORD1,
	float4 inDirectionalShadow0ProjTex : TEXCOORD2,
	float inDirectionShadow0RealDistance : TEXCOORD3,
	float3 inTangent : TEXCOORD4,
	float3 inBinormal : TEXCOORD5,
	float3 inNormal : TEXCOORD6,
	float inFogLerp : TEXCOORD7,
	out float4 outColor : COLOR0)
{
	// reconstruct tangentToWorld
	float3x3 tangentToWorld = { normalize(inTangent), normalize(inBinormal), normalize(inNormal) };
	
	float3 normal = xNormalMapEnabled ?
		// grab the normal from the normal map
		normalize(tex2D(xNormalMapSampler, inTexCoords).xyz * 2 - 1) :
		// use the default normal
		float3(0, 0, 1);
	
	// calculate normal
	float3 normalWorld = normalize(mul(normal, tangentToWorld).xyz);
	
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
				if (i != 0 || // only the first directional shadow can be handled on current hardware
					!xDirectionalShadowEnableds[0] || // cannot specify 'i' here since compiler can't deduce its range
					!InShadow(inDirectionalShadow0ProjTex, inDirectionShadow0RealDistance, xDirectionalShadow0Sampler))
					lightColor += CalculateDirectionalLightDiffuseColor(normalWorld, i);
					
				lightColor += CalculateDirectionalLightSpecularColor(inPositionWorld, normalWorld, i);
			}
		}
		
		// apply all point lights
		for (int i = 0; i < pointLightCount; ++i)
		{
			if (xPointLightEnableds[i])
			{
				float pointLightIntensity = CalculatePointLightIntensity(inPositionWorld, i);
				lightColor += CalculatePointLightDiffuseColor(inPositionWorld, normalWorld, pointLightIntensity, i);
				lightColor += CalculatePointLightSpecularColor(inPositionWorld, normalWorld, pointLightIntensity, i);
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
