namespace Datateal.Core.Kernels;

/// <param name="Kind">Jedi completion type: function, module, class, instance, keyword, property, etc.</param>
/// <param name="InsertText">Full name to insert when the completion is accepted.</param>
public record CompletionItem(
    string Label,
    string Kind,
    string? Detail,
    string? Documentation,
    string InsertText);
