using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;

public class ParticleMath {

    public static void ResolveCollision(Translation t1, ref CollisionResponse c1, Translation t2) {
        
        float radius = .5f;

        var p2ToP1 = t1.Value.xy - t2.Value.xy;
        float distance = math.length(p2ToP1);
        if (distance < radius*2) {
            c1.deltaPosition += math.normalizesafe(p2ToP1)*(radius*2-distance)/2;
        }
    }
}
