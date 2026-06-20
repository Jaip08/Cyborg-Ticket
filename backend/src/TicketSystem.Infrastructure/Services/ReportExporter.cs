using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using CsvHelper;
using TicketSystem.Application.Common.Interfaces;

namespace TicketSystem.Infrastructure.Services;

public class ReportExporter : IReportExporter
{
    private const string ExcelContentType =
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public ExportFile Write<T>(IReadOnlyCollection<T> rows, ExportFormat format, string fileBaseName)
        => format == ExportFormat.Excel
            ? ToExcel(rows, fileBaseName)
            : ToCsv(rows, fileBaseName);

    private static ExportFile ToCsv<T>(IReadOnlyCollection<T> rows, string baseName)
    {
        using var buffer = new MemoryStream();
        using (var writer = new StreamWriter(buffer, new UTF8Encoding(true), leaveOpen: true))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(rows);
        }

        return new ExportFile(buffer.ToArray(), "text/csv", $"{baseName}-{Stamp()}.csv");
    }

    private static ExportFile ToExcel<T>(IReadOnlyCollection<T> rows, string baseName)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet("Report");

        sheet.Cell(1, 1).InsertTable(rows, "Report", createTable: true);
        sheet.Columns().AdjustToContents();

        using var buffer = new MemoryStream();
        workbook.SaveAs(buffer);

        return new ExportFile(buffer.ToArray(), ExcelContentType, $"{baseName}-{Stamp()}.xlsx");
    }

    private static string Stamp() => DateTime.UtcNow.ToString("yyyyMMdd");
}
