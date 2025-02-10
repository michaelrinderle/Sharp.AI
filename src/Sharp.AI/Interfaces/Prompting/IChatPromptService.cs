using Sharp.AI.Models.Requests;
using Sharp.AI.Models.Responses;

namespace Sharp.AI.Interfaces.Prompting;

/// <summary>
/// Interface representing a service dedicated to processing natural language prompts within a chat context.
/// </summary>
/// <remarks>
/// This interface extends <see cref="IPromptService"/> and includes specific capabilities for handling
/// chat-based prompt processing, such as sending requests and receiving structured responses.
/// </remarks>
public interface IChatPromptService
    : IPromptService
{
    /// <summary>
    /// Processes a given prompt request asynchronously and returns the corresponding structured response.
    /// </summary>
    /// <param name="request">The prompt request containing the input prompt and additional metadata.</param>
    /// <returns>
    /// A <see cref="PromptResponse"/> representing the result of the prompt processing, or null if the request could not be processed.
    /// </returns>
    Task<PromptResponse?> ProcessChatPromptAsync(PromptRequest request);
}