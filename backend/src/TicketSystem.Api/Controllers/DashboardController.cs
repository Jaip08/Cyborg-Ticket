using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Application.Features.Dashboard;
using TicketSystem.Application.Features.Dashboard.Queries;

namespace TicketSystem.Api.Controllers;

[Authorize]
public class DashboardController : ApiControllerBase
{
    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsDto>> Stats()
        => Ok(await Mediator.Send(new GetDashboardStatsQuery()));

    [HttpGet("monthly")]
    public async Task<ActionResult<List<MonthlyPointDto>>> Monthly([FromQuery] int months = 6)
        => Ok(await Mediator.Send(new GetMonthlyAnalyticsQuery(months)));

    [HttpGet("status-breakdown")]
    public async Task<ActionResult<List<StatusCountDto>>> StatusBreakdown()
        => Ok(await Mediator.Send(new GetStatusBreakdownQuery()));

    [HttpGet("priority-breakdown")]
    public async Task<ActionResult<List<PriorityCountDto>>> PriorityBreakdown()
        => Ok(await Mediator.Send(new GetPriorityBreakdownQuery()));

    [HttpGet("employee-performance")]
    public async Task<ActionResult<List<EmployeePerformanceDto>>> EmployeePerformance()
        => Ok(await Mediator.Send(new GetEmployeePerformanceQuery()));
}
