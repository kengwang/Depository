namespace Depository.Abstraction.Interfaces.Pipeline;

public interface IPipelineContext<TContext, TReturnValue> where TContext : IPipelineContext<TContext, TReturnValue> where TReturnValue : class
{
    public List<IPipelineMiddleware<TContext, TReturnValue>> Middlewares { get; set; }
    public int CurrentIndex { get; set; }
}