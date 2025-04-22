
using UpkManager.Models.UpkFile.Tables;

namespace MHUpkManager
{
    public abstract class ViewEntities
    {
        public static object GetDataSource(List<UnrealImportTableEntry> importTable)
        {
            var data = importTable.Select(entry => new
            {
                Index = entry.TableIndex,
                Object = entry.NameTableIndex?.Name,
                Class = entry.TypeNameIndex?.Name,
                Package = entry.PackageNameIndex?.Name,
                Group = entry.OwnerReferenceNameIndex?.Name
            }).ToList();

            return data;
        }

        public static object GetDataSource(List<UnrealExportTableEntry> exportTable)
        {
            var data = exportTable.Select(entry => new
            {
                Index = entry.TableIndex,
                Object = entry.NameTableIndex?.Name,
                Class = entry.TypeReferenceNameIndex?.Name,
                Pakage = entry.OwnerReferenceNameIndex?.Name,
                Group = entry.ParentReferenceNameIndex?.Name,
                Flags = $"0x{entry.FlagsLow:X8}-0x{entry.FlagsHigh:X8}",
                Size = entry.SerialDataSize,
                Offset = entry.SerialDataOffset
            }).ToList();

            return data;
        }

        public static object GetDataSource(List<UnrealNameTableEntry> nameTable)
        {
            var data = nameTable.Select(entry => new
            {
                Index = entry.TableIndex,
                Name = entry.Name.String,
                Flags = $"0x{entry.Flags:X16}"
            }).ToList();

            return data;
        }
    }
}
