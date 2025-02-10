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

namespace Sharp.AI.Models.Requests;

/// <summary>
/// Represents a request containing a prompt for processing, along with associated metadata.
/// </summary>
/// <remarks>
/// This class is typically used to encapsulate input data for AI-driven models or services
/// that process user prompts and generate responses.
/// </remarks>
/// <param name="prompt">
/// The input prompt string that contains the data or query to be processed.
/// </param>
/// <property name="RequestTimestampUTC">
/// The timestamp indicating when the prompt request was created, using UTC time.
/// </property>
public class PromptRequest(string prompt)
{
    public string Prompt { get; set; } = prompt;

    public DateTime RequestTimestampUtc = DateTime.UtcNow;
}