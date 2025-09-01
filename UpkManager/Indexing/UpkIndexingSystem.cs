using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using UpkManager.Contracts;
using UpkManager.Models.UpkFile;
using UpkManager.Models.UpkFile.Tables;

namespace UpkManager.Indexing
{

    /// <summary>
    /// Multi-location UPK indexing system that handles duplicate objects across multiple UPK files
    /// </summary>
    public class MultiLocationUpkIndexingSystem
    {
        #region Models and Context

        /// <summary>
        /// Main object definition - one record per unique object
        /// </summary>
        public class UpkObject
        {
            public int Id { get; set; }
            public string ObjectName { get; set; }      // Unique object identifier
            public string ClassName { get; set; }       // Object class type
            public string PackageName { get; set; }     // Package this object belongs to
            public string PackagePath { get; set; }     // Full hierarchical path
            public DateTime FirstSeen { get; set; }     // When first indexed
            public DateTime LastSeen { get; set; }      // When last found

            // Navigation property
            public virtual ICollection<UpkObjectLocation> Locations { get; set; } = new List<UpkObjectLocation>();
        }

        /// <summary>
        /// Object location - multiple locations per object
        /// </summary>
        public class UpkObjectLocation
        {
            public int Id { get; set; }
            public int UpkObjectId { get; set; }        // Foreign key to UpkObject
            public string UpkFilePath { get; set; }     // Full path to UPK file
            public string UpkFileName { get; set; }     // UPK filename for quick search
            public int ExportIndex { get; set; }        // Index in this UPK's ExportTable
            public string OuterName { get; set; }       // Outer reference in this specific UPK
            public DateTime FileLastModified { get; set; } // UPK file modification time
            public int Version { get; set; }            // UPK header.Version
            public int EngineVersion { get; set; }      // UPK header.EngineVersion
            public long FileSize { get; set; }          // UPK file size for preference ranking
            public bool IsPrimaryLocation { get; set; } // Mark preferred location (largest file, most recent, etc.)

            // Navigation property
            public virtual UpkObject UpkObject { get; set; }
        }

        /// <summary>
        /// Package import tracking for first pass
        /// </summary>
        public class PackageImportInfo
        {
            public string PackageName { get; set; }
            public string ObjectName { get; set; }
            public string ClassName { get; set; }
            public string SourceUpkFile { get; set; }
            public string FullObjectPath { get; set; }  // Package.Object for uniqueness
        }

        /// <summary>
        /// SQLite database context with relationships
        /// </summary>
        public class UpkIndexContext : DbContext
        {
            public DbSet<UpkObject> Objects { get; set; }
            public DbSet<UpkObjectLocation> Locations { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlite("Data Source=mh152upk.db");
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // Configure relationships
                modelBuilder.Entity<UpkObjectLocation>()
                    .HasOne(l => l.UpkObject)
                    .WithMany(o => o.Locations)
                    .HasForeignKey(l => l.UpkObjectId);

                // Indexes for fast searching
                modelBuilder.Entity<UpkObject>()
                    .HasIndex(o => o.ObjectName);
                modelBuilder.Entity<UpkObject>()
                    .HasIndex(o => o.ClassName);
                modelBuilder.Entity<UpkObject>()
                    .HasIndex(o => o.PackageName);
                modelBuilder.Entity<UpkObject>()
                    .HasIndex(o => new { o.PackageName, o.ObjectName })
                    .IsUnique(); // Ensure package.object uniqueness

                modelBuilder.Entity<UpkObjectLocation>()
                    .HasIndex(l => l.UpkFilePath);
                modelBuilder.Entity<UpkObjectLocation>()
                    .HasIndex(l => l.UpkFileName);
                modelBuilder.Entity<UpkObjectLocation>()
                    .HasIndex(l => l.IsPrimaryLocation);
            }
        }

        #endregion

        #region Event Args and Progress

        /// <summary>
        /// Extended progress information with deduplication stats
        /// </summary>
        public class IndexingProgress
        {
            public string Phase { get; set; }
            public int TotalFiles { get; set; }
            public int ProcessedFiles { get; set; }
            public int IndexedFiles { get; set; }
            public int SkippedFiles { get; set; }
            public string CurrentFile { get; set; }
            public int CollectedImports { get; set; }
            public int UniqueObjects { get; set; }      // Unique objects found
            public int TotalLocations { get; set; }     // Total locations (with duplicates)
            public int DuplicateObjects { get; set; }   // Objects found in multiple UPKs

