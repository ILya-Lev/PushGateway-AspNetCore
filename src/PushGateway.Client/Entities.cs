using System.Numerics;

namespace PushGateway.Client;

public record GaugeDescriptor<T>(string Name, string Description, string Unit, T Delta) 
    where T : struct, INumber<T>;

public record HistogramDescriptor<T>(string Name, string Description, string Unit, T Delta, T[] Boundaries)
    : GaugeDescriptor<T>(Name, Description, Unit, Delta)
    where T : struct, INumber<T>;

public record GaugeDescriptorInt32(string Name, string Description, string Unit, Int32 Delta)
    : GaugeDescriptor<Int32>(Name, Description, Unit, Delta);

public record GaugeDescriptorDouble(string Name, string Description, string Unit, double Delta)
    : GaugeDescriptor<double>(Name, Description, Unit, Delta);

public record HistogramDescriptorInt32(string Name, string Description, string Unit, Int32 Delta, Int32[] Boundaries)
    : HistogramDescriptor<Int32>(Name, Description, Unit, Delta, Boundaries);

public record HistogramDescriptorDouble(string Name, string Description, string Unit, double Delta, double[] Boundaries)
    : HistogramDescriptor<double>(Name, Description, Unit, Delta, Boundaries);