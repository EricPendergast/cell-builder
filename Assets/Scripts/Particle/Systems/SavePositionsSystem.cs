using Unity.Transforms;
using Unity.Entities;
using Unity.Jobs;

using MathematicsExtensions;

[UpdateInGroup(typeof(BeforeApplyVelocityGroup))]
[UpdateAfter(typeof(ViscositySystem))]
public class SavePositionsSystem : SystemBase {
    protected override void OnUpdate() {
        Entities
            .WithName("SavePositions")
            .ForEach((
                    ref PreviousPosition prevPos,
                    in Translation pos) => {

                prevPos.Value = pos.xy();

            }).ScheduleParallel();
    }
}