            public double PercentComplete => TotalFiles > 0 ? (double)ProcessedFiles / TotalFiles * 100 : 0;
            public double AverageLocationsPerObject => UniqueObjects > 0 ? (double)TotalLocations / UniqueObjects : 0;
        }

        /// <summary>
        /// Event args when duplicates are found
        /// </summary>
        public class DuplicateFoundEventArgs : EventArgs
        {
            public string ObjectName { get; set; }
            public string PackageName { get; set; }
            public List<string> UpkFiles { get; set; } = new();
            public string PreferredLocation { get; set; }
        }

        /// <summary>
        /// Event args when processing is completed
        /// </summary>
        public class IndexingCompletedEventArgs : EventArgs
        {
            public int IndexedFiles { get; }
            public int SkippedFiles { get; }
            public int UniqueObjects { get; }
            public int TotalLocations { get; }
            public int DuplicateObjects { get; }
            public TimeSpan Duration { get; }

            public IndexingCompletedEventArgs(int indexedFiles, int skippedFiles, int uniqueObjects,
                int totalLocations, int duplicateObjects, TimeSpan duration)
            {
                IndexedFiles = indexedFiles;
                SkippedFiles = skippedFiles;
                UniqueObjects = uniqueObjects;
                TotalLocations = totalLocations;
                DuplicateObjects = duplicateObjects;
                Duration = duration;
            }
        }

        #endregion

        #region Multi-Location Background Indexer

        /// <summary>
        /// Multi-location background UPK indexer with deduplication
        /// </summary>
        public class MultiLocationBackgroundUpkIndexer
        {
            private readonly CancellationTokenSource _cancellationTokenSource;
            private readonly IProgress<IndexingProgress> _progress;
            private Task _indexingTask;

            // Collections for two-pass approach
            private readonly ConcurrentBag<PackageImportInfo> _packageImports = new();
            private readonly ConcurrentDictionary<string, HashSet<string>> _relevantObjects = new();
            private readonly ConcurrentDictionary<string, List<UpkObjectLocation>> _objectLocations = new();

            // Events
            public event EventHandler<DuplicateFoundEventArgs> DuplicateFound;
            public event EventHandler<IndexingCompletedEventArgs> IndexingCompleted;
            public event EventHandler<Exception> ErrorOccurred;

            // Configuration
            public int RequiredVersion { get; set; } = 868;
            public int RequiredEngineVersion { get; set; } = 10897;
            public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount / 2;

            public MultiLocationBackgroundUpkIndexer(IProgress<IndexingProgress> progress = null)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _progress = progress;
            }

            /// <summary>
            /// Start multi-location indexing
            /// </summary>
            public void StartIndexing(string upkDirectory, IUpkFileRepository repository)
            {
                _indexingTask = Task.Run(() => IndexUpkFilesAsync(upkDirectory, repository, _cancellationTokenSource.Token));
            }

            /// <summary>
            /// Main three-pass indexing process
            /// </summary>
            private async Task IndexUpkFilesAsync(string upkDirectory, IUpkFileRepository repository, CancellationToken cancellationToken)
            {
                var startTime = DateTime.Now;

                try
                {
                    // Initialize database
                    using (var context = new UpkIndexContext())
                    {
                        await context.Database.EnsureCreatedAsync(cancellationToken);
                    }

                    var upkFiles = Directory.GetFiles(upkDirectory, "*.upk", SearchOption.AllDirectories);

                    // PASS 1: Collect all package imports
                    await FirstPass_CollectPackageImports(upkFiles, repository, cancellationToken);

                    // Build lookup for relevant objects
                    BuildRelevantObjectsLookup();

                    // PASS 2: Collect all object locations (with duplicates)
                    var (indexedFiles, skippedFiles) = await SecondPass_CollectObjectLocations(upkFiles, repository, cancellationToken);

                    // PASS 3: Deduplicate and store in database
                    var (uniqueObjects, totalLocations, duplicateObjects) = await ThirdPass_DeduplicateAndStore(cancellationToken);

                    var duration = DateTime.Now - startTime;
                    IndexingCompleted?.Invoke(this, new IndexingCompletedEventArgs(
                        indexedFiles, skippedFiles, uniqueObjects, totalLocations, duplicateObjects, duration));
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke(this, ex);
                }
            }

