namespace TicketSystem.Application.Features.Dashboard;

public class DashboardStatsDto
{
    public int TotalTickets { get; set; }
    public int OpenTickets { get; set; }
    public int InProgressTickets { get; set; }
    public int OnHoldTickets { get; set; }
    public int ResolvedTickets { get; set; }
    public int ClosedTickets { get; set; }
    public int HighPriorityTickets { get; set; }
    public int OverdueTickets { get; set; }
    public int UnassignedTickets { get; set; }
}

public class MonthlyPointDto
{
    public string Month { get; set; } = default!;
    public int Created { get; set; }
    public int Resolved { get; set; }
}

public class StatusCountDto
{
    public string Status { get; set; } = default!;
    public int Count { get; set; }
}

public class PriorityCountDto
{
    public string Priority { get; set; } = default!;
    public int Count { get; set; }
}

public class EmployeePerformanceDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = default!;
    public int Assigned { get; set; }
    public int Resolved { get; set; }
    public int Open { get; set; }
}
