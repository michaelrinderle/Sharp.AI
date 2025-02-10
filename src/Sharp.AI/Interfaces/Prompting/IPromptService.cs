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

using Sharp.AI.Models.Options;
using Sharp.AI.Models.Prompting;

namespace Sharp.AI.Interfaces.Prompting;

/// <summary>
/// Interface defining a service for managing and interacting with AI prompts, including
/// tracking token usage, handling prompt history, configuring prompt options, and setting system-level prompts.
/// </summary>
public interface IPromptService
{
    /// <summary>
    /// Retrieves the current token usage details, including counts of input tokens, output tokens, and total tokens.
    /// </summary>
    /// <returns>
    /// A <see cref="TokenUsage"/> object containing information about the number of input tokens, output tokens, and
    /// total tokens used.
    /// </returns>
    TokenUsage GetTokenUsage();

    /// <summary>
    /// Retrieves the current history of prompts and their corresponding responses.
    /// </summary>
    /// <returns>
    /// A list of <see cref="PromptHistoryItem"/> objects representing each prompt and its associated response from the history.
    /// </returns>
    List<PromptHistoryItem> GetPromptHistory();

    /// <summary>
    /// Imports a list of prompt history items into the current prompt history collection.
    /// </summary>
    /// <param name="history">The list of <see cref="PromptHistoryItem"/> objects to be added to the prompt history.</param>
    void ImportPromptHistory(List<PromptHistoryItem> history);

    /// <summary>
    /// Configures the prompt options used by the AI service, allowing customization of behavior such as memory usage,
    /// summarization, truncation, and more.
    /// </summary>
    /// <param name="options">An instance of <see cref="PromptOptions"/> that defines the configuration for the prompt
    /// service, including properties for reasoning tags, summarization limits, truncation settings, and memory usage.</param>
    void SetPromptOptions(PromptOptions options);

    /// <summary>
    /// Sets the system-level prompt that provides baseline instructions or context for the AI's responses.
    /// </summary>
    /// <param name="instructions">The instructional text representing the system prompt to define AI behavior contextually.</param>
    void SetSystemPrompt(string instructions);
}