using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Jobs;


[UpdateBefore(typeof(ApplyVelocitySystem))]
[UpdateBefore(typeof(TransformSystemGroup))]
public class ApplyCollisionsSystem : SystemBase {
    protected override void OnUpdate() {
        Entities
            .WithName("ApplyCollisionResponses")
            .WithAll<Particle>()
            .ForEach((ref Translation position, ref Velocity velocity, ref CollisionResponse collision) => {
                position.Value += math.float3(collision.deltaPosition, 0);
                velocity.Value += collision.deltaVelocity;
                collision.deltaPosition = 0;
                collision.deltaVelocity = 0;
            })
            .ScheduleParallel();
    }
}
