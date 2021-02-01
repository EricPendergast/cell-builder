using Unity.Entities;
using Unity.Transforms;


[UpdateBefore(typeof(TransformSystemGroup))]
public class ParticleSystemGroup : ComponentSystemGroup {
}
