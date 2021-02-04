using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(ParticleSystemGroup))]
[UpdateBefore(typeof(AfterApplyVelocityGroup))]
[UpdateAfter(typeof(BeforeApplyVelocityGroup))]
public class ApplyVelocitySystem : SystemBase {
    protected override void OnUpdate() {
        float deltaTime = Time.DeltaTime;
        Entities
            .WithName("ApplyVelocity")
            .ForEach((ref Translation position, in Velocity velocity) => {
                    position.Value += velocity.xy0*deltaTime;
            })
            .ScheduleParallel();
    }
}
