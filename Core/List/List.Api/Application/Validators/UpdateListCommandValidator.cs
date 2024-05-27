using FluentValidation;
using RecAll.Core.List.Api.Application.Commands;
using RecAll.Core.List.Api.Infrastructure.Services;
using RecAll.Core.List.Domain.AggregateModels.ListAggregate;

namespace RecAll.Core.List.Api.Application.Validators;

public class UpdateListCommandValidator : AbstractValidator<UpdateListCommand> {
    public UpdateListCommandValidator(IIdentityService identityService,
        IListRepository listRepository,
        ILogger<UpdateListCommandValidator> logger) {
        RuleFor(p => p.Id).NotEmpty();
        RuleFor(p => p.Id).MustAsync(async (p, _) => {
            var userIdentityGuid = identityService.GetUserIdentityGuid();
            var isValid =
                await listRepository.GetAsync(p, userIdentityGuid) is not null;

            if (!isValid) {
                logger.LogWarning(
                    $"用户{userIdentityGuid}尝试更新已删除、不存在或不属于自己的List {p}");
            }

            return isValid;
        }).WithMessage("无效的List ID");
        RuleFor(p => p.Name).NotEmpty();
        logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
    }
}