            /// <summary>
            /// First pass: collect package imports (same as before)
            /// </summary>
            private async Task FirstPass_CollectPackageImports(string[] upkFiles, IUpkFileRepository repository, CancellationToken cancellationToken)
            {
                var processedFiles = 0;
                var totalFiles = upkFiles.Length;

                var options = new ParallelOptions
                {
                    CancellationToken = cancellationToken,
                    MaxDegreeOfParallelism = Math.Max(1, MaxDegreeOfParallelism)
                };

                await Parallel.ForEachAsync(upkFiles, options, async (upkFilePath, ct) =>
                {
                    try
                    {
                        await CollectPackageImportsFromFile(upkFilePath, repository, ct);

                        var currentProcessed = Interlocked.Increment(ref processedFiles);

                        _progress?.Report(new IndexingProgress
                        {
                            Phase = "Collecting Package Imports",
                            TotalFiles = totalFiles,
                            ProcessedFiles = currentProcessed,
                            CurrentFile = Path.GetFileName(upkFilePath),
                            CollectedImports = _packageImports.Count
                        });
                    }
                    catch (Exception ex)
                    {
                        ErrorOccurred?.Invoke(this, ex);
                        Interlocked.Increment(ref processedFiles);
                    }
                });
            }

            /// <summary>
            /// Second pass: collect all object locations (including duplicates)
            /// </summary>
            private async Task<(int indexedFiles, int skippedFiles)> SecondPass_CollectObjectLocations(
                string[] upkFiles, IUpkFileRepository repository, CancellationToken cancellationToken)
            {
                var processedFiles = 0;
                var indexedFiles = 0;
                var skippedFiles = 0;
                var totalFiles = upkFiles.Length;

                var options = new ParallelOptions
                {
                    CancellationToken = cancellationToken,
                    MaxDegreeOfParallelism = Math.Max(1, MaxDegreeOfParallelism)
                };

                await Parallel.ForEachAsync(upkFiles, options, async (upkFilePath, ct) =>
                {
                    try
                    {
                        var wasProcessed = await CollectObjectLocationsFromFile(upkFilePath, repository, ct);

                        if (wasProcessed)
                            Interlocked.Increment(ref indexedFiles);
                        else
                            Interlocked.Increment(ref skippedFiles);

                        var currentProcessed = Interlocked.Increment(ref processedFiles);

                        _progress?.Report(new IndexingProgress
                        {
                            Phase = "Collecting Object Locations",
                            TotalFiles = totalFiles,
                            ProcessedFiles = currentProcessed,
                            IndexedFiles = indexedFiles,
                            SkippedFiles = skippedFiles,
                            CurrentFile = Path.GetFileName(upkFilePath),
                            TotalLocations = _objectLocations.Values.Sum(list => list.Count)
                        });
                    }
                    catch (Exception ex)
                    {
                        ErrorOccurred?.Invoke(this, ex);
                        Interlocked.Increment(ref processedFiles);
                    }
                });

                return (indexedFiles, skippedFiles);
            }

            /// <summary>
            /// Collect object locations from single UPK file
            /// </summary>
            private async Task<bool> CollectObjectLocationsFromFile(string upkFilePath, IUpkFileRepository repository, CancellationToken cancellationToken)
            {
                var fileInfo = new FileInfo(upkFilePath);

                // Load UPK file
                UnrealHeader header;
                try
                {
                    header = await repository.LoadUpkFile(upkFilePath);
                    await Task.Run(() => header.ReadHeaderAsync(null), cancellationToken);
                }
                catch
                {
                    return false;
                }

                // Filter by version requirements
                if (header.Version != RequiredVersion || header.EngineVersion != RequiredEngineVersion)
                {
                    return false;
                }

                var packageName = Path.GetFileNameWithoutExtension(upkFilePath);

                // Check if this package has any relevant objects
                if (!_relevantObjects.ContainsKey(packageName))
                {
                    return false;
                }

                var relevantObjectNames = _relevantObjects[packageName];
                var foundObjects = false;

                // Collect locations for relevant exports
                for (int i = 0; i < header.ExportTable.Count; i++)
                {
                    var entry = header.ExportTable[i];
                    var objectName = entry.ObjectNameIndex.Name;

                    if (relevantObjectNames.Contains(objectName))
                    {
                        var objectKey = $"{packageName}.{objectName}";

                        var location = new UpkObjectLocation
                        {
                            UpkFilePath = upkFilePath,
                            UpkFileName = Path.GetFileName(upkFilePath),
                            ExportIndex = i,
                            OuterName = entry.OuterReferenceNameIndex?.Name,
                            FileLastModified = fileInfo.LastWriteTime,
                            Version = header.Version,
                            EngineVersion = (int)header.EngineVersion,
                            FileSize = fileInfo.Length
                        };

                        // Add to concurrent collection
                        _objectLocations.AddOrUpdate(
                            objectKey,
                            new List<UpkObjectLocation> { location },
                            (key, existing) =>
                            {
                                lock (existing) // Thread-safe list modification
                                {
                                    existing.Add(location);
                                    return existing;
                                }
                            }
                        );

                        foundObjects = true;
                    }
                }

                return foundObjects;
            }

