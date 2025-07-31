
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Objects;

namespace UpkManager.Models.UpkFile.Tables
{
    public class FObject(UnrealObjectTableEntryBase tableEntry) : FName
    {
        public UObject Object { get; private set; }
        public UnrealObjectTableEntryBase TableEntry { get; set; } = tableEntry;

        public T LoadObject<T>() where T : UObject
        {
            if (Object is T cached)
                return cached;

            if (TableEntry is UnrealExportTableEntry export)
            {
                if (export.UnrealObject == null)
                    export.ParseUnrealObject(false, false).GetAwaiter().GetResult();

                if (export.UnrealObject is IUnrealObject uObject)
                {
                    Object = uObject.UObject as T;
                    return Object as T;
                }
            }

            return default;
        }
    }
}
