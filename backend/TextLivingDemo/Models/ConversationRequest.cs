namespace TextLivingDemo.Models;

/// <summary>
/// Request payload containing the conversation thread to analyze
/// </summary>
public class ConversationRequest
{
    public List<Message> Messages { get; set; } = new();
}
