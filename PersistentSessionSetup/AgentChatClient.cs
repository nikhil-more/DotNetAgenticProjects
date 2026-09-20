using System.Runtime.CompilerServices;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;

public class AgentChatClient
{
    private readonly AIAgent _chatAgent;
    private AgentSession? _agentSession = null;
    private TokenTracker _tokenTracker;
    private readonly string _codeName;

    private AgentChatClient(OpenAIClient openAIClient, string modelName, string codeName)
    {
        this._codeName = codeName;
        this._chatAgent = openAIClient.GetChatClient(modelName).AsIChatClient().AsAIAgent(
            name: this._codeName,
            description: "Basic Chat Assistant",
            instructions: "Answer user queries with available information. Avoid verbose responses unless asked"
        );
        this._tokenTracker = new TokenTracker(this._codeName);
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
        _tokenTracker.UpdateUsageDetails(response.Usage);

        if (printTokenUsage && response.Usage != null)
        {
            _tokenTracker.LogUsageDetails();
        }

        return response.Text;
    }

    public async Task GetStreamingResponseAsync(string userQuery, Action<string> streamingHandler)
    {
        await foreach (var update in _chatAgent.RunStreamingAsync(userQuery, session: _agentSession))
        {
            if (!string.IsNullOrWhiteSpace(update.Text))
            {
                streamingHandler.Invoke(update.Text);
            }

            ExtractTokenUsageFromStreamingUpdate(update);
        }

        _tokenTracker.LogUsageDetails();
    }

    public async IAsyncEnumerable<string> GetStreamingResponseAsync(string userQuery, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (AgentResponseUpdate update in _chatAgent.RunStreamingAsync(userQuery, session: _agentSession, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrWhiteSpace(update.Text))
            {
                yield return update.Text;
            }

            ExtractTokenUsageFromStreamingUpdate(update);
        }

        _tokenTracker.LogUsageDetails();
    }

    private void ExtractTokenUsageFromStreamingUpdate(AgentResponseUpdate update)
    {
        if (update.Contents == null)    return;

        var usageContent = update.Contents.OfType<UsageContent>().FirstOrDefault();

        if (usageContent?.Details == null)   return;

        _tokenTracker.UpdateUsageDetails(usageContent.Details);
    }
}