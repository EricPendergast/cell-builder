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

                foreach (int entityId in new NearbyCellIterator<int>(centerGrid, grid)) {
                    Translation otherPos = positions[entityId];
                    ParticleMath.ResolveCollision(pos, ref collisionResponse, otherPos);
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

    private struct NearbyCellIterator<T> where T : struct {
        private int2 centerGrid;
        private NativeMultiHashMap<int, T> grid;
        private MultiHashMapCellIterator<T> it;
        private int o_x, o_y;
        
        public NearbyCellIterator<T> GetEnumerator() => this;

        public NearbyCellIterator(int2 centerGrid, NativeMultiHashMap<int, T> grid) {
            this.centerGrid = centerGrid;
            this.grid = grid;
            o_x = -1;
            o_y = -1;
            it = new MultiHashMapCellIterator<T>(grid, (int)math.hash(centerGrid + new int2(o_x, o_y)));
        }
    
        public bool MoveNext() {
            while (!it.MoveNext()) {
                o_y++;
                if (o_y > 1) {
                    o_y = -1;
                    o_x++;
                }
                if (o_x > 1) {
                    return false;
                }
                it = new MultiHashMapCellIterator<T>(grid, (int)math.hash(centerGrid + new int2(o_x, o_y)));
            }

            return true;
        }

        public T Current => it.Current;
    }

    private struct MultiHashMapCellIterator<T> where T : struct {
        private NativeMultiHashMap<int, T> hashMap;
        NativeMultiHashMapIterator<int> it;
        private bool found;
        public T current;
        private T nextItem;
        public MultiHashMapCellIterator<T> GetEnumerator() => this;

        public MultiHashMapCellIterator(NativeMultiHashMap<int, T> hashMap, int hash) {
            this.hashMap = hashMap;
            
            found = hashMap.TryGetFirstValue(hash, out nextItem, out it);

            current = default(T);
        }

        public bool MoveNext() {
            if (!found) {
                return false;
            }

            current = nextItem;
            found = hashMap.TryGetNextValue(out nextItem, ref it);
            return true;
        }

        public T Current => current;
    }
}
