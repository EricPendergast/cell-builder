using Unity.Collections;
using Unity.Mathematics;

namespace Utilities {
    // These iterators work in burst compiled foreach loops. (Because
    // they don't implement IDisposable)

    // Treats the hash map like a grid, and itertes over a 3x3 region
    // of buckets, centered at 'centerGrid'.
    public struct NearbyGridCellIterator<T> where T : struct {
        private int2 centerGrid;
        private NativeMultiHashMap<int, T> grid;
        private MultiHashMapIterator<T> it;
        private int o_x, o_y;

        public NearbyGridCellIterator<T> GetEnumerator() => this;

        public NearbyGridCellIterator(int2 centerGrid, NativeMultiHashMap<int, T> grid) {
            this.centerGrid = centerGrid;
            this.grid = grid;
            o_x = -1;
            o_y = -1;
            it = new MultiHashMapIterator<T>(grid, (int)math.hash(centerGrid + new int2(o_x, o_y)));
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
                it = new MultiHashMapIterator<T>(grid, (int)math.hash(centerGrid + new int2(o_x, o_y)));
            }

            return true;
        }

        public T Current => it.Current;
    }

    // Iterates over one bucket in a multi hash map.
    public struct MultiHashMapIterator<T> where T : struct {
        private NativeMultiHashMap<int, T> hashMap;
        NativeMultiHashMapIterator<int> it;
        private bool found;
        public T current;
        private T nextItem;
        public MultiHashMapIterator<T> GetEnumerator() => this;

        public MultiHashMapIterator(NativeMultiHashMap<int, T> hashMap, int hash) {
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
