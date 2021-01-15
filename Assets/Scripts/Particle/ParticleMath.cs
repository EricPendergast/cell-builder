using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;

public class ParticleMath {

    public static void ResolveCollision(Translation t1, ref CollisionResponse c1, Translation t2) {
        if (math.lengthsq(t1.Value - t2.Value) < .5) {
            c1.deltaPosition += (t1.Value.xy - t2.Value.xy)*.5f;
        }
    }
}
