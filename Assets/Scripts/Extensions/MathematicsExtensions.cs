using Unity.Mathematics;


namespace MathematicsExtensions {
    public static class Extensions {
        public static float3 xy0(this float2 f) {
            return new float3(f.x, f.y, 0);
        }
    }
}
