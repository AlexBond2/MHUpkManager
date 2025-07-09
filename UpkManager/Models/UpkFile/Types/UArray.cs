using System.Collections.Generic;

namespace UpkManager.Models.UpkFile.Types
{
    public class UArray<T> : List<T>
    {
        public UArray()
        {
        }

        public UArray(int capacity) : base(capacity)
        {
        }

        public UArray(IEnumerable<T> collection) : base(collection)
        {
        }

        public override string ToString()
        {
            return $"<{typeof(T).Name}>[{Count}]";
        }
    }
}
