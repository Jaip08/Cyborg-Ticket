using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Application.Features.Categories;
using TicketSystem.Application.Features.Categories.Commands;
using TicketSystem.Application.Features.Categories.Queries;
using TicketSystem.Domain.Constants;

namespace TicketSystem.Api.Controllers;

[Authorize]
public class CategoriesController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll()
        => Ok(await Mediator.Send(new GetCategoriesQuery()));

    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CreateCategoryCommand command)
        => Ok(await Mediator.Send(command));
}
