using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ChatBotWithAzureOpenAI.Models;
using Azure.AI.OpenAI;
using DotNetEnv;
using static System.Environment;
using System.Runtime.CompilerServices;
using Azure;

namespace ChatBotWithAzureOpenAI.Controllers;

public class ChatController : Controller
{
    private readonly ILogger<ChatController> _logger;
    public ChatController(ILogger<ChatController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GetResponse(string userMessage)
    {
        var endpoint = GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");   
        var key = GetEnvironmentVariable("AZURE_OPENAI_KEY");
        var aiModel = GetEnvironmentVariable("AZURE_OPENAI_MODEL"); 
        OpenAIClient client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));

        var chatCompletionMessage = new ChatCompletionsOptions(){
            Messages = {
                new ChatMessage(ChatRole.System, "Hello, how are you?"),
                new ChatMessage(ChatRole.User, "I'm doing great. How about you?"),
                new ChatMessage(ChatRole.Assistant, "I'm doing great. I'm glad to hear that."),
                new ChatMessage(ChatRole.User, userMessage),
            },
            MaxTokens = 150,

        };
        var response = await client.GetChatCompletionsAsync(deploymentOrModelName: aiModel, chatCompletionMessage);
        var chatResponse = response.Value.Choices.First().Message.Content;
        return Json(new { response = chatResponse});
    }
}
