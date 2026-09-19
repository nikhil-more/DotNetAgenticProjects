AgentProvider agentProvider = new AgentProvider();
AgentChatClient agentChat = agentProvider.GetAgentChatClient("BasicSetup");

Console.WriteLine("You can start asking your queryies");

while (true)
{
    Console.Write("User : ");
    var userQuery = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userQuery) || userQuery.Equals("quit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    // var response = await agentChat.GetResponseAsync(userQuery, printTokenUsage: true);

    // Console.WriteLine($"Assistant : {response}");

    await agentChat.GetStreamingResponseAsync(userQuery, (string chunk) => Console.Write(chunk));
    Console.WriteLine();
}