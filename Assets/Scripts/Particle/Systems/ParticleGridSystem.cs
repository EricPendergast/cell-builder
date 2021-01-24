using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Jobs;


[UpdateBefore(typeof(TransformSystemGroup))]
public class ParticleGridSystem : SystemBase {

    private EntityQuery hashPositionsQuery;
    [ReadOnly] public NativeMultiHashMap<int, Translation> grid;

    protected override void OnCreate() {
        grid = new NativeMultiHashMap<int, Translation>(100, Allocator.Persistent);
    }

    protected override void OnDestroy() {
        grid.Dispose();
    }

    protected override void OnUpdate() {
        int particleCount = hashPositionsQuery.CalculateEntityCount();

        if (particleCount > grid.Capacity) {
            int cap = grid.Capacity;
            grid.Dispose();
            grid = new NativeMultiHashMap<int, Translation>(math.max(cap*2, particleCount), Allocator.Persistent);
        } else {
            grid.Clear();
        }

        var gridWriter = grid.AsParallelWriter();

        Entities
            .WithName("HashPositions")
            .WithAll<Particle, CollisionResponse, Translation>()
            .WithStoreEntityQueryInField(ref hashPositionsQuery)
            .ForEach((int entityInQueryIndex, in Translation pos) => {

                gridWriter.Add(GridHash(pos.Value.xy), pos);

            }).ScheduleParallel();
    }

    const float gridSize = 2f;

    public static int2 ToGrid(float2 pos) {
        return new int2(math.floor(pos/gridSize));
    }

    private static int GridHash(float2 p) {
        return (int)math.hash(ToGrid(p));
    }

}
