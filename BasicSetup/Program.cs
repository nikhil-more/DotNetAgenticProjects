using System.ClientModel;
using System.Data;
using Microsoft.Extensions.AI;
using OpenAI;

var llmKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY") ?? "";
var searchApiKey = Environment.GetEnvironmentVariable("TAVILY_API_KEY") ?? "";
var llmBaseUri = Environment.GetEnvironmentVariable("BASE_URL") ?? "";

if (string.IsNullOrWhiteSpace(llmKey) || string.IsNullOrWhiteSpace(searchApiKey) || string.IsNullOrWhiteSpace(llmBaseUri))
{
    throw new InvalidOperationException("LLM API Key and Search Api Key are mandatory. Please configure them in .devcontainer/.env");
}

var clientOptions = new OpenAIClientOptions
{
    Endpoint = new Uri(llmBaseUri)
};

var openAiClient = new OpenAIClient(new ApiKeyCredential(llmKey), clientOptions);

// var modelId = "nvidia/nemotron-3.5-lightning:free";
var openAIModel = "nex-agi/nex-n2.5-mini:free";

IChatClient chatClient = openAiClient
                            .GetChatClient(openAIModel)
                            .AsIChatClient();

Console.WriteLine("Chat Client initialized Successfully");
Console.WriteLine("You can start asking your queryies");

while (true)
{
    Console.Write("User :");
    var userQuery = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userQuery) || userQuery.Equals("quit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    var response = await chatClient.GetResponseAsync(userQuery);

    Console.WriteLine($"Assistant : {response.Text}");

    Console.WriteLine(response);
}