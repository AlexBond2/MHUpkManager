# UPK Index Generator

Console utility for generating an SQLite index database of Marvel Heroes Omega UPK files.

## Purpose

The tool scans UPK files and creates an index database that allows:

* Fast object lookup across multiple UPK files
* Tracking duplicates and dependencies

## Usage

Run from console with the directory containing UPK files:

```bash
UpkIndexGenerator.exe "C:\MHO\UnrealEngine3\MarvelGame\CookedPCConsole"
```

Optionally specify an output database file:

```bash
UpkIndexGenerator.exe "C:\MHO\UnrealEngine3\MarvelGame\CookedPCConsole" "custom_index.db"
```

## Performance

* Typical run (about 15k UPK files) takes \~20 minutes on a modern desktop.
* Resulting database size is \~50 MB for a full scan.

## Requirements

* .NET 8.0 Runtime
* Marvel Heroes Omega UPK files
