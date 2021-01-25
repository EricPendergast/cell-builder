using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Jobs;

[UpdateBefore(typeof(TransformSystemGroup))]
public class ApplyVelocitySystem : SystemBase {
    protected override void OnUpdate() {
        float deltaTime = Time.DeltaTime;
        Entities
            .WithName("ApplyVelocity")
            .ForEach((ref Translation position, in Velocity velocity) => {
                    position.Value += new float3(velocity.Value, 0)*deltaTime;
            })
            .ScheduleParallel();
    }
}
