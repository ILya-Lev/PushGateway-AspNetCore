Prometheus PushGateway implementation with AspNetCore.
At the moment with limited functionality - only Gauges and Histograms of Int32 and Double deltas are supported.
Use case - your short living app pushes metrics into PushGateway; Prometheus reads data from this API.
See swagger for the available endpoints.
Prometheus setup as well as PushGateway deployment is on you, dear friend :)
