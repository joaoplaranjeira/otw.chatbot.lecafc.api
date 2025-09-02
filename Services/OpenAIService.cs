using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace otw.chatbot.lecafc.api.Services;

public class OpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public OpenAIService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["OpenAI:ApiKey"]!;
        _model = config["OpenAI:Model"] ?? "gpt-3.5-turbo";
    }

    public async Task<string> InterpretMessageAsync(string userMessage, string context = "")
    {
        const int maxRetries = 3;
        int delayMs = 2000;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            var result = await SendRequestToOpenAIAsync(userMessage, context);

            if (result != "429")
                return result;

            Console.WriteLine($"⚠️ OpenAI rate limit hit. Retrying attempt {attempt}/{maxRetries} in {delayMs / 1000}s...");
            await Task.Delay(delayMs);
            delayMs *= 2; // exponential backoff
        }

        return "⚠️ Too many requests to OpenAI. Please try again later.";
    }

    private async Task<string> SendRequestToOpenAIAsync(string userMessage, string context)
    {
        var requestMessage = $"{context}\n\nQuestion: {userMessage}";

        var payload = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = "You are the virtual assistant of Leça Futebol Clube. Answer questions based only on the provided context." },
                new { role = "user", content = requestMessage }
            },
            temperature = 0.3
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        var response = await _httpClient.SendAsync(request);

        if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            return "429";

        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(responseJson);

        var responseMessage = document.RootElement
                       .GetProperty("choices")[0]
                       .GetProperty("message")
                       .GetProperty("content")
                       .GetString()
               ?? "I'm sorry, I couldn't understand the question.";

        Console.WriteLine($"🤖 OpenAI request: {requestMessage}");
        Console.WriteLine($"🤖 OpenAI response: {responseMessage}");

        return responseMessage;
    }
}