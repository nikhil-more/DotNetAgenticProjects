using System.ClientModel;
using OpenAI;

public class AgentProvider
{
    #region Constants

    private const string lightweightModel = "nex-agi/nex-n2.5-mini:free";
    private const string midTierModel = "nex-agi/nex-n2.5-pro:free";

    #endregion

    #region Properties
    private readonly string llmApiKey;
    private readonly string llmBaseUri;
    private readonly string searchApiKey;

    private readonly OpenAIClient openAIClient;
    
    #endregion

    public AgentProvider()
    {
        llmApiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY") ?? "";
        searchApiKey = Environment.GetEnvironmentVariable("TAVILY_API_KEY") ?? "";
        llmBaseUri = Environment.GetEnvironmentVariable("BASE_URL") ?? "";

        if (string.IsNullOrWhiteSpace(llmApiKey) || string.IsNullOrWhiteSpace(searchApiKey) || string.IsNullOrWhiteSpace(llmBaseUri))
        {
            throw new InvalidOperationException("LLM API Key and Search Api Key are mandatory. Please configure them in .devcontainer/.env");
        }

        openAIClient = InitializeOpenAIClient();
    }

    private OpenAIClient InitializeOpenAIClient()
    {
        if (string.IsNullOrWhiteSpace(llmApiKey) || string.IsNullOrWhiteSpace(llmBaseUri))
        {
            throw new InvalidOperationException("api key or base url are not configured.");
        }

        var clientOptions = new OpenAIClientOptions
        {
            Endpoint = new Uri(llmBaseUri)
        };

        var openAIClient = new OpenAIClient(new ApiKeyCredential(llmApiKey), clientOptions);

        Console.WriteLine("OpenAI Client Initialized Successfully");

        return openAIClient;
    }

    public async Task<AgentChatClient> GetAgentChatClient(string codeName)
    {

        var agentChatClient = await AgentChatClient.CreateAgentChatClientInstance(openAIClient, lightweightModel, codeName);

        Console.WriteLine($"Agent Chat Client Initialized Sucessfully. (CodeName : {codeName})");

        return agentChatClient;
    }
}
