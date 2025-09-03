using Microsoft.EntityFrameworkCore;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using UpkManager.Contracts;
using UpkManager.Indexing;
using UpkManager.Models.UpkFile;

namespace UpkIndexGenerator
{
    public static class UpkIndexingSystem
    {
        #region EF Models & Context

        public class PackageImportInfo
        {
            public int Id { get; set; }
            public string FullObjectPath { get; set; }
            public string PackageName { get; set; }
            public string ObjectName { get; set; }
            public string ClassName { get; set; }
            public string SourceUpkFile { get; set; }
        }

        public class UpkObjectLocation
        {
            public int Id { get; set; }
            public string ObjectPath { get; set; }
            public string UpkFileName { get; set; }
            public int ExportIndex { get; set; }
            public long FileSize { get; set; }
        }

        /// <summary>
        /// Tracks which UPK files were fully scanned.
        /// </summary>
        public class ScannedFile
        {
            public int Id { get; set; }
            public string FileName { get; set; }
            public long FileSize { get; set; }
            public DateTime LastScannedAt { get; set; }
            public bool ImportsDone { get; set; }
            public bool ExportsDone { get; set; }
        }

        public class UpkIndexContext(string dbPath) : DbContext
        {
            private readonly string dbPath = dbPath;
            public DbSet<PackageImportInfo> PackageImports { get; set; }
            public DbSet<UpkObjectLocation> ObjectLocations { get; set; }
            public DbSet<ScannedFile> ScannedFiles { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<PackageImportInfo>()
                    .HasIndex(p => p.FullObjectPath);

                modelBuilder.Entity<UpkObjectLocation>()
                    .HasIndex(o => o.ObjectPath);

                modelBuilder.Entity<ScannedFile>()
                    .HasIndex(f => f.FileName)
                    .IsUnique();
            }
        }

        #endregion

        #region Configuration

        public static string DbPath { get; set; } = "mh152upk.db";
        public static int RequiredVersion { get; set; } = 868;
        public static int RequiredEngineVersion { get; set; } = 10897;

        #endregion

        #region Pass 1 — Imports

        public static async Task CollectPackageImportsFromFileAsync(string upkFilePath, IUpkFileRepository repository, CancellationToken ct)
        {
            using var context = new UpkIndexContext(DbPath);

            var fileInfo = new FileInfo(upkFilePath);
            var fileName = fileInfo.Name;

            var alreadyScanned = await context.ScannedFiles
                .AnyAsync(f => f.FileName == fileName && f.ImportsDone, ct);
            if (alreadyScanned) return;

            UnrealHeader header;
            try
            {
                header = await repository.LoadUpkFile(upkFilePath);
                await Task.Run(() => header.ReadHeaderAsync(null), ct);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Imports] Failed to load UPK {upkFilePath}: {ex.Message}");
                return;
            }

            if (header.Version != RequiredVersion || header.EngineVersion != RequiredEngineVersion)
                return;

            foreach (var importEntry in header.ImportTable)
            {
                if (!UpkFilePackageSystem.IsPackageOuter(header, importEntry))
                    continue;

                var packageName = UpkFilePackageSystem.GetPackageName(header, importEntry);
                var fullPath = importEntry.GetPathName().ToLowerInvariant();
                if (string.IsNullOrEmpty(fullPath))
                    continue;

                context.PackageImports.Add(new PackageImportInfo
                {
                    FullObjectPath = fullPath,
                    PackageName = packageName,
                    ObjectName = importEntry.ObjectNameIndex?.Name,
                    ClassName = importEntry.ClassNameIndex?.Name,
                    SourceUpkFile = fileName
                });
            }

            // Mark file as scanned only after imports saved
            var scannedFile = context.ScannedFiles.FirstOrDefault(f => f.FileName == fileName);
            if (scannedFile != null)
            {
                scannedFile.ImportsDone = true;
                scannedFile.LastScannedAt = DateTime.UtcNow;
            }
            else
            {
                context.ScannedFiles.Add(new ScannedFile
                {
                    FileName = fileName,
                    FileSize = fileInfo.Length,
                    ImportsDone = true,
                    LastScannedAt = DateTime.UtcNow
                });
            }

