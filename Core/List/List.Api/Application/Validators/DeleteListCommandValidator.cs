using FluentValidation;
using RecAll.Core.List.Api.Application.Commands;
using RecAll.Core.List.Api.Infrastructure.Services;
using RecAll.Core.List.Domain.AggregateModels.ListAggregate;

namespace RecAll.Core.List.Api.Application.Validators;

public class DeleteListCommandValidator : AbstractValidator<DeleteListCommand> {
    public DeleteListCommandValidator(IIdentityService identityService,
        IListRepository listRepository,
        ILogger<DeleteListCommandValidator> logger) {
        RuleFor(p => p.Id).NotEmpty();
        RuleFor(p => p.Id).MustAsync(async (p, _) => {
            var userIdentityGuid = identityService.GetUserIdentityGuid();
            var isValid =
                await listRepository.GetAsync(p, userIdentityGuid) is not null;

            if (!isValid) {
                logger.LogWarning(
                    $"用户{userIdentityGuid}尝试删除已删除、不存在或不属于自己的List {p}");
            }

            return isValid;
        }).WithMessage("无效的List ID");
        logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
    }
}