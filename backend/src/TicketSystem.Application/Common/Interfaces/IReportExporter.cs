namespace TicketSystem.Application.Common.Interfaces;

public enum ExportFormat
{
    Csv,
    Excel
}

public interface IReportExporter
{
    ExportFile Write<T>(IReadOnlyCollection<T> rows, ExportFormat format, string fileBaseName);
}

public record ExportFile(byte[] Content, string ContentType, string FileName);
