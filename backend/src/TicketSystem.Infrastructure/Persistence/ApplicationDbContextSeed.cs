using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Constants;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Infrastructure.Persistence;

public static class ApplicationDbContextSeed
{
    public static async Task SeedAsync(ApplicationDbContext db, IPasswordHasher hasher, CancellationToken ct = default)
    {
        await SeedRolesAsync(db, ct);
        await SeedCategoriesAsync(db, ct);
        await SeedUsersAsync(db, hasher, ct);
        await SeedTicketsAsync(db, ct);
    }

    private static async Task SeedRolesAsync(ApplicationDbContext db, CancellationToken ct)
    {
        if (await db.Roles.AnyAsync(ct))
            return;

        db.Roles.AddRange(
            new Role { Name = Roles.Admin, Description = "Full administrative access" },
            new Role { Name = Roles.Manager, Description = "Manages tickets, assignments and reporting" },
            new Role { Name = Roles.Employee, Description = "Raises and works on tickets" });

        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedCategoriesAsync(ApplicationDbContext db, CancellationToken ct)
    {
        if (await db.Categories.AnyAsync(ct))
            return;

        db.Categories.AddRange(
            new Category { Name = "Hardware", Description = "Laptops, peripherals and physical equipment" },
            new Category { Name = "Software", Description = "Applications, licences and installs" },
            new Category { Name = "Network", Description = "Connectivity, VPN and Wi-Fi issues" },
            new Category { Name = "Account & Access", Description = "Logins, permissions and provisioning" },
            new Category { Name = "Billing", Description = "Invoices, subscriptions and payments" },
            new Category { Name = "General", Description = "Anything that doesn't fit elsewhere" });

        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedUsersAsync(ApplicationDbContext db, IPasswordHasher hasher, CancellationToken ct)
    {
        if (await db.Users.AnyAsync(ct))
            return;

        var roles = await db.Roles.ToDictionaryAsync(r => r.Name, ct);

        db.Users.AddRange(
            new User
            {
                FullName = "Site Administrator",
                Email = "admin@ticket.local",
                PasswordHash = hasher.Hash("Admin@123"),
                RoleId = roles[Roles.Admin].Id
            },
            new User
            {
                FullName = "Maria Chen",
                Email = "manager@ticket.local",
                PasswordHash = hasher.Hash("Manager@123"),
                RoleId = roles[Roles.Manager].Id
            },
            new User
            {
                FullName = "Daniel Okafor",
                Email = "employee@ticket.local",
                PasswordHash = hasher.Hash("Employee@123"),
                RoleId = roles[Roles.Employee].Id
            },
            new User
            {
                FullName = "Priya Nair",
                Email = "priya@ticket.local",
                PasswordHash = hasher.Hash("Employee@123"),
                RoleId = roles[Roles.Employee].Id
            });

        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedTicketsAsync(ApplicationDbContext db, CancellationToken ct)
    {
        if (await db.Tickets.AnyAsync(ct))
            return;

        var categories = await db.Categories.ToDictionaryAsync(c => c.Name, ct);
        var users = await db.Users.ToDictionaryAsync(u => u.Email, ct);

        var admin = users["admin@ticket.local"];
        var manager = users["manager@ticket.local"];
        var daniel = users["employee@ticket.local"];
        var priya = users["priya@ticket.local"];

        var now = DateTime.UtcNow;

        var tickets = new List<Ticket>
        {
            Build("Laptop won't power on after update", "Since the latest Windows update my laptop shows a black screen on boot. Tried holding the power button with no luck.",
                TicketPriority.High, TicketStatus.InProgress, categories["Hardware"], daniel, priya, now.AddMonths(-4).AddDays(2), dueInDays: 1),

            Build("VPN drops every few minutes", "The corporate VPN disconnects roughly every five minutes, which makes remote work almost impossible.",
                TicketPriority.Critical, TicketStatus.Open, categories["Network"], manager, daniel, now.AddMonths(-3).AddDays(5), dueInDays: -2),

            Build("Request: Adobe Photoshop licence", "I need a Photoshop licence for the new marketing campaign assets.",
                TicketPriority.Low, TicketStatus.OnHold, categories["Software"], priya, null, now.AddMonths(-3).AddDays(12)),

            Build("Cannot access shared finance drive", "Permission denied when opening the finance shared folder. I had access last week.",
                TicketPriority.Medium, TicketStatus.Resolved, categories["Account & Access"], daniel, manager, now.AddMonths(-2).AddDays(3), resolvedAfterDays: 2),

            Build("Duplicate charge on March invoice", "We were billed twice for the March subscription. Please review and refund the duplicate.",
                TicketPriority.Medium, TicketStatus.Closed, categories["Billing"], priya, daniel, now.AddMonths(-2).AddDays(9), resolvedAfterDays: 3, closedAfterDays: 4),

            Build("New starter onboarding setup", "Please prepare accounts, laptop and access for our new analyst starting next Monday.",
                TicketPriority.High, TicketStatus.InProgress, categories["Account & Access"], manager, priya, now.AddMonths(-1).AddDays(4), dueInDays: 3),

            Build("Printer on 3rd floor jamming", "The shared printer keeps jamming on double-sided prints. Paper tray two seems to be the issue.",
                TicketPriority.Low, TicketStatus.Open, categories["Hardware"], daniel, null, now.AddDays(-12)),

            Build("Email signature not applying", "The company email signature isn't being added to outgoing mail on the desktop client.",
                TicketPriority.Medium, TicketStatus.Open, categories["Software"], priya, daniel, now.AddDays(-3))
        };

        db.Tickets.AddRange(tickets);
        await db.SaveChangesAsync(ct);

        // A couple of tickets get some conversation and history so the timeline isn't empty.
        var vpn = tickets[1];
        var finance = tickets[3];

        db.TicketComments.AddRange(
            new TicketComment { TicketId = vpn.Id, AuthorId = daniel.Id, Content = "I've reproduced this on two different machines, so it looks network-side rather than a single laptop.", IsInternal = false, CreatedAt = vpn.CreatedAt.AddHours(3) },
            new TicketComment { TicketId = vpn.Id, AuthorId = manager.Id, Content = "Escalating to the network vendor. Flagging as critical for the morning.", IsInternal = true, CreatedAt = vpn.CreatedAt.AddHours(5) },
            new TicketComment { TicketId = finance.Id, AuthorId = manager.Id, Content = "Re-added you to the finance security group. Can you confirm access is back?", IsInternal = false, CreatedAt = finance.CreatedAt.AddDays(1) },
            new TicketComment { TicketId = finance.Id, AuthorId = daniel.Id, Content = "Confirmed, I can open the folder again. Thanks!", IsInternal = false, CreatedAt = finance.CreatedAt.AddDays(2) });

        db.ActivityLogs.AddRange(
            new ActivityLog { TicketId = vpn.Id, UserId = manager.Id, Action = "Created", Description = "Opened ticket at Critical priority.", CreatedAt = vpn.CreatedAt },
            new ActivityLog { TicketId = vpn.Id, UserId = manager.Id, Action = "Assigned", Description = "Assigned to Daniel Okafor.", CreatedAt = vpn.CreatedAt.AddHours(1) },
            new ActivityLog { TicketId = finance.Id, UserId = daniel.Id, Action = "Created", Description = "Opened ticket at Medium priority.", CreatedAt = finance.CreatedAt },
            new ActivityLog { TicketId = finance.Id, UserId = manager.Id, Action = "StatusChanged", Description = "Status changed from Open to Resolved.", CreatedAt = finance.CreatedAt.AddDays(2) });

        await db.SaveChangesAsync(ct);
    }

    private static Ticket Build(
        string title,
        string description,
        TicketPriority priority,
        TicketStatus status,
        Category category,
        User createdBy,
        User? assignedTo,
        DateTime createdAt,
        int? dueInDays = null,
        int? resolvedAfterDays = null,
        int? closedAfterDays = null)
    {
        return new Ticket
        {
            Title = title,
            Description = description,
            Priority = priority,
            Status = status,
            CategoryId = category.Id,
            CreatedById = createdBy.Id,
            AssignedToId = assignedTo?.Id,
            CreatedAt = createdAt,
            DueDate = dueInDays is null ? null : createdAt.AddDays(dueInDays.Value),
            ResolvedAt = resolvedAfterDays is null ? null : createdAt.AddDays(resolvedAfterDays.Value),
            ClosedAt = closedAfterDays is null ? null : createdAt.AddDays(closedAfterDays.Value)
        };
    }
}
