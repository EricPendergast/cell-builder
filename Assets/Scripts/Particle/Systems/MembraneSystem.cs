using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Jobs;

using Utilities;

[UpdateAfter(typeof(ParticleGridSystem))]
[UpdateBefore(typeof(ApplyCollisionsSystem))]
[UpdateInGroup(typeof(ParticleSystemGroup))]
[DisableAutoCreation]
public class MembraneSystem : SystemBase {
    ParticleGridSystem myParticleSystem;

    protected override void OnCreate() {
        myParticleSystem = World.GetOrCreateSystem<ParticleGridSystem>();
    }

    protected override void OnUpdate() {
        Dependency = JobHandle.CombineDependencies(Dependency, myParticleSystem.GetFinalJobHandle());
        
        var grid = myParticleSystem.grid;

        var gelSettings = GelSettings.Instance.data;

        Entities
            .WithName("ResolveCollisions")
            .WithReadOnly(grid)
            .ForEach((
                    ref CollisionResponse collisionResponse, 
                    in ParticleRigidbody body,
                    in Translation pos,
                    in Velocity vel) => {

                int2 centerGrid = ParticleGridSystem.ToGrid(pos.Value.xy);

                foreach (RigidParticleInfo otherPos in new NearbyGridCellIterator<RigidParticleInfo>(centerGrid, grid)) {
                    ParticleMath.GelPhysics(
                        new RigidParticleInfo{
                            body=body,
                            pos=pos,
                            vel=vel
                        },
                        ref collisionResponse,
                        otherPos,
                        gelSettings
                    );
                }
            }).ScheduleParallel();
    }

}