            /// <summary>
            /// Third pass: deduplicate objects and store in database
            /// </summary>
            private async Task<(int uniqueObjects, int totalLocations, int duplicateObjects)> ThirdPass_DeduplicateAndStore(CancellationToken cancellationToken)
            {
                using var context = new UpkIndexContext();

                // Clear existing data
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Locations", cancellationToken);
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Objects", cancellationToken);

                var uniqueObjects = 0;
                var totalLocations = 0;
                var duplicateObjects = 0;

                foreach (var kvp in _objectLocations)
                {
                    var objectKey = kvp.Key;
                    var locations = kvp.Value;
                    var parts = objectKey.Split('.');
                    var packageName = parts[0];
                    var objectName = parts[1];

                    // Determine primary location (prefer largest file, most recent)
                    var primaryLocation = locations
                        .OrderByDescending(l => l.FileSize)
                        .ThenByDescending(l => l.FileLastModified)
                        .First();
                    primaryLocation.IsPrimaryLocation = true;

                    // Create unique object record
                    var upkObject = new UpkObject
                    {
                        ObjectName = objectName,
                        PackageName = packageName,
                        ClassName = GetClassNameFromImports(packageName, objectName), // From collected imports
                        PackagePath = $"{packageName}.{objectName}",
                        FirstSeen = DateTime.Now,
                        LastSeen = DateTime.Now
                    };

                    context.Objects.Add(upkObject);
                    await context.SaveChangesAsync(cancellationToken); // Get the ID

                    // Add all locations for this object
                    foreach (var location in locations)
                    {
                        location.UpkObjectId = upkObject.Id;
                        context.Locations.Add(location);
                    }

                    uniqueObjects++;
                    totalLocations += locations.Count;

                    if (locations.Count > 1)
                    {
                        duplicateObjects++;

                        // Fire duplicate found event
                        DuplicateFound?.Invoke(this, new DuplicateFoundEventArgs
                        {
                            ObjectName = objectName,
                            PackageName = packageName,
                            UpkFiles = locations.Select(l => l.UpkFileName).ToList(),
                            PreferredLocation = primaryLocation.UpkFileName
                        });
                    }

                    // Update progress
                    _progress?.Report(new IndexingProgress
                    {
                        Phase = "Deduplicating and Storing",
                        UniqueObjects = uniqueObjects,
                        TotalLocations = totalLocations,
                        DuplicateObjects = duplicateObjects
                    });
                }

                await context.SaveChangesAsync(cancellationToken);
                return (uniqueObjects, totalLocations, duplicateObjects);
            }

            private bool IsPackageOuter(UnrealHeader header, UnrealImportTableEntry importEntry)
            {
                if (importEntry.OuterReferenceNameIndex == null) return false;

                var outerRef = header.GetObjectTableEntry(importEntry.OuterReference);
                if (outerRef is not UnrealExportTableEntry entry) return false;

                return entry.ObjectNameIndex.Name?.Equals("Package", StringComparison.OrdinalIgnoreCase) == true;
            }

            private string GetPackageName(UnrealHeader header, UnrealImportTableEntry importEntry)
            {
                var outerRef = header.GetObjectTableEntry(importEntry.OuterReference);
                return outerRef.ObjectNameIndex?.Name;
            }

