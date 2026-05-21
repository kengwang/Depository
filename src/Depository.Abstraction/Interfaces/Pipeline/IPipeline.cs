namespace Depository.Abstraction.Interfaces.Pipeline;

public interface IPipeline<TContext, TReturnValue> where TContext : IPipelineContext<TContext, TReturnValue> where TReturnValue : class
{
    public Task<TReturnValue?> InvokeAsync(TContext context, CancellationToken cancellationToken = default);
}