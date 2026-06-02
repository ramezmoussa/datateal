namespace Datateal.Core.Kernels;

/// <param name="Context">Code from all prior cells, joined by newlines. Gives Jedi visibility into variables/imports defined earlier in the session.</param>
public record SemanticTokenRequest(string Code, string Context = "");
