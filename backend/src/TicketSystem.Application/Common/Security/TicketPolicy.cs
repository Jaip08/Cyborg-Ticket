using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Constants;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Common.Security;

/// <summary>
/// Central place for the "who can touch a ticket" rules so handlers don't drift apart.
/// </summary>
public static class TicketPolicy
{
    public static bool IsStaff(ICurrentUser user) =>
        user.IsInRole(Roles.Admin) || user.IsInRole(Roles.Manager);

    public static bool CanView(ICurrentUser user, Ticket ticket) =>
        IsStaff(user) || ticket.CreatedById == user.Id || ticket.AssignedToId == user.Id;

    public static bool CanEdit(ICurrentUser user, Ticket ticket) =>
        IsStaff(user) || ticket.CreatedById == user.Id || ticket.AssignedToId == user.Id;

    public static bool CanDelete(ICurrentUser user, Ticket ticket) =>
        IsStaff(user) || ticket.CreatedById == user.Id;

    public static bool CanAssign(ICurrentUser user) => IsStaff(user);

    public static bool CanChangeStatus(ICurrentUser user, Ticket ticket) =>
        IsStaff(user) || ticket.AssignedToId == user.Id || ticket.CreatedById == user.Id;

    public static bool CanSeeInternalNotes(ICurrentUser user, Ticket ticket) =>
        IsStaff(user) || ticket.AssignedToId == user.Id;
}
