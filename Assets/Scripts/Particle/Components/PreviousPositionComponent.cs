using Unity.Mathematics;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct PreviousPosition : IComponentData {
    public float2 Value;
}
