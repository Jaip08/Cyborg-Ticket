using Microsoft.Extensions.Options;
using TicketSystem.Application.Common.Interfaces;

namespace TicketSystem.Infrastructure.Services;

public class LocalFileStorage : IFileStorage
{
    private readonly string _root;

    public LocalFileStorage(IOptions<FileStorageOptions> options)
    {
        var configured = options.Value.Path;
        _root = Path.IsPathRooted(configured)
            ? configured
            : Path.Combine(AppContext.BaseDirectory, configured);

        Directory.CreateDirectory(_root);
    }

    public async Task<StoredFile> SaveAsync(Stream content, string originalFileName, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(originalFileName);
        var storedName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(_root, storedName);

        await using var file = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write);
        await content.CopyToAsync(file, cancellationToken);
        await file.FlushAsync(cancellationToken);

        return new StoredFile(storedName, file.Length);
    }

    public Task<Stream?> OpenAsync(string storedName, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_root, storedName);
        if (!File.Exists(fullPath))
            return Task.FromResult<Stream?>(null);

        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        return Task.FromResult<Stream?>(stream);
    }

    public void Delete(string storedName)
    {
        var fullPath = Path.Combine(_root, storedName);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}
