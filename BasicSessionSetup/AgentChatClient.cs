using System.Runtime.CompilerServices;
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

    public async Task<string> GetResponseAsync(string userQuery, bool printTokenUsage = false)
    {
        var response = await chatClient.GetResponseAsync(userQuery);

        if (printTokenUsage && response.Usage != null)
        {
            Console.WriteLine($"Usage Details : \n Input Token Count : {response.Usage.InputTokenCount} \n Output Token Count : {response.Usage.OutputTokenCount} \n Reasoning Token Count : {response.Usage.ReasoningTokenCount}");
        }

        return response.Text;
    }

    public async Task GetStreamingResponseAsync(string userQuery, Action<string> streamingHandler)
    {
        await foreach (var update in chatClient.GetStreamingResponseAsync(userQuery))
        {
            streamingHandler.Invoke(update.Text);
        }
    }

    public async IAsyncEnumerable<string> GetStreamingResponseAsync(string userQuery, [EnumeratorCancellation]CancellationToken cancellationToken = default)
    {
        await foreach(var update in chatClient.GetStreamingResponseAsync(userQuery, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrWhiteSpace(update.Text))
            {
                yield return update.Text;
            }
        }
    }
}

//Great. So lets say person A is standing behind person B. Person C is standing ahead of Person A. There are 3 people between Person C and Person E. There are in total 5 people - A, B, C, D, E. So tell me where is D standing and how many people are there in between B and D. (By ahead it means directly in front of it, with no one in between)