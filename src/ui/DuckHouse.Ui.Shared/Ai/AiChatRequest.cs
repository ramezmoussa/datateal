namespace DuckHouse.Ui.Shared.Ai;

/// <summary>Context type that determines what content the AI is operating on.</summary>
public enum AiContextType
{
    /// <summary>Working on an individual notebook cell; full notebook content is included for context.</summary>
    NotebookCell,
    /// <summary>Working on the notebook as a whole.</summary>
    Notebook,
    /// <summary>Working on a SQL query file.</summary>
    Query,
}

/// <summary>A single message in the chat history.</summary>
public record AiChatMessage(string Role, string Content);

/// <summary>
/// Request to start an AI chat completion stream.
/// Sent from the WASM client to the server SignalR hub.
/// </summary>
public record AiChatRequest(
    AiProviderType Provider,
    /// <summary>API key for the selected provider. Never persisted server-side.</summary>
    string ApiKey,
    /// <summary>Provider endpoint URL (e.g. Azure OpenAI resource endpoint).</summary>
    string Endpoint,
    /// <summary>Model / deployment name (e.g. "gpt-4o").</summary>
    string Model,
    AiContextType ContextType,
    /// <summary>The conversation history so far (user + assistant turns).</summary>
    IReadOnlyList<AiChatMessage> Messages,
    /// <summary>
    /// Full notebook content as JSON (ipynb format), if context involves a notebook.
    /// Null for Query context.
    /// </summary>
    string? NotebookJson,
    /// <summary>Index of the cell the user is focused on (-1 for notebook-level context).</summary>
    int FocusedCellIndex,
    /// <summary>
    /// Current SQL query content, for Query context.
    /// Null for Notebook/NotebookCell context.
    /// </summary>
    string? QueryContent,
    /// <summary>IDs of catalogs attached to the current workspace item.</summary>
    IReadOnlyList<Guid> CatalogIds
);
