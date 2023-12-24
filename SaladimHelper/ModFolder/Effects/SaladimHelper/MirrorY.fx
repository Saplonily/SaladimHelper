uniform float Strength = 1.0;

texture2D tex : register(t0);
sampler texSampler : register(s0);

float4 PS_Mask(float2 uv : TEXCOORD0) : COLOR0
{
    float4 origin = tex2D(texSampler, uv);
    float4 mirrorY = tex2D(texSampler, float2(uv.x, 1.0 - uv.y));
    return Strength * mirrorY + (1 - Strength) * origin;
}

technique InvertColor
{
    pass pass0
    {
        PixelShader = compile ps_2_0 PS_Mask();
    }
}