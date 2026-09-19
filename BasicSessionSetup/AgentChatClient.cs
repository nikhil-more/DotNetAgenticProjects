using System.Runtime.CompilerServices;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;

public class AgentChatClient
{
    private readonly AIAgent _chatAgent;
    private AgentSession? _agentSession = null;
    private readonly string _codeName;

    private AgentChatClient(OpenAIClient openAIClient, string modelName, string codeName)
    {
        this._chatAgent = openAIClient.GetChatClient(modelName).AsIChatClient().AsAIAgent(
            name: codeName,
            description: "Basic Chat Assistant",
            instructions: "Answer user queries with available information. Avoid verbose responses unless asked"
        );
        this._codeName = codeName;
    }

    public static async Task<AgentChatClient> CreateAgentChatClientInstance(OpenAIClient openAIClient, string modelName, string codeName)
    {
        var agentClient = new AgentChatClient(openAIClient, modelName, codeName);
        agentClient._agentSession = await agentClient._chatAgent.CreateSessionAsync();

        return agentClient;
    }

    private async Task InitializeAgentSession(bool forceNew = false)
    {
        if (forceNew || _agentSession == null)
        {
            _agentSession = await _chatAgent.CreateSessionAsync();
        }
    }

    public async Task<string> GetResponseAsync(string userQuery, bool printTokenUsage = false)
    {
        var response = await _chatAgent.RunAsync(userQuery, session: _agentSession);

        if (printTokenUsage && response.Usage != null)
        {
            Console.WriteLine($"Usage Details : \n Input Token Count : {response.Usage.InputTokenCount} \n Output Token Count : {response.Usage.OutputTokenCount} \n Reasoning Token Count : {response.Usage.ReasoningTokenCount}");
        }

        return response.Text;
    }

    public async Task GetStreamingResponseAsync(string userQuery, Action<string> streamingHandler)
    {
        await foreach (var update in _chatAgent.RunStreamingAsync(userQuery, session: _agentSession))
        {
            streamingHandler.Invoke(update.Text);
        }
    }

    public async IAsyncEnumerable<string> GetStreamingResponseAsync(string userQuery, [EnumeratorCancellation]CancellationToken cancellationToken = default)
    {
        await foreach(var update in _chatAgent.RunStreamingAsync(userQuery, session: _agentSession, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrWhiteSpace(update.Text))
            {
                yield return update.Text;
            }
        }
    }

}