#ifndef LIGHT_RECEIVER_BASE_FX
#define LIGHT_RECEIVER_BASE_FX

#include "XiFogReceiverBase.fx"

#define directionalLightCount 4
#define pointLightCount 8

// general lighting
bool xLightingEnabled;

// material properties
float4 xDiffuseColor;
float3 xSpecularColor;
float xSpecularPower;

// ambient lighting
bool xAmbientLightEnabled;
float3 xAmbientLightColor;

// directional lights
bool xDirectionalLightEnableds[directionalLightCount];
float3 xDirectionalLightDirections[directionalLightCount];
float3 xDirectionalLightDiffuseColors[directionalLightCount];
float3 xDirectionalLightSpecularColors[directionalLightCount];

// point lights
bool xPointLightEnableds[pointLightCount];
float3 xPointLightPositions[pointLightCount];
float3 xPointLightDiffuseColors[pointLightCount];
float3 xPointLightSpecularColors[pointLightCount];
float xPointLightRanges[pointLightCount];
float xPointLightFalloffs[pointLightCount];

float3 CalculateDirectionalLightDiffuseColor(float3 pixelNormalWorld, int index)
{
	float3 lightDiffuseColor = xDirectionalLightDiffuseColors[index];
	float3 lightDirection = xDirectionalLightDirections[index];
	float dotProduct = dot(pixelNormalWorld, -lightDirection);
	return xDiffuseColor.rgb * lightDiffuseColor * saturate(dotProduct);
}

float3 CalculateDirectionalLightSpecularColor(
	float3 pixelPositionWorld, float3 pixelNormalWorld, int index)
{
	float3 lightSpecularColor = xDirectionalLightSpecularColors[index];
	float3 lightDirection = xDirectionalLightDirections[index];
	float3 toCamera = normalize(xCameraPosition - pixelPositionWorld);
	float3 reflection = reflect(lightDirection, pixelNormalWorld);
	float specularIntensity = pow(saturate(dot(reflection, toCamera)), xSpecularPower);	
	return xSpecularColor * lightSpecularColor * specularIntensity;
}

float3 CalculatePointLightDiffuseColor(
	float3 pixelPositionWorld, float3 pixelNormalWorld, float pointLightIntensity, int index)
{	
	float3 lightPosition = xPointLightPositions[index];
	float3 lightDiffuseColor = xPointLightDiffuseColors[index];	
	float3 lightNormal = normalize(pixelPositionWorld - lightPosition);	
	float dotProduct = dot(pixelNormalWorld, -lightNormal);	
	return xDiffuseColor.rgb * lightDiffuseColor * saturate(dotProduct) * pointLightIntensity;
}

float CalculatePointLightIntensity(float3 pixelPositionWorld, int index)
{
	float result;	
	float3 lightPosition = xPointLightPositions[index];
	float lightRange = xPointLightRanges[index];
	float distanceFromLight = length(pixelPositionWorld - lightPosition);
	if (distanceFromLight < lightRange) result = 1.0f;
	else
	{
		float lightFalloff = xPointLightFalloffs[index];
		float secondDistanceFromLight = distanceFromLight - lightRange;
		result = saturate(lightFalloff / secondDistanceFromLight);
	}
	
	return result;
}

float3 CalculatePointLightSpecularColor(
	float3 pixelPositionWorld, float3 pixelNormalWorld, float pointLightIntensity, int index)
{
	float3 lightPosition = xPointLightPositions[index];
	float3 lightSpecularColor = xPointLightSpecularColors[index];
	float3 lightNormal = normalize(pixelPositionWorld - lightPosition);
	float3 toCamera = normalize(xCameraPosition - pixelPositionWorld);
	float3 reflection = reflect(lightNormal, pixelNormalWorld);
	float specularIntensity = pow(saturate(dot(reflection, toCamera)), xSpecularPower);
	return xSpecularColor * lightSpecularColor * specularIntensity * pointLightIntensity;
}

float CalculateSpecularEffect(float3 specularColor)
{
	return (specularColor.r + specularColor.g + specularColor.b) / 3.0f;
}

void DirectionalShadowVertexShader(
	float4 inPosition : POSITION0,
	out float4 outPosition : POSITION0,
	out float4 outPositionDuplicate : TEXCOORD0)
{
	outPosition = mul(inPosition, xWorldViewProjection);
	outPositionDuplicate = outPosition;
}

void DirectionalShadowPixelShader(
	float4 inPosition : TEXCOORD0,
	out float4 outColor : COLOR0)
{
	float depth = inPosition.z / inPosition.w;
	outColor = float4(depth, 0, 0, 0);
}

#endif
