using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Application.Common.Models;
using TicketSystem.Application.Features.Activity;
using TicketSystem.Application.Features.Activity.Queries;
using TicketSystem.Application.Features.Attachments;
using TicketSystem.Application.Features.Attachments.Commands;
using TicketSystem.Application.Features.Attachments.Queries;
using TicketSystem.Application.Features.Comments;
using TicketSystem.Application.Features.Comments.Commands;
using TicketSystem.Application.Features.Comments.Queries;
using TicketSystem.Application.Features.Tickets;
using TicketSystem.Application.Features.Tickets.Commands;
using TicketSystem.Application.Features.Tickets.Queries;
using TicketSystem.Domain.Constants;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Api.Controllers;

[Authorize]
public class TicketsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<TicketDto>>> GetTickets([FromQuery] GetTicketsQuery query)
        => Ok(await Mediator.Send(query));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TicketDto>> GetById(Guid id)
        => Ok(await Mediator.Send(new GetTicketByIdQuery(id)));

    [HttpPost]
    public async Task<ActionResult<TicketDto>> Create(CreateTicketCommand command)
    {
        var ticket = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TicketDto>> Update(Guid id, UpdateTicketRequest request)
        => Ok(await Mediator.Send(new UpdateTicketCommand(
            id, request.Title, request.Description, request.Priority, request.CategoryId, request.DueDate)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteTicketCommand(id));
        return NoContent();
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    [HttpPost("{id:guid}/assign")]
    public async Task<ActionResult<TicketDto>> Assign(Guid id, AssignTicketRequest request)
        => Ok(await Mediator.Send(new AssignTicketCommand(id, request.AssigneeId)));

    [HttpPost("{id:guid}/status")]
    public async Task<ActionResult<TicketDto>> ChangeStatus(Guid id, ChangeStatusRequest request)
        => Ok(await Mediator.Send(new ChangeTicketStatusCommand(id, request.Status)));

    [HttpGet("{id:guid}/comments")]
    public async Task<ActionResult<List<CommentDto>>> GetComments(Guid id)
        => Ok(await Mediator.Send(new GetTicketCommentsQuery(id)));

    [HttpPost("{id:guid}/comments")]
    public async Task<ActionResult<CommentDto>> AddComment(Guid id, AddCommentRequest request)
        => Ok(await Mediator.Send(new AddCommentCommand(id, request.Content, request.IsInternal)));

    [HttpGet("{id:guid}/activity")]
    public async Task<ActionResult<List<ActivityDto>>> GetActivity(Guid id)
        => Ok(await Mediator.Send(new GetTicketActivityQuery(id)));

    [HttpGet("{id:guid}/attachments")]
    public async Task<ActionResult<List<AttachmentDto>>> GetAttachments(Guid id)
        => Ok(await Mediator.Send(new GetTicketAttachmentsQuery(id)));

    [HttpPost("{id:guid}/attachments")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<AttachmentDto>> UploadAttachment(Guid id, IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No file was uploaded." });

        await using var stream = file.OpenReadStream();
        var command = new UploadAttachmentCommand(id, stream, file.FileName, file.ContentType, file.Length);
        return Ok(await Mediator.Send(command));
    }

    [HttpGet("attachments/{attachmentId:guid}/download")]
    public async Task<IActionResult> DownloadAttachment(Guid attachmentId)
    {
        var file = await Mediator.Send(new DownloadAttachmentQuery(attachmentId));
        return File(file.Content, file.ContentType, file.FileName);
    }
}

public record UpdateTicketRequest(
    string Title,
    string Description,
    TicketPriority Priority,
    Guid CategoryId,
    DateTime? DueDate);

public record AssignTicketRequest(Guid AssigneeId);

public record ChangeStatusRequest(TicketStatus Status);

public record AddCommentRequest(string Content, bool IsInternal);
