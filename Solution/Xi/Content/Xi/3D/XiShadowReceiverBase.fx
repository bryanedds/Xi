#ifndef SHADOW_RECEIVER_BASE_FX
#define SHADOW_RECEIVER_BASE_FX

#include "XiLightReceiverBase.fx"

#define directionalShadowCount 1

// directional shadows
float xDirectionalShadowDepthBias;
float3 xDirectionalShadowPositions[directionalShadowCount];
float4x4 xDirectionalShadowWorldViewProjections[directionalShadowCount];
bool xDirectionalShadowEnableds[directionalShadowCount];

// shadow map(s) - the number of these is hard-coded, but we only need 1 anyhow
texture xDirectionalShadow0;

// shadow map sampler(s) - the number of these is hard-coded as well...
sampler xDirectionalShadow0Sampler = sampler_state
{
	texture = <xDirectionalShadow0>;
	minFilter = LINEAR;
	magFilter = LINEAR;
	mipFilter = LINEAR;
	addressU = CLAMP;
	addressV = CLAMP;
};

bool InShadow(float4 projTex, float realDistance, sampler directionalShadowSampler)
{
	bool inShadow = false;
	
	float2 projTexHom = float2(
		+projTex.x / projTex.w / 2 + 0.5f,
		-projTex.y / projTex.w / 2 + 0.5f);
		
	// make sure pixel is in the range of depth map
	if (projTexHom.x == saturate(projTexHom.x) &&
		projTexHom.y == saturate(projTexHom.y) &&
		realDistance < 1)
	{
		float shadowMapDepth = tex2D(directionalShadowSampler, projTexHom).r;
		float biasedRealDistance = realDistance - xDirectionalShadowDepthBias;
		inShadow = biasedRealDistance > shadowMapDepth;
    }
    
    return inShadow;
}

#endif
