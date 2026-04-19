using Microsoft.AspNetCore.Mvc;
using TextLivingDemo.Models;
using TextLivingDemo.Services;

namespace TextLivingDemo.Controllers;

/// <summary>
/// API controller for conversation analysis
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ConversationController : ControllerBase
{
    private readonly ConversationAnalysisService _analysisService;
    private readonly ILogger<ConversationController> _logger;

    public ConversationController(
        ConversationAnalysisService analysisService,
        ILogger<ConversationController> logger)
    {
        _analysisService = analysisService;
        _logger = logger;
    }

    /// <summary>
    /// Analyzes an apartment leasing conversation thread
    /// </summary>
    /// <param name="request">The conversation thread to analyze</param>
    /// <returns>AI-powered analysis with conversion probability and recommendations</returns>
    [HttpPost("analyze")]
    [ProducesResponseType(typeof(AnalysisResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AnalysisResponse>> AnalyzeConversation([FromBody] ConversationRequest request)
    {
        try
        {
            // Validate request
            if (request.Messages == null || request.Messages.Count == 0)
            {
                return BadRequest(new { error = "No messages provided" });
            }

            if (request.Messages.Count < 2)
            {
                return BadRequest(new { error = "At least 2 messages are required for analysis" });
            }

            _logger.LogInformation("Received analysis request with {Count} messages", request.Messages.Count);

            // Perform analysis
            var result = await _analysisService.AnalyzeConversationAsync(request.Messages);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing conversation analysis");
            return StatusCode(500, new
            {
                error = "An error occurred while analyzing the conversation",
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}
