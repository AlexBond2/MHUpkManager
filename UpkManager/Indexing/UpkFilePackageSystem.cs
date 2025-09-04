using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UpkManager.Models.UpkFile;
using UpkManager.Models.UpkFile.Tables;

namespace UpkManager.Indexing
{
    public class UpkFilePackageSystem
    {
        private readonly string dbFilePath;
        private PackageData data;
        private bool isDirty = false;

        [MessagePackObject]
        public class PackageData
        {
            [MessagePack.Key(0)]
            public Dictionary<string, List<LocationEntry>> ObjectPathMap { get; set; }

            public PackageData()
            {
                ObjectPathMap = [];
            }
        }

        [MessagePackObject]
        public class LocationEntry
        {
            [MessagePack.Key(0)]
            public string UpkFileName { get; set; }

            [MessagePack.Key(1)]
            public int ExportIndex { get; set; }

            [MessagePack.Key(2)]
            public long FileSize { get; set; }

            public LocationEntry() { }

            public LocationEntry(string upkFileName, int exportIndex, long fileSize)
            {
                UpkFileName = upkFileName;
                ExportIndex = exportIndex;
                FileSize = fileSize;
            }
        }

        /// <summary>
        /// Create a new database or load an existing one from file
        /// </summary>
        /// <param name="dbFilePath">Path to the MessagePack file</param>
        /// <param name="createNew">If true, a new database is created, old one is overwritten</param>
        public UpkFilePackageSystem(string dbFilePath, bool createNew = false)
        {
            this.dbFilePath = dbFilePath;

            if (createNew || !File.Exists(dbFilePath))
            {
                data = new PackageData();
                isDirty = true;
            }
            else
            {
                LoadFromDisk();
            }
        }

        /// <summary>
        /// Load an existing database from file
        /// </summary>
        public static UpkFilePackageSystem LoadFromFile(string dbFilePath)
        {
            if (!File.Exists(dbFilePath))
                throw new FileNotFoundException("Database file not found.", dbFilePath);

            return new UpkFilePackageSystem(dbFilePath, createNew: false);
        }

        /// <summary>
        /// Load data from disk
        /// </summary>
        private void LoadFromDisk()
        {
            try
            {
                var bytes = File.ReadAllBytes(dbFilePath);
                data = MessagePackSerializer.Deserialize<PackageData>(bytes);

                data ??= new PackageData();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load database from {dbFilePath}", ex);
            }
        }

        /// <summary>
        /// Save data to disk
        /// </summary>
        private void SaveToDisk()
        {
            try
            {
                var bytes = MessagePackSerializer.Serialize(data);

                // Write to temp file first for safety
                var tempFile = dbFilePath + ".tmp";
                File.WriteAllBytes(tempFile, bytes);

                // Move temp file to actual file (atomic on most filesystems)
                if (File.Exists(dbFilePath))
                {
                    File.Delete(dbFilePath);
                }
                File.Move(tempFile, dbFilePath);

                isDirty = false;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save database to {dbFilePath}", ex);
            }
        }

        /// <summary>
        /// Get all UPK file names containing the given object path
        /// </summary>
        public List<string> GetUpkFiles(string packagePath)
        {
            if (!data.ObjectPathMap.TryGetValue(packagePath, out var locations))
            {
                return [];
            }

            return locations.Select(x => x.UpkFileName).Distinct().ToList();
        }

        /// <summary>
        /// Get all location entries for a given object path
        /// </summary>
        public List<LocationEntry> GetLocations(string packagePath)
        {
            if (!data.ObjectPathMap.TryGetValue(packagePath, out var locations))
            {
                return [];
            }

            return [.. locations];
        }

        public LocationEntry GetFirstLocation(string packagePath, LocationFilter filter)
        {
            if (!data.ObjectPathMap.TryGetValue(packagePath, out var locations) 
                || locations == null 
                || locations.Count == 0)
                return null;

            return filter switch
            {
                LocationFilter.MinSize => locations.OrderBy(x => x.FileSize).FirstOrDefault(),
                LocationFilter.MaxSize => locations.OrderByDescending(x => x.FileSize).FirstOrDefault(),
                _ => locations.FirstOrDefault(),
            };
        }

        /// <summary>
        /// Add a full mapping: ObjectPath -> UPK file -> location
        /// </summary>
        public void AddMapping(string objectPath, string upkFileName, int exportIndex, long fileSize)
        {
            if (!data.ObjectPathMap.TryGetValue(objectPath, out var locations))
            {
                locations = [];
                data.ObjectPathMap[objectPath] = locations;
            }

            // Check for duplicate
            var exists = locations.Any(l =>
                l.UpkFileName == upkFileName &&
                l.ExportIndex == exportIndex);

            if (!exists)
            {
                locations.Add(new LocationEntry(upkFileName, exportIndex, fileSize));
                isDirty = true;
            }
        }

        /// <summary>
        /// Get statistics about the database
        /// </summary>
        public (int ObjectPathCount, int TotalMappings, int UniqueFiles) GetStatistics()
        {
            var objectPathCount = data.ObjectPathMap.Count;
            var totalMappings = data.ObjectPathMap.Values.Sum(list => list.Count);
            var uniqueFiles = data.ObjectPathMap.Values
                .SelectMany(list => list.Select(l => l.UpkFileName))
                .Distinct()
                .Count();

            return (objectPathCount, totalMappings, uniqueFiles);
        }

        /// <summary>
        /// Check if an object path exists in the database
        /// </summary>
        public bool ContainsObjectPath(string objectPath)
        {
            return data.ObjectPathMap.ContainsKey(objectPath);
        }

        /// <summary>
        /// Get all object paths in the database
        /// </summary>
        public List<string> GetAllObjectPaths()
        {
            return data.ObjectPathMap.Keys.ToList();
        }

        /// <summary>
        /// Get all unique UPK file names in the database
        /// </summary>
        public List<string> GetAllUpkFiles()
        {
            return data.ObjectPathMap.Values
                .SelectMany(list => list.Select(l => l.UpkFileName))
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Remove an object path and all its mappings
        /// </summary>
        public bool RemoveObjectPath(string objectPath)
        {
            if (data.ObjectPathMap.Remove(objectPath))
            {
                isDirty = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clear all data
        /// </summary>
        public void Clear()
        {
            data.ObjectPathMap.Clear();
            isDirty = true;
        }

        /// <summary>
        /// Save and commit changes to the database
        /// </summary>
        public void Save()
        {
            if (isDirty)
            {
                SaveToDisk();
            }
        }

        #region Helpers

        public static bool IsPackageOuter(UnrealHeader header, UnrealImportTableEntry importEntry)
        {
            if (importEntry?.OuterReferenceNameIndex == null)
                return false;

            var outerRef = header.GetObjectTableEntry(importEntry.OuterReference);
            string className = null;
            if (outerRef is UnrealExportTableEntry outerExport)
                className = outerExport.ClassReferenceNameIndex?.Name;
            else if(outerRef is UnrealImportTableEntry import)
                className = import.ClassNameIndex?.Name;

            return className != null && className.Equals("Package", StringComparison.OrdinalIgnoreCase);
        }

        public static string GetPackageName(UnrealHeader header, UnrealImportTableEntry importEntry)
        {
            var outerRef = header.GetObjectTableEntry(importEntry.OuterReference);
            return outerRef?.ObjectNameIndex?.Name;
        }

        #endregion
    }

    public enum LocationFilter
    {
        MinSize,
        MaxSize
    }
}