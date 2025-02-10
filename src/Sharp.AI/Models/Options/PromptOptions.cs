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

namespace Sharp.AI.Models.Options;

/// <summary>
/// Represents configuration options for customizing the behavior of a AI prompting system.
/// Defines properties and default values related to reasoning, summarization, truncation,
/// memory usage, and retrieval-augmented generation (RAG).
/// </summary>
public class PromptOptions(
    string? reasoningTag = PromptOptions.DefaultReasoningTag,
    int? summarizeMaxWordCount = PromptOptions.DefaultSummarizeMaxWordCount,
    int? truncationMaxPreviousPrompts = PromptOptions.DefaultTruncationMaxPreviousPrompts,
    bool? useMemory = PromptOptions.DefaultUseMemory,
    bool? useRAG = PromptOptions.DefaultUseRag,
    bool? useSummarization = PromptOptions.DefaultUseSummarization,
    bool? useTruncation = PromptOptions.DefaultUseTruncation)
{
    private const string DefaultReasoningTag = "think";
    private const int DefaultSummarizeMaxWordCount = 50;
    private const int DefaultTruncationMaxPreviousPrompts = 5;
    private const bool DefaultUseMemory = true;
    private const bool DefaultUseRag = false;
    private const bool DefaultUseSummarization = false;
    private const bool DefaultUseTruncation = false;

    public string ReasoningTag { get; init; } = reasoningTag ?? DefaultReasoningTag;
    
    public int SummarizeMaxWordCount { get; init; } = summarizeMaxWordCount ?? DefaultSummarizeMaxWordCount;
    
    public int TruncationMaxPreviousPrompts { get; init; } = truncationMaxPreviousPrompts ?? DefaultSummarizeMaxWordCount;

    public bool UseMemory { get; set; } = useMemory ?? DefaultUseMemory;
    
    public bool UseTruncation { get; set; } = useTruncation ?? DefaultUseTruncation;
    
    public bool UseSummarization { get; set; } = useSummarization ?? DefaultUseSummarization;
    
    public bool UseRAG { get; set; } = useRAG ?? DefaultUseRag;
}
