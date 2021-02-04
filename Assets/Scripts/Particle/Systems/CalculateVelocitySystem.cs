using Unity.Transforms;
using Unity.Entities;
using Unity.Jobs;

using MathematicsExtensions;

[UpdateInGroup(typeof(AfterApplyVelocityGroup))]
public class CalculateVelocitySystem : SystemBase {
    protected override void OnUpdate() {
        float dt = Time.DeltaTime;
        Entities
            .WithName("CalculateVelocity")
            .ForEach((
                    ref Velocity vel,
                    in PreviousPosition prevPos,
                    in Translation pos) => {

                vel.Value = (pos.xy() - prevPos.Value)/dt;

            }).ScheduleParallel();
    }
}
