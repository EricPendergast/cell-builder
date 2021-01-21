using Unity.Mathematics;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct CollisionResponse : IComponentData {
    public float2 deltaVelocity;
    public float2 deltaPosition;
}

