using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;

public class ParticleMath {

    public static void ResolveCollision(RigidParticleInfo i1, ref CollisionResponse c1, RigidParticleInfo i2) {
        
        float minDistance = i1.body.radius + i2.body.radius;

        var p2ToP1 = i1.pos.Value.xy - i2.pos.Value.xy;
        float distance = math.length(p2ToP1);
        if (distance < minDistance) {
            c1.deltaPosition += math.normalizesafe(p2ToP1)*(minDistance-distance)/2;
        }
    }
}
