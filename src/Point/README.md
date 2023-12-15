EULYNX Point Simulator
===============

.NET 5 and node.js 14 are required as dependencies (works on Linux and arm64/arm32, too):

- https://dotnet.microsoft.com/
- https://nodejs.org/en/

Then, execute `dotnet run` in this directory and open http://localhost:5000 in the browser.


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