            await context.SaveChangesAsync(ct);
        }

        #endregion

        #region Pass 2 — Export Locations

        public static async Task CollectObjectLocationsFromFileAsync(string upkFilePath, IUpkFileRepository repository, CancellationToken ct)
        {
            using var context = new UpkIndexContext(DbPath);

            var fileInfo = new FileInfo(upkFilePath);
            var fileName = fileInfo.Name;

            var alreadyScanned = await context.ScannedFiles
                .AnyAsync(f => f.FileName == fileName && f.ExportsDone, ct);
            if (alreadyScanned) return;

            UnrealHeader header;
            try
            {
                header = await repository.LoadUpkFile(upkFilePath);
                await Task.Run(() => header.ReadHeaderAsync(null), ct);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Locations] Failed to load UPK {upkFilePath}: {ex.Message}");
                return;
            }

            if (header.Version != RequiredVersion || header.EngineVersion != RequiredEngineVersion)
                return;

            var exportPaths = header.ExportTable
                .Select(e => e?.GetPathName())
                .Where(p => !string.IsNullOrEmpty(p))
                .Select(p => p.ToLowerInvariant())
                .Distinct()
                .ToList();

            if (exportPaths.Count == 0)
                return;

            var relevantPaths = await context.PackageImports
                .Where(p => exportPaths.Contains(p.FullObjectPath))
                .Select(p => p.FullObjectPath)
                .Distinct()
                .ToListAsync(ct);

            if (relevantPaths.Count == 0)
                return;

            var relevantSet = relevantPaths.ToHashSet();

            foreach (var entry in header.ExportTable)
            {
                var fullPath = entry?.GetPathName().ToLowerInvariant();
                if (string.IsNullOrEmpty(fullPath) || !relevantSet.Contains(fullPath))
                    continue;

                var exists = await context.ObjectLocations
                    .AnyAsync(o => o.ObjectPath == fullPath
                                && o.UpkFileName == fileName 
                                && o.ExportIndex == entry.TableIndex, ct);
                if (exists) continue;

                context.ObjectLocations.Add(new UpkObjectLocation
                {
                    ObjectPath = fullPath,
                    UpkFileName = fileName,
                    ExportIndex = entry.TableIndex,
                    FileSize = fileInfo.Length
                });
            }

            var scannedFile = context.ScannedFiles.FirstOrDefault(f => f.FileName == fileName);
            if (scannedFile != null)
            {
                scannedFile.ExportsDone = true;
                scannedFile.LastScannedAt = DateTime.UtcNow;
            }
            else
            {
                context.ScannedFiles.Add(new ScannedFile
                {
                    FileName = fileName,
                    FileSize = fileInfo.Length,
                    ExportsDone = true,
                    LastScannedAt = DateTime.UtcNow
                });
            }

            await context.SaveChangesAsync(ct);
        }

        #endregion

        #region Convenience API

        public static async Task InitializeDatabaseAsync(CancellationToken ct = default)
        {
            using var context = new UpkIndexContext(DbPath);
            await context.Database.EnsureCreatedAsync(ct);
        }

        public static void Convert(string outputMessagePack)
        {
            // Open SQLite context
            using var context = new UpkIndexContext(DbPath);
            // Create new MessagePack database
            var ufps = new UpkFilePackageSystem(outputMessagePack, createNew: true);

            // Read all object locations from SQLite
            var locations = context.ObjectLocations
                                   .Select(o => new
                                   {
                                       o.ObjectPath,
                                       o.UpkFileName,
                                       o.ExportIndex,
                                       o.FileSize
                                   })
                                   .ToList();

            Console.WriteLine($"Converting {locations.Count} object locations to MessagePack...");

            // Add each mapping
            foreach (var loc in locations)
            {
                ufps.AddMapping(loc.ObjectPath, loc.UpkFileName, loc.ExportIndex, loc.FileSize);
            }

            // Save MessagePack file
            ufps.Save();
            Console.WriteLine($"Conversion finished. MessagePack saved to '{outputMessagePack}'");
        }

        #endregion
    }
}
