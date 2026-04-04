namespace DuckHouse.Core.Kernels;

public record ExecuteRequest(string Code, double Timeout = 60.0);
