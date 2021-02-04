using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using MathematicsExtensions;

public class ParticleMath {

    public static void ResolveCollision(RigidParticleInfo i1, ref CollisionResponse c1, RigidParticleInfo i2) {
        
        // TODO: Replace this with a call to SolidCollision
        float minDistance = i1.body.radius + i2.body.radius;

        var p2ToP1 = i1.pos.Value.xy - i2.pos.Value.xy;
        float distance = math.length(p2ToP1);
        if (distance < minDistance) {
            c1.deltaPosition += math.normalizesafe(p2ToP1)*(minDistance-distance)/2;
        }
    }


    public static float2 SolidCollision(float2 p1, float2 p2, float minDistance) {

        var p2ToP1 = p1 - p2;
        float distance = math.length(p2ToP1);
        if (distance < minDistance) {
            return math.normalizesafe(p2ToP1)*(minDistance-distance)/2;
        }
        return 0;
    }

    public static void GelPhysics(RigidParticleInfo i1, ref CollisionResponse c1, RigidParticleInfo i2, GelSettings.Data s) {
        if (i1.pos.xy().Equals(i2.pos.xy())) {
            return;
        }
        float springLength = (i1.body.radius + i2.body.radius)*s.springLengthMultiplier;

        if (math.length(i1.pos.xy() - i2.pos.xy()) < (i1.body.radius+i2.body.radius)) {
            c1.force += DampedSpringForce(i1.pos.xy(), i1.vel.xy(), i2.pos.xy(), i2.vel.xy(), springLength, s.springConstant, s.dampingConstant);
        }

        c1.deltaPosition += ParticleMath.SolidCollision(i1.pos.xy(), i2.pos.xy(), (i1.body.radius + i2.body.radius)*s.solidCoreMultiplier);
    }

    // Note that this damps velocity in all components (NOT just in the
    // component parallel to the spring).
    // Using the equation F = -k*x - b*v
    // Returns the force on p1
    public static float2 DampedSpringForce(float2 p1, float2 v1, float2 p2, float2 v2, float springLength, float k, float damping) {
        var p1ToP2 = p2 - p1;
        var dist = math.length(p1ToP2);
        var positionDifference = math.normalizesafe(p1ToP2)*(dist - springLength);
        var velocityDifference = v2 - v1;

        return k*(positionDifference) + damping*(velocityDifference);
    }
}
