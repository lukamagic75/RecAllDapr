using FluentValidation;
using MediatR;
using RecAll.Core.List.Domain.Exceptions;

namespace RecAll.Core.List.Api.Application.Behaviors;

public class
    ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest,
    TResponse> where TRequest : IRequest<TResponse> {
    private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> _logger;

    private readonly IValidator<TRequest>[] _validators;

    public ValidatorBehavior(
        ILogger<ValidatorBehavior<TRequest, TResponse>> logger,
        IValidator<TRequest>[] validators) {
        _logger = logger;
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken) {
        var typeName = request.GetType().Name;

        _logger.LogInformation("----- Validating command {CommandType}",
            typeName);

        var failures =
            (await Task.WhenAll(_validators.Select(p =>
                p.ValidateAsync(request, cancellationToken))))
            .SelectMany(p => p.Errors).Where(p => p != null).ToList();

        if (failures.Any()) {
            _logger.LogWarning(
                "Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}",
                typeName, request, failures);

            throw new ListDomainException(
                $"Command Validation Errors for type {typeof(TRequest).Name}",
                new ValidationException("Validation exception", failures));
        }

        return await next();
    }
}