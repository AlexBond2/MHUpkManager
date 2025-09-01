using System.Diagnostics;
using System.Threading.Tasks;
using System;
using System.IO;

using UpkManager.Contracts;
using UpkManager.Repository;
using static UpkManager.Indexing.MultiLocationUpkIndexingSystem;


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

            // Parse command line arguments or use defaults
            string upkDirectory = args.Length > 0 
                ? args[0] 
                : @"d:\marvel\Upk\Test\";
            
            string outputDb = args.Length > 1 
                ? args[1] 
                : "mh152upk.db";

            // Validate input directory
            if (!Directory.Exists(upkDirectory))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: Directory not found: {upkDirectory}");
                Console.ResetColor();
                Console.WriteLine("\nUsage: UpkIndexGenerator.exe [upk_directory] [output_db]");
                Console.WriteLine("Example: UpkIndexGenerator.exe \"C:\\MHO\\UnrealEngine3\\MarvelGame\\CookedPCConsole\\\" \"mh152upk.db\"");
                Environment.Exit(1);
            }

            // Count UPK files
            Console.WriteLine($"Scanning directory: {upkDirectory}");
            var upkFiles = Directory.GetFiles(upkDirectory, "*.upk", SearchOption.AllDirectories);
            Console.WriteLine($"Found {upkFiles.Length} UPK files\n");

            if (upkFiles.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning: No UPK files found in the specified directory");
                Console.ResetColor();
                Environment.Exit(0);
            }

            // Check if database already exists
            if (File.Exists(outputDb))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Warning: Database '{outputDb}' already exists.");
                Console.Write("Overwrite? (y/n): ");
                Console.ResetColor();
                
                var key = Console.ReadKey();
                Console.WriteLine();
                
                if (key.Key != ConsoleKey.Y)
                {
                    Console.WriteLine("Operation cancelled.");
                    Environment.Exit(0);
                }
                
                // Backup existing database
                var backupName = $"{outputDb}.backup_{DateTime.Now:yyyyMMdd_HHmmss}";
                File.Move(outputDb, backupName);
                Console.WriteLine($"Existing database backed up to: {backupName}\n");
            }

            // Initialize repository
            IUpkFileRepository repository = new UpkFileRepository();
            
            // Create and start indexing
            var stopwatch = Stopwatch.StartNew();
            var generator = new IndexGenerator(repository);
            
            try
            {
                await generator.GenerateIndexAsync(upkDirectory, outputDb);
                
                stopwatch.Stop();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✓ Index generation completed in {stopwatch.Elapsed.TotalMinutes:F2} minutes");
                Console.WriteLine($"  Database saved to: {Path.GetFullPath(outputDb)}");
                Console.WriteLine($"  File size: {new FileInfo(outputDb).Length / 1024 / 1024:F2} MB");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n✗ Index generation failed after {stopwatch.Elapsed.TotalMinutes:F2} minutes");
                Console.WriteLine($"  Error: {ex.Message}");
                Console.WriteLine($"  Stack trace: {ex.StackTrace}");
                Console.ResetColor();
                Environment.Exit(1);
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }

    /// <summary>
    /// Main index generator class
    /// </summary>
    public class IndexGenerator(IUpkFileRepository repository)
    {
        private readonly IUpkFileRepository _repository = repository;
        private MultiLocationBackgroundUpkIndexer _indexer;
        private int _processedFiles = 0;
        private int _totalFiles = 0;
        private readonly object _consoleLock = new object();

        public async Task GenerateIndexAsync(string upkDirectory, string outputDb)
        {
            // Initialize database
            await InitializeDatabaseAsync();

            // Create progress reporter
            var progress = new Progress<IndexingProgress>(ReportProgress);

            // Create and configure indexer
            _indexer = CreateMultiLocationIndexer(progress);
            
            // Configure for Marvel Heroes Omega
            _indexer.RequiredVersion = 868;           // MHO version
            _indexer.RequiredEngineVersion = 10897;   // MHO engine version
            _indexer.MaxDegreeOfParallelism = Environment.ProcessorCount; // Use all cores for batch processing

            // Subscribe to events
            _indexer.DuplicateFound += OnDuplicateFound;
            _indexer.IndexingCompleted += OnIndexingCompleted;
            _indexer.ErrorOccurred += OnErrorOccurred;

            // Start indexing
            Console.WriteLine("Starting indexing process...\n");
            _indexer.StartIndexing(upkDirectory, _repository);

            // Wait for completion
            await _indexer.WaitForCompletionAsync();
        }

        private void ReportProgress(IndexingProgress progress)
        {
            lock (_consoleLock)
            {
                _processedFiles = progress.ProcessedFiles;
                _totalFiles = progress.TotalFiles;

                // Move cursor to beginning of line and clear it
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.WindowWidth - 1));
                Console.SetCursorPosition(0, Console.CursorTop);

                // Display progress based on phase
                switch (progress.Phase)
                {
                    case "Collecting Package Imports":
                        Console.Write($"[Phase 1/3] Collecting imports: {progress.ProcessedFiles}/{progress.TotalFiles} " +
                                    $"({progress.PercentComplete:F1}%) | Imports: {progress.CollectedImports}");
                        break;

                    case "Collecting Object Locations":
                        Console.Write($"[Phase 2/3] Collecting locations: {progress.ProcessedFiles}/{progress.TotalFiles} " +
                                    $"({progress.PercentComplete:F1}%) | Indexed: {progress.IndexedFiles} | " +
                                    $"Locations: {progress.TotalLocations}");
                        break;

                    case "Deduplicating and Storing":
                        Console.Write($"[Phase 3/3] Deduplicating: Objects: {progress.UniqueObjects} | " +
                                    $"Locations: {progress.TotalLocations} | Duplicates: {progress.DuplicateObjects}");
                        break;
                }
            }
        }

        private void OnDuplicateFound(object sender, DuplicateFoundEventArgs e)
        {
            // Only log significant duplicates (found in 3+ files)
            if (e.UpkFiles.Count >= 3)
            {
                lock (_consoleLock)
                {
                    // Save cursor position
                    Console.WriteLine(); // Move to next line
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"  [DUP] {e.PackageName}.{e.ObjectName} found in {e.UpkFiles.Count} files");
                    Console.ResetColor();
                }
            }
        }

        private void OnIndexingCompleted(object sender, IndexingCompletedEventArgs e)
        {
            Console.WriteLine("\n\n=== Indexing Statistics ===");
            Console.WriteLine($"Duration:          {e.Duration.TotalMinutes:F2} minutes");
            Console.WriteLine($"Files indexed:     {e.IndexedFiles:N0}");
            Console.WriteLine($"Files skipped:     {e.SkippedFiles:N0}");
            Console.WriteLine($"Unique objects:    {e.UniqueObjects:N0}");
            Console.WriteLine($"Total locations:   {e.TotalLocations:N0}");
            Console.WriteLine($"Duplicate objects: {e.DuplicateObjects:N0}");
            Console.WriteLine($"Avg locations/obj: {(double)e.TotalLocations / e.UniqueObjects:F2}");
        }

        private void OnErrorOccurred(object sender, Exception ex)
        {
            lock (_consoleLock)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}