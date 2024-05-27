using FluentValidation;
using RecAll.Core.List.Api.Application.Commands;
using RecAll.Core.List.Domain.AggregateModels;
using RecAll.Infrastructure.Ddd.Domain.SeedWork;

namespace RecAll.Core.List.Api.Application.Validators;

public class CreateListCommandValidator : AbstractValidator<CreateListCommand> {
    public CreateListCommandValidator(
        ILogger<CreateListCommandValidator> logger) {
        RuleFor(p => p.Name).NotEmpty();
        RuleFor(p => p.TypeId).NotEmpty();
        RuleFor(p => p.TypeId).Must(Enumeration.IsValidValue<ListType>)
            .WithMessage("无效的Type ID");
        logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
    }
}