using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Jobs;

using Utilities;
using MathematicsExtensions;

[UpdateInGroup(typeof(BeforeApplyVelocityGroup))]
[UpdateAfter(typeof(ParticleGridSystem))]
public class ViscositySystem : SystemBase {

    ParticleGridSystem gridSystem;

    protected override void OnCreate() {
        gridSystem = World.GetOrCreateSystem<ParticleGridSystem>();
    }

    protected override void OnUpdate() {
        Dependency = JobHandle.CombineDependencies(Dependency, gridSystem.GetFinalJobHandle());

        var grid = gridSystem.grid;

        var viscositySettings = ViscositySettings.Instance.data;

        float dt = Time.DeltaTime;
        
        Entities
            .WithName("ApplyViscosity")
            .WithReadOnly(grid)
            .ForEach((
                    ref Velocity vel,
                    in ParticleRigidbody body,
                    in Translation pos) => {

                int2 centerGrid = ParticleGridSystem.ToGrid(pos.Value.xy);

                foreach (RigidParticleInfo other in new NearbyGridCellIterator<RigidParticleInfo>(centerGrid, grid)) {
                    ApplyViscosity(
                        ref vel,
                        body,
                        pos,
                        other,
                        dt,
                        viscositySettings);
                }
            }).ScheduleParallel();
    }

    private static void ApplyViscosity(
            ref Velocity vel,
            in ParticleRigidbody body,
            in Translation pos,
            in RigidParticleInfo other,
            float dt,
            in ViscositySettings.Data settings) {
        // Equations from (Clavet, Beaudoin, and Poulin, 2005)
        float2 r_ij = other.pos.xy() - pos.xy();
        float2 r_ijNorm = math.normalizesafe(r_ij);
        float q = math.length(r_ij)/(body.radius + other.body.radius);

        if (q < 1) {
            // Inward radial velocity
            float u = math.dot((other.vel.xy() - vel.xy()), r_ijNorm);
            if (u > 0) {
                // linear and quadratic impulses
                float2 I = dt*(1-q)*(settings.sigma*u + settings.beta*u*u)*r_ijNorm;

                vel.Value -= I/2;
            }
        }
    }
}
