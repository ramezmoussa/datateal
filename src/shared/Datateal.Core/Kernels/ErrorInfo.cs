namespace Datateal.Core.Kernels;

public record ErrorInfo(string Ename, string Evalue, IReadOnlyList<string> Traceback);
