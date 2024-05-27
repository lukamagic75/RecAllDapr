using MediatR;

namespace RecAll.Infrastructure.Ddd.Infrastructure;

public class NoMediator : IMediator {
    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = new()) =>
        Task.FromResult(default(TResponse));

    public Task Send<TRequest>(TRequest request,
        CancellationToken cancellationToken = new CancellationToken())
        where TRequest : IRequest =>
        Task.CompletedTask;

    public Task<object> Send(object request,
        CancellationToken cancellationToken = new()) =>
        Task.FromResult(default(object));

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
        IStreamRequest<TResponse> request,
        CancellationToken cancellationToken = new()) =>
        throw new Exception();

    public IAsyncEnumerable<object> CreateStream(object request,
        CancellationToken cancellationToken = new()) =>
        throw new Exception();

    public Task Publish(object notification,
        CancellationToken cancellationToken = new()) =>
        Task.CompletedTask;

    public Task Publish<TNotification>(TNotification notification,
        CancellationToken cancellationToken = new())
        where TNotification : INotification =>
        Task.CompletedTask;
}