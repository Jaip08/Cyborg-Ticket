using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Features.Categories.Commands;

public record CreateCategoryCommand(string Name, string? Description) : IRequest<CategoryDto>;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IApplicationDbContext _db;

    public CreateCategoryCommandHandler(IUnitOfWork uow, IApplicationDbContext db)
    {
        _uow = uow;
        _db = db;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        var exists = await _db.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);
        if (exists)
            throw new ConflictException($"A category named \"{name}\" already exists.");

        var category = new Category { Name = name, Description = request.Description?.Trim() };
        await _uow.Categories.AddAsync(category, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return new CategoryDto { Id = category.Id, Name = category.Name, Description = category.Description };
    }
}
