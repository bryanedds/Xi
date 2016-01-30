#ifndef FOG_RECEIVER_BASE_FX
#define FOG_RECEIVER_BASE_FX

#include "XiStandardBase.fx"

float3 xFogColor;
float xFogStart;
float xFogEnd;
bool xFogEnabled;

float3 ApplyFog(float3 color, float lerpValue)
{
	return lerp(color, xFogColor, lerpValue);
}

#endif
