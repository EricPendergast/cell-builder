using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Jobs;

public struct RigidParticleInfo {
    public ParticleRigidbody body;
    public Translation pos;
    public Velocity vel;
    public Entity entity;
}

[UpdateInGroup(typeof(ParticleSystemGroup))]
public class ParticleGridSystem : SystemBase {

    private EntityQuery hashPositionsQuery;
    public NativeMultiHashMap<int2, RigidParticleInfo> grid;

    protected override void OnCreate() {
        grid = new NativeMultiHashMap<int2, RigidParticleInfo>(100, Allocator.Persistent);
    }

    protected override void OnDestroy() {
        grid.Dispose();
    }

    protected override void OnUpdate() {
        int particleCount = hashPositionsQuery.CalculateEntityCount();

        Resize(ref grid, particleCount);
        grid.Clear();

        var gridWriter = grid.AsParallelWriter();

        Entities
            .WithName("HashPositions")
            .WithStoreEntityQueryInField(ref hashPositionsQuery)
            .ForEach((in Entity entity, in ParticleRigidbody body, in Translation pos, in Velocity vel) => {

                gridWriter.Add(ToGrid(pos.Value.xy),
                    new RigidParticleInfo{
                        body=body, pos=pos, vel=vel, entity=entity
                    }
                );

            }).ScheduleParallel();
    }

    private static void Resize<T>(ref NativeMultiHashMap<int2, T> grid, int particleCount) where T : struct {
        if (particleCount > grid.Capacity) {
            int cap = grid.Capacity;
            grid.Dispose();
            grid = new NativeMultiHashMap<int2, T>(math.max(cap*2, particleCount), Allocator.Persistent);
        }
    }

    const float gridSize = 2f;

    public static int2 ToGrid(float2 pos) {
        return new int2(math.floor(pos/gridSize));
    }

    public JobHandle GetFinalJobHandle() {
        return Dependency;
    }
}
