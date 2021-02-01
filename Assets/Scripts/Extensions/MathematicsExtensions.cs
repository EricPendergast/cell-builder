using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;


namespace MathematicsExtensions {
    public static class Extensions {
        public static float3 xy0(this float2 f) {
            return new float3(f.x, f.y, 0);
        }

        public static float2 xy(this Translation t) {
            return new float2(t.Value.x, t.Value.y);
        }

        public static float2 xy(this Velocity v) {
            return new float2(v.Value.x, v.Value.y);
        }
    }
}
