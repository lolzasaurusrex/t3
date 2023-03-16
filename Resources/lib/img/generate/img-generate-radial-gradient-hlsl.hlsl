cbuffer ParamConstants : register(b0)
{
    float2 Center;
    float Width;
    float Offset;
    float PingPong;
    float Repeat;
    float PolarOrientation;
    float Bias;
}

cbuffer TimeConstants : register(b1)
{
    float globalTime;
    float time;
    float runTime;
    float beatTime;
}

cbuffer Resolution : register(b2)
{
    float TargetWidth;
    float TargetHeight;
}

struct vsOutput
{
    float4 position : SV_POSITION;
    float2 texCoord : TEXCOORD;
};

Texture2D<float4> ImageA : register(t0);
Texture2D<float4> Gradient : register(t1);
sampler texSampler : register(s0);

float fmod(float x, float y)
{
    return (x - y * floor(x / y));
}

float4 psMain(vsOutput psInput) : SV_TARGET
{
    float2 uv = psInput.texCoord;

    float aspectRation = TargetWidth / TargetHeight;
    float2 p = uv;
    p -= 0.5;
    p.x *= aspectRation;

    float c = 0;

    if (PolarOrientation < 0.5)
    {
        c = distance(p, Center) * 2 - Offset;
    }
    else
    {
        p += Center;
        float Radius = 1;
        float l = 2 * length(p) / Radius;

        float2 polar = float2(atan2(p.x, p.y) / 3.141578 / 2 + 0.5, l) + Center - Center.x;
        c = polar.x + Offset;
    }

    float4 orgColor = ImageA.Sample(texSampler, psInput.texCoord);

    c = PingPong > 0.5
            ? (Repeat < 0.5 ? (abs(c) / Width)
                            : saturate(0.9999999 - abs(fmod(c, Width * 2) - Width) / Width))
            : c / Width;

    c = Repeat > 0.5
            ? fmod(c, 1)
            : saturate(c);

    float dBiased = Bias >= 0
                        ? pow(c, Bias + 1)
                        : 1 - pow(clamp(1 - c, 0, 10), -Bias + 1);

    dBiased = clamp(dBiased, 0.001, 0.999);
    float4 gradient = Gradient.Sample(texSampler, float2(dBiased, 0));
    float a = orgColor.a + gradient.a - orgColor.a * gradient.a;
    float3 rgb = (1.0 - gradient.a) * orgColor.rgb + gradient.a * gradient.rgb;
    return float4(rgb, a);
}