# UPK Index Generator

Console utility for generating SQLite database index of Marvel Heroes Omega UPK files.

## Purpose

This tool scans all UPK files and creates an index database for:
- Fast object lookup across multiple UPK files
- Tracking duplicate objects (same object in multiple UPKs)
- Identifying primary/preferred object locations

## Usage

### Basic usage (with defaults):
```bash
UpkIndexGenerator.exe
```
Uses default path: `C:\Marvel Heroes Omega\Game\MarvelHeroesOmega\Content`

### Custom directory:
```bash
UpkIndexGenerator.exe "C:\MHO\UnrealEngine3\MarvelGame\CookedPCConsole"
```

### Custom directory and output database:
```bash
UpkIndexGenerator.exe "C:\MHO\UnrealEngine3\MarvelGame\CookedPCConsole" "custom_index.db"
```

## Output

The tool generates an SQLite database (`mh152upk.db` by default) containing:
- All unique objects found across UPK files
- Multiple locations for duplicated objects
- Metadata (file size, version, modification date)

## Requirements

- .NET 8.0 Runtime
- Marvel Heroes Omega UPK files (version 868, engine 10897)
- Sufficient disk space for the index database (~50-200 MB depending on content)

## Build from source

```bash
cd UpkIndexGenerator
dotnet build -c Release
dotnet publish -c Release -r win-x64 --self-contained false
```

## Integration

The generated database can be used with MHUpkManager for:
- Fast texture/object lookup
- Automatic dependency resolution
- Finding the best version of duplicated resources

## Performance

- Uses parallel processing (all CPU cores)
- Typical indexing time: 5-15 minutes for full game
- ~10,000-50,000 unique objects expected