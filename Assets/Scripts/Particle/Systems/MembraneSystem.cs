using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Jobs;

using Utilities;

[UpdateAfter(typeof(ParticleGridSystem))]
[UpdateBefore(typeof(ApplyCollisionsSystem))]
[UpdateBefore(typeof(TransformSystemGroup))]
public class MembraneSystem : SystemBase {
    ParticleGridSystem myParticleSystem;

    protected override void OnCreate() {
        myParticleSystem = World.GetOrCreateSystem<ParticleGridSystem>();
    }

    protected override void OnUpdate() {
        var grid = myParticleSystem.grid;

        Entities
            .WithName("ResolveCollisions")
            .WithAll<Particle, CollisionResponse, Translation>()
            .WithReadOnly(grid)
            .ForEach((
                    ref CollisionResponse collisionResponse, 
                    in Translation pos) => {

                int2 centerGrid = ParticleGridSystem.ToGrid(pos.Value.xy);

                foreach (Translation otherPos in new NearbyGridCellIterator<Translation>(centerGrid, grid)) {
                    ParticleMath.ResolveCollision(pos, ref collisionResponse, otherPos);
                }

            }).ScheduleParallel();
    }
}
