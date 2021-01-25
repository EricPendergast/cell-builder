using Unity.Collections;
using Unity.Mathematics;

namespace Utilities {
    // These iterators work in burst compiled foreach loops. (Because
    // they don't implement IDisposable)

    // Treats the hash map like a grid, and itertes over a 3x3 region
    // of buckets, centered at 'centerGrid'.
    public struct NearbyGridCellIterator<T> 
         where T : struct
    {
        private int2 centerGrid;
        private NativeMultiHashMap<int2, T> grid;
        private MultiHashMapIterator<int2, T> it;
        private int o_x, o_y;

        public NearbyGridCellIterator<T> GetEnumerator() => this;

        public NearbyGridCellIterator(int2 centerGrid, NativeMultiHashMap<int2, T> grid) {
            this.centerGrid = centerGrid;
            this.grid = grid;
            o_x = -1;
            o_y = -1;
            it = new MultiHashMapIterator<int2, T>(grid, centerGrid + new int2(o_x, o_y));
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
                it = new MultiHashMapIterator<int2, T>(grid, centerGrid + new int2(o_x, o_y));
            }

            return true;
        }

        public T Current => it.Current;
    }

    // Iterates over one bucket in a multi hash map.
    public struct MultiHashMapIterator<U, T> 
        where T : struct
        where U : struct, System.IEquatable<U>
    {
        private NativeMultiHashMap<U, T> hashMap;
        NativeMultiHashMapIterator<U> it;
        private bool found;
        public T current;
        private T nextItem;
        public MultiHashMapIterator<U, T> GetEnumerator() => this;

        public MultiHashMapIterator(NativeMultiHashMap<U, T> hashMap, U key) {
            this.hashMap = hashMap;

            found = hashMap.TryGetFirstValue(key, out nextItem, out it);

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
