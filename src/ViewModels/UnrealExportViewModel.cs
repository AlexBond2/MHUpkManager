using System.ComponentModel;
using UpkManager.Models.UpkFile.Objects;
using UpkManager.Models.UpkFile.Tables;

namespace MHUpkManager.Models
{
    public class UnrealExportViewModel
    {
        [Category("General")]
        public int Index { get; }

        [Category("General")]
        public string Object { get; }

        [Category("General")]
        public string Class { get; }

        [Category("General")]
        public string Super { get; }

        [Category("General")]
        public string Outer { get; }

        [Category("General")]
        public string Archetype { get; }

        [Category("Flags")]
        public EObjectFlags ObjectFlags { get; }

        [Category("Flags")]
        public ExportFlags ExportFlags { get; }

        [Category("Pakage")]
        public int SerialSize { get; }

        [Category("Pakage")]
        public int SerialOffset { get; }

        [Category("Pakage")]
        public Guid PackageGuid { get; }

        [Category("Pakage")]
        public EPackageFlags PackageFlags { get; }

        [Category("Pakage")]
        public Int32[] NetObjects { get; }

        public UnrealExportViewModel(UnrealExportTableEntry entry)
        {
            Index = entry.TableIndex;
            Object = entry.ObjectNameIndex?.Name;
            Class = entry.ClassReferenceNameIndex?.Name;
            Super = entry.SuperReferenceNameIndex?.Name;
            Outer = entry.OuterReferenceNameIndex?.Name;
            Archetype = entry.ArchetypeReferenceNameIndex?.Name;
            ObjectFlags = (EObjectFlags)entry.ObjectFlags;
            ExportFlags = (ExportFlags)entry.ExportFlags;
            SerialSize = entry.SerialDataSize;
            SerialOffset = entry.SerialDataOffset;
            PackageGuid = new(entry.PackageGuid);
            PackageFlags = (EPackageFlags)entry.PackageFlags;
            NetObjects = entry.NetObjects.Select(i => new Int32(i)).ToArray();
        }
    }
}
