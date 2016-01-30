#include "XiStandardBase.fx"

void NormalVertexShader(
	float4 inPosition : POSITION0,
	inout float4 ioColor : COLOR0,
    out float4 outPosition : POSITION0)
{
    outPosition = mul(inPosition, xWorldViewProjection);
}

void NormalPixelShader(inout float4 ioColor : COLOR0) { }

technique Normal
{
    pass Normal
    {       
        vertexShader = compile vs_1_1 NormalVertexShader();
        pixelShader = compile ps_1_1 NormalPixelShader();
    }
}
