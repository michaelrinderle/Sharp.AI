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

using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.AI;
using Sharp.AI.Interfaces.Prompting;
using Sharp.AI.Models.Options;
using Sharp.AI.Models.Prompting;
using Sharp.AI.Models.Requests;
using Sharp.AI.Models.Responses;

namespace Sharp.AI.Abstractions;

/// <summary>
/// Represents an abstract base class for implementing a chat service that handles processing of prompt requests,
/// manages prompt history, token usage, and system prompts, and provides utilities for prompt generation and response parsing.
/// </summary>
public abstract class AbstractChatService(IChatClient chatClient)
    : IChatPromptService
{
    private const string UserPrefix = "User: ";
    private const string AssistantPrefix = "Assistant: ";
    private const string HistoryHeader = "[History] \n";
    private const string InstructionsHeader = "[Instructions] \n";

    private readonly List<PromptHistoryItem> _promptHistory = new();
    private readonly TokenUsage _tokenUsage = new();
    private PromptOptions _promptOptions = new();
    private string? _systemPrompt;
    
    protected readonly IChatClient ChatClient = chatClient;
    
    public List<PromptHistoryItem> GetPromptHistory() => _promptHistory;
    
    public TokenUsage GetTokenUsage() => _tokenUsage;

    public void ImportPromptHistory(List<PromptHistoryItem> history) => _promptHistory.AddRange(history);
    
    public abstract Task<PromptResponse?> ProcessChatPromptAsync(PromptRequest request);

    public void SetPromptOptions(PromptOptions options) => _promptOptions = options;
    
    public void SetSystemPrompt(string instructions) => _systemPrompt = instructions;

    protected async Task AddPromptHistoryItem(string prompt, string response)
    {
        if (_promptOptions.UseSummarization)
        {
            prompt = await SummarizeIfNeeded(prompt, _promptOptions.SummarizeMaxWordCount);
            response = await SummarizeIfNeeded(response, _promptOptions.SummarizeMaxWordCount);
        }
        
        _promptHistory.Add(new PromptHistoryItem(prompt, response));
        
        await Task.CompletedTask;
    }

    protected string GenerateInternalPrompt(string prompt)
    {
        StringBuilder sb = new();
        
        if (_systemPrompt is not null)
            sb.Append($"{InstructionsHeader} {_systemPrompt}\n");
        
        if (_promptOptions.UseMemory)
            sb.Append(_promptOptions is { UseTruncation: false, UseSummarization: false, UseRAG: false }
                ? GenerateHistory()
                : GenerateHistoryWithOptions());
        
        sb.Append($"{UserPrefix}{prompt}");
        
        return sb.ToString();
    }

    protected PromptResponse ParsePromptResponse(string prompt, ChatCompletion completion)
    {
        var thinkPattern = $"<{_promptOptions.ReasoningTag}>(.*?)</{_promptOptions.ReasoningTag}>";
        var response = completion.Message.Text!;
        string? reasoning = null;
        
        Match match = Regex.Match(response, thinkPattern, RegexOptions.Singleline);
        
        if (match.Success)
        {
            reasoning = match.Groups[1].Value.Trim();
            response = Regex.Replace(response, thinkPattern, string.Empty, RegexOptions.Singleline).Trim();
        }
        
        return new PromptResponse
        {
            Prompt = prompt,
            Reasoning = reasoning,
            Response = response,
        };
    }

    protected void UpdateTokenUsage(UsageDetails? usageDetails)
    {
        if (usageDetails is null) return;
        
        _tokenUsage.InputTokens += usageDetails.InputTokenCount ?? 0;
        _tokenUsage.OutputTokens += usageDetails.OutputTokenCount ?? 0;
        _tokenUsage.TotalTokens += usageDetails.TotalTokenCount ?? 0;
    }
    
    private void AppendHistoryItem(StringBuilder sb, PromptHistoryItem item)
    {
        sb.Append($"{UserPrefix}{item.Prompt}\n");
        sb.Append($"{AssistantPrefix}{item.Response}\n");
    }
    
    private string GenerateHistory()
    {
        if (_promptHistory.Count == 0) return string.Empty;
        
        StringBuilder sb = new();
        sb.Append(HistoryHeader);
        
        foreach (var item in _promptHistory)
        {
            AppendHistoryItem(sb, item);
        }
        
        return sb.ToString();
    }

    private string GenerateHistoryWithOptions()
    {
        if (_promptHistory.Count == 0) return string.Empty;
        
        var historySubset = _promptOptions.UseTruncation
            ? _promptHistory.TakeLast(_promptOptions.TruncationMaxPreviousPrompts)
            : _promptHistory;
        
        StringBuilder sb = new();
        sb.Append(HistoryHeader);
        
        foreach (var item in historySubset)
        {
            AppendHistoryItem(sb, item);
        }
        
        return sb.ToString();
    }

    private async Task<string> SummarizeIfNeeded(string text, int maxWordCount)
    {
        if (text.Split(' ').Length <= maxWordCount) return text;
        
        var summaryPrompt = $"Summarize this to {maxWordCount} words: {text}";
        var completion = await ChatClient.CompleteAsync(summaryPrompt);
        
        UpdateTokenUsage(completion.Usage);
        
        var parsedSummary = ParsePromptResponse(text, completion);
        var summarizedWords = parsedSummary.Response.Split(' ').Take(maxWordCount);
        
        return string.Join(" ", summarizedWords);
    }
}
