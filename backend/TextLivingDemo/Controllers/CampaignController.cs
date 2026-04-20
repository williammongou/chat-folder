using Microsoft.AspNetCore.Mvc;
using TextLivingDemo.Models;
using TextLivingDemo.Services;

namespace TextLivingDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController : ControllerBase
{
    private readonly CampaignGenerationService _campaignService;
    private readonly ILogger<CampaignController> _logger;

    public CampaignController(CampaignGenerationService campaignService, ILogger<CampaignController> logger)
    {
        _campaignService = campaignService;
        _logger = logger;
    }

    /// <summary>
    /// Generate promotional SMS campaign options
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<CampaignGenerationResponse>> GenerateCampaign([FromBody] CampaignGenerationRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest(new { error = "Prompt is required" });
            }

            _logger.LogInformation("Generating campaign for prompt: {Prompt}", request.Prompt);

            var response = await _campaignService.GenerateCampaignAsync(request);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GenerateCampaign endpoint");
            return StatusCode(500, new { error = "Failed to generate campaign", details = ex.Message });
        }
    }

    /// <summary>
    /// Get current token usage for the day
    /// </summary>
    [HttpGet("token-usage")]
    public ActionResult<TokenUsageResponse> GetTokenUsage()
    {
        try
        {
            var usage = _campaignService.GetTokenUsage();
            return Ok(usage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting token usage");
            return StatusCode(500, new { error = "Failed to get token usage" });
        }
    }

    /// <summary>
    /// Reset token usage counter (admin use)
    /// </summary>
    [HttpPost("reset-tokens")]
    public ActionResult ResetTokenUsage()
    {
        try
        {
            _campaignService.ResetTokenUsage();
            return Ok(new { message = "Token usage reset successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting token usage");
            return StatusCode(500, new { error = "Failed to reset token usage" });
        }
    }

    /// <summary>
    /// Health check
    /// </summary>
    [HttpGet("health")]
    public ActionResult<object> Health()
    {
        return Ok(new
        {
            status = "healthy",
            service = "TextLiving Campaign Generator",
            timestamp = DateTime.UtcNow
        });
    }
}
