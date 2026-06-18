using BanSach.Application.Common;
using BanSach.Application.Dtos.Category;
using BanSach.Application.Interfaces;
using BanSach.Model;
using FluentValidation;
using MediatR;

namespace BanSach.Application.Features.Categories.Commands;

public sealed record CreateCategoryCommand(string Name, int DisplayOrder) : IRequest<Result<CategoryDto>>;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Name = request.Name,
            DisplayOrder = request.DisplayOrder,
            CreatedDate = DateTime.UtcNow
        };

        await _context.Categories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<CategoryDto>.Success(new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            DisplayOrder = category.DisplayOrder,
            CreatedDate = category.CreatedDate
        });
    }
}