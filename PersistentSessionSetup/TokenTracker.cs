using Microsoft.Extensions.AI;

public class TokenTracker
{
    #region Properties
    private readonly string _agentName;

    private long _inputTokenCount = 0;
    private long _outputTokenCount = 0;
    private long _reasoningTokenCount = 0;
    private long _totalTokenCount = 0;

    #endregion

    public TokenTracker(string agentName)
    {
        _agentName = agentName;
    }

    public void UpdateUsageDetails(UsageDetails? usageDetails)
    {
        _inputTokenCount += usageDetails?.InputTokenCount ?? 0;
        _outputTokenCount += usageDetails?.OutputTokenCount ?? 0;
        _reasoningTokenCount += usageDetails?.ReasoningTokenCount ?? 0;
        _totalTokenCount += usageDetails?.TotalTokenCount ?? 0;
    }

    public void ResetUsageDetails()
    {
        _inputTokenCount = 0;
        _outputTokenCount = 0;
        _reasoningTokenCount = 0;
        _totalTokenCount = 0;
    }

    public void LogUsageDetails()
    {
        Console.WriteLine("\n=================================================");
        Console.WriteLine($"Current Token Usage By '{_agentName}' : ");
        Console.WriteLine($"Input Token Count : {_inputTokenCount}");
        Console.WriteLine($"Output Token Count : {_outputTokenCount}");
        Console.WriteLine($"Reasoning Token Count : {_reasoningTokenCount}");
        Console.WriteLine($"Total Token Count : {_totalTokenCount}");
        Console.WriteLine("=================================================");
    }
}