namespace DuckHouse.Core.Kernels;

/// <param name="Line">1-based line number.</param>
/// <param name="StartChar">0-based column number.</param>
/// <param name="Length">Length of the token in characters.</param>
/// <param name="TokenType">Semantic token type: function, class, parameter, variable, builtin, selfParameter, property, decorator, namespace.</param>
public record SemanticToken(int Line, int StartChar, int Length, string TokenType);
