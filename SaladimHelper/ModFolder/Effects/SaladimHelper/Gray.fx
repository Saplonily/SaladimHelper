uniform float Strength = 1.0;

texture2D tex : register(t0);
sampler texSampler : register(s0);

float4 PS_Mask(float2 uv : TEXCOORD0) : COLOR0
{
    float4 origin = tex2D(texSampler, uv);
    float s = 0.2125 * origin.r + 0.7154 * origin.g + 0.0721 * origin.a;
    return Strength * float4(s, s, s, origin.a) + (1 - Strength) * origin;
}

technique InvertColor
{
    pass pass0
    {
        PixelShader = compile ps_2_0 PS_Mask();
    }
}