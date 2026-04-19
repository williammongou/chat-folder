namespace TextLivingDemo.Models;

/// <summary>
/// Represents a single message in a conversation thread
/// </summary>
public class Message
{
    public string Sender { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
