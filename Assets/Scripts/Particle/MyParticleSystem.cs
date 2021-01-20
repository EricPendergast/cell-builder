using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;


[UpdateBefore(typeof(TransformSystemGroup))]
public class MyParticleSystem : SystemBase {

    private EntityQuery hashPositionsQuery;

    protected override void OnCreate() {
    }

    protected override void OnUpdate() {
        int particleCount = hashPositionsQuery.CalculateEntityCount();

        var grid = new NativeMultiHashMap<int, int>(particleCount, Allocator.TempJob);
        var positions = new NativeArray<Translation>(particleCount, Allocator.TempJob);

        var gridWriter = grid.AsParallelWriter();

        Entities
            .WithName("HashPositions")
            .WithAll<Particle, CollisionResponse, Translation>()
            .WithStoreEntityQueryInField(ref hashPositionsQuery)
            .ForEach((int entityInQueryIndex, in Translation pos) => {

                gridWriter.Add(GridHash(pos.Value.xy), entityInQueryIndex);
                positions[entityInQueryIndex] = pos;

            }).ScheduleParallel();

        Entities
            .WithName("ResolveCollisions")
            .WithAll<Particle, CollisionResponse, Translation>()
            .WithReadOnly(grid)
            .WithReadOnly(positions)
            .ForEach((
                    int entityInQueryIndex,
                    ref CollisionResponse collisionResponse, 
                    in Translation pos) => {

                int2 centerGrid = ToGrid(pos.Value.xy);

                bool found = false;

                for (int o_x = -1; o_x <= 1; o_x++) {
                    for (int o_y = -1; o_y <= 1; o_y++) {
                        int2 searchGrid = centerGrid + new int2(o_x, o_y);

                        NativeMultiHashMapIterator<int> it;
                        found = grid.TryGetFirstValue((int)math.hash(searchGrid), out int entityId, out it);
                        while (found) {
                            Translation otherPos = positions[entityId];
                            ParticleMath.ResolveCollision(pos, ref collisionResponse, otherPos);

                            found = grid.TryGetNextValue(out entityId, ref it);
                        }
                    }
                }

            }).ScheduleParallel();

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

        float deltaTime = Time.DeltaTime;
        Entities
            .WithName("ApplyVelocity")
            .WithAll<Particle>()
            .ForEach((ref Translation position, in Velocity velocity) => {
                    position.Value += new float3(velocity.Value, 0)*deltaTime;
            })
            .ScheduleParallel();

        Dependency.Complete();
        grid.Dispose();
        positions.Dispose();
    }

    const float gridSize = 2f;

    private static int2 ToGrid(float2 pos) {
        return new int2(math.floor(pos/gridSize));
    }

    private static int GridHash(float2 p) {
        return (int)math.hash(ToGrid(p));
    }
}