            /// <summary>
            /// Helper methods (implement based on your structure)
            /// </summary>
            private async Task CollectPackageImportsFromFile(string upkFilePath, IUpkFileRepository repository, CancellationToken cancellationToken)
            {
                UnrealHeader header;
                try
                {
                    header = await repository.LoadUpkFile(upkFilePath);
                    await Task.Run(() => header.ReadHeaderAsync(null), cancellationToken);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to load UPK {upkFilePath}: {ex.Message}");
                    return;
                }

                if (header.Version != RequiredVersion || header.EngineVersion != RequiredEngineVersion)
                    return;

                // Import Table
                foreach (var importEntry in header.ImportTable)
                {
                    if (IsPackageOuter(header, importEntry))
                    {
                        var packageName = GetPackageName(header, importEntry);
                        var objectName = importEntry.ObjectNameIndex.Name;
                        var className = importEntry.ClassNameIndex?.Name;

                        var packageImport = new PackageImportInfo
                        {
                            PackageName = packageName,
                            ObjectName = objectName,
                            ClassName = className,
                            SourceUpkFile = upkFilePath,
                            FullObjectPath = importEntry.GetPathName()
                        };

                        _packageImports.Add(packageImport);
                    }
                }
            }

            private void BuildRelevantObjectsLookup()
            {
                foreach (var import in _packageImports)
                {
                    if (!_relevantObjects.ContainsKey(import.PackageName))
                    {
                        _relevantObjects[import.PackageName] = new HashSet<string>();
                    }
                    _relevantObjects[import.PackageName].Add(import.ObjectName);
                }
            }

            private string GetClassNameFromImports(string packageName, string objectName)
            {
                return _packageImports
                    .FirstOrDefault(i => i.PackageName == packageName && i.ObjectName == objectName)
                    ?.ClassName ?? "Unknown";
            }

            public void Stop()
            {
                _cancellationTokenSource.Cancel();
            }

            public async Task WaitForCompletionAsync()
            {
                if (_indexingTask != null)
                {
                    await _indexingTask;
                }
            }
        }

        #endregion

        #region Enhanced Search Service

        /// <summary>
        /// Enhanced search service with multi-location support
        /// </summary>
        public class MultiLocationSearchService
        {
            private readonly ConcurrentDictionary<string, List<string>> _cache = new();

            /// <summary>
            /// Find all UPK files containing specified object
            /// </summary>
            public async Task<List<string>> FindAllObjectLocationsAsync(string objectName)
            {
                var cacheKey = $"all_{objectName}";
                if (_cache.TryGetValue(cacheKey, out var cachedPaths))
                    return cachedPaths;

                using var context = new UpkIndexContext();
                var locations = await context.Objects
                    .Where(o => o.ObjectName == objectName)
                    .SelectMany(o => o.Locations)
                    .Select(l => l.UpkFilePath)
                    .ToListAsync();

                _cache[cacheKey] = locations;
                return locations;
            }

            /// <summary>
            /// Find primary (preferred) location for object
            /// </summary>
            public async Task<string> FindPrimaryObjectLocationAsync(string objectName)
            {
                var cacheKey = $"primary_{objectName}";
                if (_cache.TryGetValue(cacheKey, out var cachedPaths) && cachedPaths.Any())
                    return cachedPaths.First();

                using var context = new UpkIndexContext();
                var primaryLocation = await context.Objects
                    .Where(o => o.ObjectName == objectName)
                    .SelectMany(o => o.Locations)
                    .Where(l => l.IsPrimaryLocation)
                    .Select(l => l.UpkFilePath)
                    .FirstOrDefaultAsync();

                if (primaryLocation != null)
                {
                    _cache[cacheKey] = new List<string> { primaryLocation };
                }

                return primaryLocation;
            }

