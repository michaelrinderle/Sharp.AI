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

using System.Diagnostics;
using Microsoft.Extensions.AI;
using Sharp.AI.Abstractions;
using Sharp.AI.Interfaces.Clients;
using Sharp.AI.Models.Prompting;
using Sharp.AI.Models.Requests;
using Sharp.AI.Models.Responses;

namespace Sharp.AI.Services.Clients;

public class TextChatClientService(IChatClient chatClient) 
    : AbstractChatService(chatClient), ITextChatClientService
{
    public override async Task<PromptResponse?> ProcessChatPromptAsync(PromptRequest request)
    {
        try
        {
            var internalPrompt = GenerateInternalPrompt(request.Prompt);
            var completion = await ChatClient.CompleteAsync(internalPrompt);

            UpdateTokenUsage(completion.Usage);

            var response = ParsePromptResponse(request.Prompt, completion);

            response.RequestTimestampUtc = request.RequestTimestampUtc;
            response.ResponseTimestampUtc = DateTime.UtcNow;

            response.TokenUsage = new TokenUsage
            {
                InputTokens = completion.Usage?.InputTokenCount ?? 0,
                OutputTokens = completion.Usage?.OutputTokenCount ?? 0,
                TotalTokens = completion.Usage?.TotalTokenCount ?? 0,
            };

            await AddPromptHistoryItem(response.Prompt, response.Response);

            return response;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.StackTrace);
        }

        return null;
    }

    public async Task<PromptResponse?> SendPromptRequestAsync(PromptRequest request)
    {
       return await ProcessChatPromptAsync(request);
    }
}