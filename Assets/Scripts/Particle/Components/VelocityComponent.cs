using Unity.Mathematics;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct Velocity : IComponentData {
    public float2 Value;

    public float3 xy0 => new float3(Value, 0);
}
