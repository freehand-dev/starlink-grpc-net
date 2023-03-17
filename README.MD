# starlink-grpc-net
> Tools for working with the Starlink user terminal via the gRPC protocol.

## Tools
- starlink-proto-dump
- dish-status-watcher
- dish-get-status
- dish-get-config
- dish-control

## Quick start
First, dump proto files from Starlink terminal.
```bash
dotnet starlink-proto-dump.dll ../starlink-grpc-protos/
```

Build starlink-grpc-protos library
```bash
cd ./starlink-grpc-protos
dotnet build
```

Run tools
```bash
dotnet dish-status-watcher.dll
```

## starlink-proto-dump
```bash
dotnet starlink-proto-dump.dll
dotnet starlink-proto-dump.dll ./path_to_save
```

## dish-status-watcher
```bash
dotnet dish-status-watcher.dll
dotnet dish-status-watcher.dll --interval 5000 --fields DeviceState.UptimeS,PopPingDropRate --log log.txt
dotnet dish-status-watcher.dll -i 5000 -f DeviceState.UptimeS,PopPingDropRate -l log.txt
```

- `--interval` - refresh interval. (default is 1000)
- `--fields` - show fields. (default is "DeviceState.UptimeS, PopPingDropRate, PopPingLatencyMs, SecondsToFirstNonemptySlot, UplinkThroughputBps, DownlinkThroughputBps, GpsStats.GpsValid, GpsStats.NoSatsAfterTtff, GpsStats.GpsSats, ObstructionStats.CurrentlyObstructed, ObstructionStats.TimeObstructed, ObstructionStats.ValidS")
- `--log` - save data to file. (default is none)


## dish-get-status
```bash
dotnet dish-get-status.dll
```

## dish-get-config
```bash
dotnet dish-get-config.dll
```

## dish-control
```bash
dotnet dish-control.dll reboot
dotnet dish-control.dll stow
dotnet dish-control.dll unstow
```
