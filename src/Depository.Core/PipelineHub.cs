using Depository.Abstraction.Interfaces.Pipeline;

namespace Depository.Core;

public class PipelineHub<TContext, TReturnValue> : IPipeline<TContext, TReturnValue>
    where TContext : IPipelineContext<TContext, TReturnValue> where TReturnValue : class
{
    public readonly List<IPipelineMiddleware<TContext, TReturnValue>> Middlewares = new();

    public async Task<TReturnValue?> InvokeAsync(TContext context, CancellationToken cancellationToken = default)
    {
        context.Middlewares = Middlewares;
        context.CurrentIndex = -1;
        return await InvokeNextMiddleware(context, cancellationToken);
    }

    public async Task<TReturnValue?> InvokeNextMiddleware(TContext context,
                                                          CancellationToken cancellationToken = default)
    {
        if (context.CurrentIndex + 1 < context.Middlewares.Count)
        {
            return await context.Middlewares[++context.CurrentIndex]
                                .InvokeAsync(context, InvokeNextMiddleware, cancellationToken);
        }

        return null;
    }
}
