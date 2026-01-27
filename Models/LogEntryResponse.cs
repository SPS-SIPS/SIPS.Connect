namespace SIPS.Connect.Models;

public record LogFileResponse(
    string FileName,
    DateTime LastWriteTime,
    long SizeBytes,
    string Size);

