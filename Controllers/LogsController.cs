using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIPS.Connect.Services;

namespace SIPS.Connect.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = KnownRoles.Logs)]
public class LogsController(ILogService logService, ILogger<LogsController> logger) : ControllerBase
{
    [HttpGet("files")]
    public async Task<IActionResult> GetLogFiles(CancellationToken cancellationToken)
    {
        try
        {
            var files = await logService.GetLogFilesAsync();
            return Ok(files);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve log files");
            return StatusCode(500, "Internal server error");
        }
    }


    [HttpGet("download/{fileName}")]
    public async Task<IActionResult> DownloadLogFile(string fileName, CancellationToken cancellationToken)
    {
        try
        {
            var fileBytes = await logService.DownloadLogFileAsync(fileName, cancellationToken);

            if (fileBytes != null) return File(fileBytes, "application/octet-stream", fileName);
            logger.LogWarning("Downloaded file is null: {FileName}", fileName);
            return NotFound(new { Message = "File not found or empty." });
        }
        catch (FileNotFoundException ex)
        {
            logger.LogWarning(ex, "Log file not found: {FileName}", fileName);
            return NotFound(new { ex.Message });
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid file name: {FileName}", fileName);
            return BadRequest(new { ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to download log file: {FileName}", fileName);
            return StatusCode(500, "Internal server error");
        }
    }
}