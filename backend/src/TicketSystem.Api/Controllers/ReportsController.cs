using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Features.Dashboard;
using TicketSystem.Application.Features.Dashboard.Queries;
using TicketSystem.Application.Features.Reports;

namespace TicketSystem.Api.Controllers;

[Authorize]
public class ReportsController : ApiControllerBase
{
    [HttpGet("by-status")]
    public async Task<ActionResult<List<StatusCountDto>>> ByStatus()
        => Ok(await Mediator.Send(new GetStatusBreakdownQuery()));

    [HttpGet("by-priority")]
    public async Task<ActionResult<List<PriorityCountDto>>> ByPriority()
        => Ok(await Mediator.Send(new GetPriorityBreakdownQuery()));

    [HttpGet("by-employee")]
    public async Task<ActionResult<List<EmployeePerformanceDto>>> ByEmployee()
        => Ok(await Mediator.Send(new GetEmployeePerformanceQuery()));

    [HttpGet("monthly-trend")]
    public async Task<ActionResult<List<MonthlyPointDto>>> MonthlyTrend([FromQuery] int months = 6)
        => Ok(await Mediator.Send(new GetMonthlyAnalyticsQuery(months)));

    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] string format = "csv", [FromQuery] string report = "tickets")
    {
        var exportFormat = format.ToLowerInvariant() is "excel" or "xlsx"
            ? ExportFormat.Excel
            : ExportFormat.Csv;

        var file = await Mediator.Send(new ExportTicketsReportQuery(exportFormat));
        return File(file.Content, file.ContentType, file.FileName);
    }
}
