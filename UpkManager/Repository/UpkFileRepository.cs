using System.IO;
using System.Linq;
using System.Threading.Tasks;

using UpkManager.Contracts;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile;
using UpkManager.Models.UpkFile.Tables;

namespace UpkManager.Repository {

  public sealed class UpkFileRepository : IUpkFileRepository {

    #region IUpkFileRepository Implementation
            
    public async Task<UnrealHeader> LoadUpkFile(string Filename) {
      byte[] data = await Task.Run(() => File.ReadAllBytes(Filename));

      ByteArrayReader reader = ByteArrayReader.CreateNew(data, 0);

      UnrealHeader header = new (reader) {
        FullFilename = Filename,
        FileSize     = data.LongLength
      };

      return header;
    }

    public async Task SaveUpkFile(UnrealHeader Header, string Filename) {
      if (Header == null) return;

      foreach(UnrealExportTableEntry export in Header.ExportTable.Where(export => export.UnrealObject == null)) 
                await export.ParseUnrealObject(false, false);

      FileStream stream = new (Filename, FileMode.Create);

      int headerSize = Header.GetBuilderSize();

      ByteArrayWriter writer = ByteArrayWriter.CreateNew(headerSize);

      await Header.WriteBuffer(writer, 0);

      await stream.WriteAsync(writer.GetBytes(), 0, headerSize);

      foreach(UnrealExportTableEntry export in Header.ExportTable) {
        ByteArrayWriter objectWriter = await export.WriteObjectBuffer();

        await stream.WriteAsync(objectWriter.GetBytes(), 0, objectWriter.Index);
      }

      await stream.FlushAsync();

      stream.Close();
    }

    #endregion IUpkFileRepository Implementation

  }

}
