namespace TicketSystem.Application.Common.Interfaces;

public interface IFileStorage
{
    Task<StoredFile> SaveAsync(Stream content, string originalFileName, CancellationToken cancellationToken = default);
    Task<Stream?> OpenAsync(string storedName, CancellationToken cancellationToken = default);
    void Delete(string storedName);
}

public record StoredFile(string StoredName, long Size);
