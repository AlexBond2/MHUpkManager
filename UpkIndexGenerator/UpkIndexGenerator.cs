using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UpkManager.Contracts;
using UpkManager.Indexing; // use new UpkIndexingSystem
using UpkManager.Repository;

namespace UpkIndexGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=================================");
            Console.WriteLine("  UPK Index Database Generator");
            Console.WriteLine("  Marvel Heroes Omega");
            Console.WriteLine("=================================\n");

            string upkDirectory = args.Length > 0
                ? args[0]
                : @"d:\\marvel\\Upk\\Test\\";

            string outputDb = args.Length > 1
                ? args[1]
                : "mh152upk.db";

            if (!Directory.Exists(upkDirectory))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: Directory not found: {upkDirectory}");
                Console.ResetColor();
                Environment.Exit(1);
            }

            var upkFiles = Directory.GetFiles(upkDirectory, "*.upk", SearchOption.TopDirectoryOnly);
            Console.WriteLine($"Found {upkFiles.Length} UPK files\n");
            if (upkFiles.Length == 0) Environment.Exit(0);

            if (File.Exists(outputDb))
            {
                var backupName = $"{outputDb}.backup_{DateTime.Now:yyyyMMdd_HHmmss}";
                File.Move(outputDb, backupName, overwrite: true);
                Console.WriteLine($"Existing database backed up to: {backupName}\n");
            }

            IUpkFileRepository repository = new UpkFileRepository();
            var stopwatch = Stopwatch.StartNew();

            using var cts = new CancellationTokenSource();
            var ct = cts.Token;

            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("\nCancellation requested...");
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                await UpkIndexingSystem.InitializeDatabaseAsync();

                int totalFiles = upkFiles.Length;
                int currentFile = 0;

                Console.WriteLine("=== Phase 1: Collecting imports ===");
                foreach (var upk in upkFiles)
                {
                    ct.ThrowIfCancellationRequested();
                    currentFile++;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"[{currentFile}/{totalFiles}] Processing imports: {Path.GetFileName(upk)}");
                    Console.ResetColor();

                    await UpkIndexingSystem.CollectPackageImportsFromFileAsync(upk, repository, ct);
                }

                Console.WriteLine("=== Phase 2: Collecting object locations ===");
                currentFile = 0;
                foreach (var upk in upkFiles)
                {
                    ct.ThrowIfCancellationRequested();
                    currentFile++;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"[{currentFile}/{totalFiles}] Processing object locations: {Path.GetFileName(upk)}");
                    Console.ResetColor();

                    await UpkIndexingSystem.CollectObjectLocationsFromFileAsync(upk, repository, ct);
                }

                stopwatch.Stop();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✓ Index generation completed in {stopwatch.Elapsed.TotalMinutes:F2} minutes");
                Console.WriteLine($"  Database saved to: {Path.GetFullPath(outputDb)}");
                Console.WriteLine($"  File size: {new FileInfo(outputDb).Length / 1024 / 1024:F2} MB");
                Console.ResetColor();
            }
            catch (OperationCanceledException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nOperation was cancelled by user.");
                Console.ResetColor();
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
