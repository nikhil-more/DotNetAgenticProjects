
using Microsoft.Extensions.AI;
using OpenAI;

public class AgentChatClient
{
    private readonly IChatClient chatClient;
    private readonly string codeName;

    public AgentChatClient(OpenAIClient openAIClient, string modelName, string codeName)
    {
        chatClient = openAIClient.GetChatClient(modelName).AsIChatClient();
        this.codeName = codeName;
    }

    public async Task<string> GetResponseAsync(string userQuery)
    {
        var response = await chatClient.GetResponseAsync(userQuery);

        return response.Text;
    }
}