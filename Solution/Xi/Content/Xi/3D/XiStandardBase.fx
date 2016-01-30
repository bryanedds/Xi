#ifndef STANDARD_BASE_FX
#define STANDARD_BASE_FX

// standard effect parameters
float4x4 xWorld;
float4x4 xView;
float4x4 xProjection;
float4x4 xViewProjection;
float4x4 xWorldViewProjection;
float4x4 xWorldInverse;
texture xDiffuseMap;
float3 xCameraPosition;

sampler xDiffuseMapSampler = sampler_state
{
	texture = <xDiffuseMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

#endif
