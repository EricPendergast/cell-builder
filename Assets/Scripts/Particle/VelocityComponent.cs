using Unity.Mathematics;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct Velocity : IComponentData {
    public float2 Value;
}
