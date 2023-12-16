EULYNX Point Simulator
===============

.NET 5 and node.js 14 are required as dependencies (works on Linux and arm64/arm32, too):

- https://dotnet.microsoft.com/
- https://nodejs.org/en/

Then, execute `dotnet run` in this directory and open http://localhost:5101 in the browser.


## gRPC API

`PreventEndPosition(SimulatedPositionMessage)` can be used to put the point in a degraded or no end position at all.
The following are the allowed parameter values and resulting point states:

|SimulatedPositionMessage.position|SimulatedPositionMessage.degradedPosition|pointPosition|degradedPointPosition|
|--|--|--|--|
|UnintendedPosition| * | NoEndPosition | NotDegraded
|PreventedLeft|DegradedLeft| NoEndPosition | DegradedLeft
|PreventedRight|DegradedRight| NoEndPosition | DegradedRight
|PreventedLeft|NotDegraded| NoEndPosition | NotDegraded
|PreventedRight|NotDegraded| NoEndPosition | NotDegraded

## Arguments

The `PointSettings` command-line arguments are used to configure the point simulator.

| Argument | Description |
|---|---|
| `--PointSettings:LocalId` | The local point ID. |
| `--PointSettings:LocalRastaId` | The local Rasta ID. |
| `--PointSettings:RemoteId` | The remote point ID. |
| `--PointSettings:RemoteEndpoint` | The remote endpoint URL. |
| `--PointSettings:ConnectionProtocol` | The protocol to use  when connecting to an endpoint. |

For example, to set the local point ID to `W1`, the local Rasta ID to `96`, the remote point ID to `INTERLOCKING`, and the remote endpoint URL to `http://localhost:5100` using `EulynxBaseline4R1`, you would use the following command:
```
dotnet run --PointSettings:LocalId W1 --PointSettings:LocalRastaId 96 --PointSettings:RemoteId INTERLOCKING --PointSettings:RemoteEndpoint http://localhost:5100 --PointSettings:ConnectionProtocol EulynxBaseline4R1
```

### Available Connection Protocols:
- EulynxBaseline4R1
- EulynxBaseline4R2
