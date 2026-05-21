using Depository.Abstraction.Interfaces.Pipeline;
using Depository.Core;
using FluentAssertions;
using TUnit.Core;

namespace Depository.Tests;

public class DepositoryPipelineTests
{
    [Test]
    public async Task InvokeAsync_WithoutMiddlewares_ShouldReturnNullAndInitializeContext()
    {
        var pipeline = new PipelineHub<TestPipelineContext, string>();
        var context = new TestPipelineContext();

        var result = await pipeline.InvokeAsync(context);

        result.Should().BeNull();
        context.Middlewares.Should().BeSameAs(pipeline.Middlewares);
        context.CurrentIndex.Should().Be(-1);
    }

    [Test]
    public async Task InvokeAsync_WithMiddlewares_ShouldInvokeInOrderAndReturnLastResult()
    {
        var pipeline = new PipelineHub<TestPipelineContext, string>();
        var context = new TestPipelineContext();
        pipeline.Middlewares.Add(new RecordingMiddleware("first"));
        pipeline.Middlewares.Add(new RecordingMiddleware("second", "done"));

        var result = await pipeline.InvokeAsync(context);

        result.Should().Be("done");
        context.Calls.Should().Equal("first:before", "second:before", "second:after", "first:after");
        context.CurrentIndex.Should().Be(1);
    }

    [Test]
    public async Task InvokeNextMiddleware_WhenNoMiddlewareRemains_ShouldReturnNull()
    {
        var pipeline = new PipelineHub<TestPipelineContext, string>();
        var context = new TestPipelineContext
        {
            Middlewares = pipeline.Middlewares,
            CurrentIndex = -1
        };

        var result = await pipeline.InvokeNextMiddleware(context);

        result.Should().BeNull();
    }

    private sealed class TestPipelineContext : IPipelineContext<TestPipelineContext, string>
    {
        public List<IPipelineMiddleware<TestPipelineContext, string>> Middlewares { get; set; } = new();
        public int CurrentIndex { get; set; }
        public List<string> Calls { get; } = new();
    }

    private sealed class RecordingMiddleware : IPipelineMiddleware<TestPipelineContext, string>
    {
        private readonly string _name;
        private readonly string? _result;

        public RecordingMiddleware(string name, string? result = null)
        {
            _name = name;
            _result = result;
        }

        public async Task<string?> InvokeAsync(
            TestPipelineContext context,
            IPipelineMiddleware<TestPipelineContext, string>.PipelineMiddlewareDelegate next,
            CancellationToken cancellationToken = default)
        {
            context.Calls.Add($"{_name}:before");
            var result = _result ?? await next(context, cancellationToken);
            context.Calls.Add($"{_name}:after");
            return result;
        }
    }
}
