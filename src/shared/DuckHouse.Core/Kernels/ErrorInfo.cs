namespace DuckHouse.Core.Kernels;

public record ErrorInfo(string Ename, string Evalue, IReadOnlyList<string> Traceback);
