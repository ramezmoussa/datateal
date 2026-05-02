namespace DuckHouse.Core.Kernels;

public record SemanticTokenResponse(IReadOnlyList<SemanticToken> Tokens);
