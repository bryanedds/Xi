#include "XiStandardBase.fx"

texture xSkyMap;

samplerCUBE skyMap = sampler_state
{
	Texture = <xSkyMap>;
	AddressU  = Wrap;
	AddressV  = Wrap;
	AddressW  = Wrap;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
};

void NormalVertexShader(
	float4 inPosition : POSITION0,
	out float4 outPosition : POSITION0,
	out float3 outCoordinate : TEXCOORD0 )
{
	// Scale the cube as to avoid the near clip plane in the frustum
	float3 scaledPosition = inPosition * 100;

	// Calculate orientation. Using a float3 result, so translation is ignored
	float3 orientedPosition = mul(scaledPosition, xView);
	
	// Calculate projection, moving all vertices to the far clip plane
	outPosition = mul(float4(orientedPosition, 1.0), xProjection);

	// Set the coordinate as a float3
	outCoordinate = float3(inPosition.xyz);
};

void NormalPixelShader(
	float3 inCoordinate : TEXCOORD0,
	out float4 outColor : COLOR0)
{
	// grab the pixel color value from the skybox cube map
	outColor = texCUBE(skyMap, inCoordinate);
};

technique Normal
{
	pass Normal
	{
		vertexShader = compile vs_1_1 NormalVertexShader();
		pixelShader = compile ps_1_1 NormalPixelShader();
	}
}
