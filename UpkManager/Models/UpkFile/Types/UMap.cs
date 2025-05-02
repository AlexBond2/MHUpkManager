using System.Collections.Generic;

namespace UpkManager.Models.UpkFile.Types
{
    public class UMap<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public UMap() : base()
        {

        }

        public UMap(int capacity) : base(capacity)
        {

        }

        public override string ToString()
        {
            return $"<{typeof(TKey).Name}, {typeof(TValue).Name}>[{Count}]";
        }

    }
}
