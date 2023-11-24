uniform float Strength = 1.0;
static const float offset = 1.0 / 300.0;

texture2D tex : register(t0);
sampler texSampler : register(s0);

float4 PS_Mask(float2 uv : TEXCOORD0) : COLOR0
{
    float2 offsets[9] =
    {
        float2(-offset, offset),
        float2(0.0, offset),
        float2(offset, offset),
        float2(-offset, 0.0),
        float2(0.0, 0.0),
        float2(offset, 0.0),
        float2(-offset, -offset),
        float2(0.0, -offset),
        float2(offset, -offset)
    };

    float kernel[9] =
    {
        0.0947416, 0.118318, 0.0947416,
        0.1183180, 0.147761, 0.1183180,
        0.0947416, 0.118318, 0.0947416
    };

    float3 sampleTex[9];
    for (int i = 0; i < 9; i++)
    {
        sampleTex[i] = float3(tex2D(texSampler, uv + offsets[i]).rgb);
    }

    float3 col = float3(0.0, 0.0, 0.0);
    for (int j = 0; j < 9; j++)
        col += sampleTex[j] * kernel[j];

    float4 origin = tex2D(texSampler, uv);
    return Strength * float4(col, 1.0) + (1 - Strength) * origin;
}

technique InvertColor
{
    pass pass0
    {
        PixelShader = compile ps_2_0 PS_Mask();
    }
}