using Unity.Entities;
using Unity.Mathematics;

public struct MouseComponent : IComponentData {
    public Entity selected;
    public float2 mouseWorldPosition;
}
