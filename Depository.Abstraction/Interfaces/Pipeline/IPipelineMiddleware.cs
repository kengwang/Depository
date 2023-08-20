namespace Depository.Abstraction.Interfaces.Pipeline;

public interface IPipelineMiddleware<TContext, TReturnValue> where TContext : IPipelineContext<TContext, TReturnValue> where TReturnValue : class
{
    public delegate Task<TReturnValue?>
        PipelineMiddlewareDelegate(TContext context,
                                   CancellationToken cancellationToken = default);

    public Task<TReturnValue?> InvokeAsync(TContext context,
                                           PipelineMiddlewareDelegate next,
                                           CancellationToken cancellationToken = default);
}