            /// <summary>
            /// Get object with all its locations
            /// </summary>
            public async Task<ObjectWithLocations> GetObjectWithAllLocationsAsync(string objectName)
            {
                using var context = new UpkIndexContext();
                var obj = await context.Objects
                    .Include(o => o.Locations)
                    .FirstOrDefaultAsync(o => o.ObjectName == objectName);

                if (obj == null) return null;

                return new ObjectWithLocations
                {
                    ObjectName = obj.ObjectName,
                    PackageName = obj.PackageName,
                    ClassName = obj.ClassName,
                    Locations = obj.Locations.Select(l => new LocationInfo
                    {
                        UpkFilePath = l.UpkFilePath,
                        UpkFileName = l.UpkFileName,
                        ExportIndex = l.ExportIndex,
                        FileSize = l.FileSize,
                        IsPrimary = l.IsPrimaryLocation,
                        LastModified = l.FileLastModified
                    }).OrderByDescending(l => l.IsPrimary).ThenByDescending(l => l.FileSize).ToList()
                };
            }

            /// <summary>
            /// Find duplicate objects (objects in multiple UPKs)
            /// </summary>
            public async Task<List<DuplicateObjectInfo>> FindDuplicateObjectsAsync()
            {
                using var context = new UpkIndexContext();
                var duplicates = await context.Objects
                    .Include(o => o.Locations)
                    .Where(o => o.Locations.Count > 1)
                    .Select(o => new DuplicateObjectInfo
                    {
                        ObjectName = o.ObjectName,
                        PackageName = o.PackageName,
                        LocationCount = o.Locations.Count,
                        Locations = o.Locations.Select(l => l.UpkFileName).ToList(),
                        PrimaryLocation = o.Locations.Where(l => l.IsPrimaryLocation).Select(l => l.UpkFileName).FirstOrDefault()
                    })
                    .ToListAsync();

                return duplicates;
            }

            /// <summary>
            /// Get statistics about duplicates
            /// </summary>
            public async Task<DuplicationStats> GetDuplicationStatsAsync()
            {
                using var context = new UpkIndexContext();

                var totalObjects = await context.Objects.CountAsync();
                var duplicateObjects = await context.Objects.CountAsync(o => o.Locations.Count > 1);
                var totalLocations = await context.Locations.CountAsync();
                var maxLocationsPerObject = await context.Objects.MaxAsync(o => o.Locations.Count);

                return new DuplicationStats
                {
                    TotalUniqueObjects = totalObjects,
                    DuplicateObjects = duplicateObjects,
                    TotalLocations = totalLocations,
                    MaxLocationsPerObject = maxLocationsPerObject,
                    AverageLocationsPerObject = totalObjects > 0 ? (double)totalLocations / totalObjects : 0,
                    DuplicationRate = totalObjects > 0 ? (double)duplicateObjects / totalObjects * 100 : 0
                };
            }
        }

        /// <summary>
        /// Object with all its locations
        /// </summary>
        public class ObjectWithLocations
        {
            public string ObjectName { get; set; }
            public string PackageName { get; set; }
            public string ClassName { get; set; }
            public List<LocationInfo> Locations { get; set; } = new();
        }

        /// <summary>
        /// Location information
        /// </summary>
        public class LocationInfo
        {
            public string UpkFilePath { get; set; }
            public string UpkFileName { get; set; }
            public int ExportIndex { get; set; }
            public long FileSize { get; set; }
            public bool IsPrimary { get; set; }
            public DateTime LastModified { get; set; }
        }

        /// <summary>
        /// Duplicate object information
        /// </summary>
        public class DuplicateObjectInfo
        {
            public string ObjectName { get; set; }
            public string PackageName { get; set; }
            public int LocationCount { get; set; }
            public List<string> Locations { get; set; } = new();
            public string PrimaryLocation { get; set; }
        }

        /// <summary>
        /// Statistics about object duplication
        /// </summary>
        public class DuplicationStats
        {
            public int TotalUniqueObjects { get; set; }
            public int DuplicateObjects { get; set; }
            public int TotalLocations { get; set; }
            public int MaxLocationsPerObject { get; set; }
            public double AverageLocationsPerObject { get; set; }
            public double DuplicationRate { get; set; }
        }

        #endregion

        #region Public API

        public static MultiLocationBackgroundUpkIndexer CreateMultiLocationIndexer(IProgress<IndexingProgress> progress = null)
        {
            return new MultiLocationBackgroundUpkIndexer(progress);
        }

        public static MultiLocationSearchService CreateMultiLocationSearchService()
        {
            return new MultiLocationSearchService();
        }

        public static async Task InitializeDatabaseAsync()
        {
            using var context = new UpkIndexContext();
            await context.Database.EnsureCreatedAsync();
        }

        #endregion
    }
}
