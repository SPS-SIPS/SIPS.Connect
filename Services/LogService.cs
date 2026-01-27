using SIPS.Connect.Models;

namespace SIPS.Connect.Services;

public interface ILogService
{
    Task<IReadOnlyList<LogFileResponse>> GetLogFilesAsync();
    
    Task<byte[]?> DownloadLogFileAsync(
        string fileName,
        CancellationToken cancellationToken = default);
}

public class LogService(
    ILogger<LogService> logger)
    : ILogService
{
    public async Task<IReadOnlyList<LogFileResponse>> GetLogFilesAsync()
    {
        var logsPath = ResolveLogsPath();

        if (!Directory.Exists(logsPath))
        {
            logger.LogWarning("Logs directory not found at {Path}", logsPath);
            return [];
        }

        var files = Directory.GetFiles(logsPath, "log*.log")
            .Select(f => new FileInfo(f))
            .OrderByDescending(f => f.LastWriteTime);

        var results = files.Select(file => new LogFileResponse(
            file.Name,
            file.LastWriteTime,
            file.Length,
            FormatBytes(file.Length)
        )).ToList();

        return await Task.FromResult(results);
    }

    public async Task<byte[]?> DownloadLogFileAsync(
        string fileName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name must be provided.", nameof(fileName));
        }

        if (!IsSafeFileName(fileName))
        {
            throw new ArgumentException("Unsafe file name.", nameof(fileName));
        }

        var logsPath = ResolveLogsPath();
        var fullPath = Path.Combine(logsPath, fileName);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("Log file not found.", fileName);
        }

        logger.LogInformation("Downloading log file {File}", fullPath);
        
        return await File.ReadAllBytesAsync(fullPath, cancellationToken);
    }
    
    private static string ResolveLogsPath()
    {
        return Directory.Exists("/logs") ? "/logs" : Path.Combine(AppContext.BaseDirectory, "logs");
    }

    private static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
        if (bytes == 0) return "0 B";

        // Handle negative inputs
        var absoluteBytes = Math.Abs(bytes);

        // Calculate the order of magnitude using Logarithms
        var order = (int)Math.Floor(Math.Log(absoluteBytes, 1024));

        // Clamp the order to the max index of our array
        order = Math.Min(order, sizes.Length - 1);

        var adjustedSize = bytes / Math.Pow(1024, order);

        return $"{adjustedSize:0.##} {sizes[order]}";
    }

    private static bool IsSafeFileName(string fileName)
    {
        return !fileName.Contains("..") &&
               !fileName.Contains(Path.DirectorySeparatorChar) &&
               !fileName.Contains(Path.AltDirectorySeparatorChar);
    }
}