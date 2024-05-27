using System.Reflection;
using Autofac;
using FluentValidation;
using MediatR;
using RecAll.Core.List.Api.Application.Behaviors;
using RecAll.Core.List.Api.Application.Commands;
using RecAll.Core.List.Api.Application.Validators;
using Module = Autofac.Module;

namespace RecAll.Core.List.Api.Infrastructure.AutofacModules;

public class MediatorModule : Module {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
            .AsImplementedInterfaces();

        builder.RegisterAssemblyTypes(typeof(CreateListCommand).GetTypeInfo()
            .Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));

        builder.RegisterAssemblyTypes(typeof(CreateListCommandValidator)
                .GetTypeInfo().Assembly)
            .Where(p => p.IsClosedTypeOf(typeof(IValidator<>)))
            .AsImplementedInterfaces();

        builder.RegisterGeneric(typeof(LoggingBehavior<,>))
            .As(typeof(IPipelineBehavior<,>));
        
        builder.RegisterGeneric(typeof(ValidatorBehavior<,>))
            .As(typeof(IPipelineBehavior<,>));
        
        builder.RegisterGeneric(typeof(TransactionBehaviour<,>))
            .As(typeof(IPipelineBehavior<,>));
    }
}