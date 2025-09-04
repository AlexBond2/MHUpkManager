# UPK Index Generator

Console utility for generating an SQLite index database of Marvel Heroes Omega UPK files.

## Purpose

The tool scans UPK files and creates an index database that allows:

* Fast object lookup across multiple UPK files
* Tracking duplicates and dependencies

## Usage

### Generate Index from UPK Files

Run from console with the directory containing UPK files:

```bash
UpkIndexGenerator.exe "C:\MHO\UnrealEngine3\MarvelGame\CookedPCConsole"
```

Optionally specify an output database file:

```bash
UpkIndexGenerator.exe "C:\MHO\UnrealEngine3\MarvelGame\CookedPCConsole" "custom_index.db"
```

### Convert SQLite to MessagePack

Convert an existing SQLite index database to MessagePack format for faster in-memory operations:

```bash
UpkIndexGenerator.exe -convert [sqlite_db_path] [output_mpk_path]
```

Examples:

```bash
# Convert using default paths (mh152upk.db → mh152.mpk)
UpkIndexGenerator.exe -convert

# Convert specific SQLite database to default output
UpkIndexGenerator.exe -convert "my_index.db"

# Convert with custom input and output paths
UpkIndexGenerator.exe -convert "my_index.db" "my_index.mpk"
```

## Performance

* Typical run (about 15k UPK files) takes ~40 minutes on a modern desktop.
* Resulting SQLite database size is ~220 MB for a full scan.
* MessagePack format provides faster lookups with in-memory storage and reduces file size to ~13 MB.

## Requirements

* .NET 8.0 Runtime
* Marvel Heroes Omega UPK files