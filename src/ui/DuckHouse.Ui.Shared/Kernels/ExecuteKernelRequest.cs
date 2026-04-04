namespace DuckHouse.Ui.Shared.Kernels;

public record ExecuteKernelRequest(string Code, double Timeout = 60.0);
