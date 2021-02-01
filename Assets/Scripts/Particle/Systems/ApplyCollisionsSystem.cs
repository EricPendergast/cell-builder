using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Jobs;


[UpdateBefore(typeof(ApplyVelocitySystem))]
[UpdateBefore(typeof(TransformSystemGroup))]
public class ApplyCollisionsSystem : SystemBase {
    protected override void OnUpdate() {
        
        float dt = Time.DeltaTime;

        Entities
            .WithName("ApplyCollisionResponses")
            .ForEach((ref Translation position, ref Velocity velocity, ref CollisionResponse collision, in ParticleRigidbody rb) => {
                position.Value += math.float3(collision.deltaPosition, 0);
                velocity.Value += collision.deltaVelocity;
                velocity.Value += collision.force*dt/rb.mass;
                collision.deltaPosition = 0;
                collision.deltaVelocity = 0;
                collision.force = 0;
            })
            .ScheduleParallel();
    }
}
