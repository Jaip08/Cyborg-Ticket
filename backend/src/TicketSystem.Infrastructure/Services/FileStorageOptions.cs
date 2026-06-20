namespace TicketSystem.Infrastructure.Services;

public class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    // Relative paths are resolved against the app's base directory.
    public string Path { get; set; } = "uploads";
}
