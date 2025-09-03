using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using UpkManager.Contracts;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Indexing;
using System;

namespace UpkManager.Repository {

  public sealed class UpkFileRepository : IUpkFileRepository 
  {
        private readonly Dictionary<string, UnrealHeader> _headerCache = [];
        private readonly Queue<string> _cacheOrder = new();
        private const int MaxCacheSize = 10;

        public UpkFilePackageSystem PackageIndex { get; private set; }

        #region IUpkFileRepository Implementation

        public async Task<UnrealHeader> LoadUpkFile(string filename)
        {
            if (_headerCache.TryGetValue(filename, out var cachedHeader))
                return cachedHeader;

            byte[] data = await Task.Run(() => File.ReadAllBytes(filename));

            var reader = ByteArrayReader.CreateNew(data, 0);

            UnrealHeader header = new (reader) 
            {
                FullFilename = filename,
                FileSize     = data.LongLength,
                Repository   = this
            };

            AddToCache(filename, header);

            return header;
        }

        private void AddToCache(string fullPath, UnrealHeader header)
        {
            if (_headerCache.ContainsKey(fullPath))
                return;

            if (_headerCache.Count >= MaxCacheSize)
            {
                string oldestKey = _cacheOrder.Dequeue();
                _headerCache.Remove(oldestKey);
            }

            _headerCache[fullPath] = header;
            _cacheOrder.Enqueue(fullPath);
        }

        public async Task SaveUpkFile(UnrealHeader Header, string Filename) 
        {
            if (Header == null) return;

            foreach(UnrealExportTableEntry export in Header.ExportTable.Where(export => export.UnrealObject == null)) 
                    await export.ParseUnrealObject(false, false);

            FileStream stream = new (Filename, FileMode.Create);

            int headerSize = Header.GetBuilderSize();

            ByteArrayWriter writer = ByteArrayWriter.CreateNew(headerSize);

            await Header.WriteBuffer(writer, 0);

            await stream.WriteAsync(writer.GetBytes(), 0, headerSize);

            foreach(UnrealExportTableEntry export in Header.ExportTable) 
            {
                ByteArrayWriter objectWriter = await export.WriteObjectBuffer();

                await stream.WriteAsync(objectWriter.GetBytes(), 0, objectWriter.Index);
            }

            await stream.FlushAsync();

            stream.Close();
        }

        public void LoadPackageIndex(string indexPath)
        {
            PackageIndex = UpkFilePackageSystem.LoadFromFile(indexPath);
        }

        public UnrealObjectTableEntryBase GetExportEntry(string pathName, string root)
        {
            return Task.Run(async () =>
                {
                    UnrealObjectTableEntryBase entry = null;

                    var location = PackageIndex?.GetFirstLocation(pathName, LocationFilter.MinSize);
                    if (location == null) return entry;

                    string fullPath = Path.Combine(root, location.UpkFileName);
                    int exportIndex = location.ExportIndex;

                    var header = await LoadUpkFile(fullPath);            
                    await header.ReadHeaderAsync(null);

                    entry = header.ExportTable.Find( e => e.TableIndex ==  exportIndex);
                    return entry;
                }).GetAwaiter().GetResult();
        }

        #endregion IUpkFileRepository Implementation

    }

}
