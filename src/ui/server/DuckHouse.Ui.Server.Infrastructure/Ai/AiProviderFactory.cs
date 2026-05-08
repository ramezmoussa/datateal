using Azure;
using Azure.AI.OpenAI;
using DuckHouse.Ui.Server.Application.Ai;
using DuckHouse.Ui.Shared.Ai;
using Microsoft.Extensions.AI;

namespace DuckHouse.Ui.Server.Infrastructure.Ai;

/// <summary>
/// Creates <see cref="IChatClient"/> instances for each supported AI provider.
/// Clients are short-lived and created per-request; credentials are never cached.
/// </summary>
public class AiProviderFactory : IAiProviderFactory
{
    public IChatClient Create(AiProviderType provider, string apiKey, string endpoint, string model)
    {
        return provider switch
        {
            AiProviderType.AzureOpenAI => CreateAzureOpenAIClient(apiKey, endpoint, model),
            _ => throw new ArgumentException($"Unknown AI provider: {provider}", nameof(provider)),
        };
    }

    private static IChatClient CreateAzureOpenAIClient(string apiKey, string endpoint, string model)
    {
        var client = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
        // GetChatClient returns OpenAI.Chat.ChatClient; AsIChatClient() is the MEA.OpenAI extension
        return client.GetChatClient(model).AsIChatClient();
    }
}
