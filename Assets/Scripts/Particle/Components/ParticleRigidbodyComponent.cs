using Unity.Entities;

[GenerateAuthoringComponent]
public struct ParticleRigidbody : IComponentData {
    public float radius;
    public float mass;
}
