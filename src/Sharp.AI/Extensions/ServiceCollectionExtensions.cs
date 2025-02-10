/*
    MIT License

    michael rinderle 2025
    written by michael rinderle <michael@sofdigital.net>

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

*/

using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using Sharp.AI.Enums;
using Sharp.AI.Models;
using Sharp.AI.Models.Configuration;

namespace Sharp.AI.Extensions;

/// <summary>
/// Provides extension methods for configuring AI services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures AI services to the specified <see cref="IServiceCollection"/> using the provided AI client.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the AI services to.</param>
    /// <param name="aiClientConfiguration">The <see cref="AiClientConfiguration"/> containing configuration details for the AI model, endpoint, and API key.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with the configured AI services.</returns>
    /// <exception cref="ArgumentException">Thrown when the specified AI client model type is not supported.</exception>
    public static IServiceCollection AddSharpAI(this IServiceCollection services, AiClientConfiguration aiClientConfiguration)
    {
        var chatClient = CreateChatClient(aiClientConfiguration);
        services.AddChatClient(chatClient);

        return services;
    }

    /// <summary>
    /// Creates and configures an implementation of <see cref="IChatClient"/> based on the specified AI client settings.
    /// </summary>
    /// <param name="aiClientConfiguration">The <see cref="AiClientConfiguration"/> containing configuration details including model type, endpoint, and API key.</param>
    /// <returns>An instance of <see cref="IChatClient"/> configured according to the provided <see cref="AiClientConfiguration"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when an unsupported model type is specified in the <see cref="AiClientConfiguration"/>.</exception>
    private static IChatClient CreateChatClient(AiClientConfiguration aiClientConfiguration) => aiClientConfiguration.ModelType switch
    {
        ModelType.OpenAI => new OpenAIClient(aiClientConfiguration.EndpointOrApiKey)
            .AsChatClient(modelId: aiClientConfiguration.ModelId),
        ModelType.AzureOpenAI => new AzureOpenAIClient(
                new Uri(aiClientConfiguration.EndpointOrApiKey),
                new DefaultAzureCredential())
            .AsChatClient(modelId: aiClientConfiguration.ModelId),
        ModelType.Ollama => new OllamaChatClient(aiClientConfiguration.EndpointOrApiKey, aiClientConfiguration.ModelId),
        _ => throw new ArgumentException("Incorrect AI client model type", nameof(aiClientConfiguration.ModelType))
    };
}