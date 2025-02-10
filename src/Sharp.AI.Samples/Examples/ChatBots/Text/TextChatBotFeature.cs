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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sharp.AI.Enums;
using Sharp.AI.Extensions;
using Sharp.AI.Interfaces.Clients;
using Sharp.AI.Models;
using Sharp.AI.Models.Configuration;
using Sharp.AI.Models.Options;
using Sharp.AI.Models.Requests;
using Sharp.AI.Samples.Interfaces;
using Sharp.AI.Services.Clients;

namespace Sharp.AI.Samples.Examples.ChatBots.Text;

public class TextChatBotFeature
    : IExample
{
    private readonly IHost? _app;

    private ITextChatClientService? _chatClientService;

    public TextChatBotFeature()
    {
        AiClientConfiguration clientConfiguration = new(ModelType.Ollama, "http://127.0.0.1:11434", "deepseek-r1:1.5b");
        
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddSharpAI(clientConfiguration);
        builder.Services.AddSingleton<ITextChatClientService, TextChatClientService>();
        
        _app = builder.Build();
    }

    public async Task Run()
    {
        _chatClientService = _app?.Services.GetService<ITextChatClientService>();
        _chatClientService!.SetPromptOptions(new PromptOptions(useMemory: true));
        _chatClientService!.SetSystemPrompt(
            """
                1. You are a helpful artificial intelligence assistant.
                2. If the user doesn't tell you their name, always reply with 'Hello, fellow human being'.
                    If the user tells you their name, always great them with the name that they gave you.
                3. If you don't know the answer to a question, just say 'I don't know'.
            """);

        Console.WriteLine("Welcome to the chat client example! ('q' to quit)\n");

        while (true)
        {
            Console.Write("User: ");
            var input = Console.ReadLine();
            if (input is null) continue;
            if (input.Equals("q", StringComparison.CurrentCultureIgnoreCase)) break;

            var response = await _chatClientService!.SendPromptRequestAsync(new PromptRequest(input));

            Console.WriteLine("Assistant: \n");
            if (response?.Reasoning is not null)
            {
                Console.WriteLine("[Reasoning]\n" + response.Reasoning.Replace("\n", "*") + "\n");
            }

            Console.WriteLine("[Response]\n" + response?.Response.Replace("\n", ""));
            Console.WriteLine("\n");

            var tokenUsage = _chatClientService!.GetTokenUsage();
            Console.WriteLine(
                $"TokenUsage: In: {tokenUsage.InputTokens}, Out: {tokenUsage.OutputTokens}, Total: {tokenUsage.TotalTokens}\n");
        }
    }
}