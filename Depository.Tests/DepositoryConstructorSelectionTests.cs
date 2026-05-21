using System.Reflection;
using Depository.Abstraction.Interfaces.Pipeline;
using Depository.Abstraction.Models.Options;
using Depository.Core;
using Depository.Extensions;
using Depository.Tests.Implements;
using Depository.Tests.Interfaces;
using FluentAssertions;
using TUnit.Core;

namespace Depository.Tests;

public class DepositoryConstructorSelectionTests
{
    [Test]
    public void Resolve_WithActivatorConstructorAttribute_ShouldUseMarkedConstructor()
    {
        var depository = DepositoryFactory.CreateNew();
        depository.AddSingleton<MarkedConstructorService>();

        var service = depository.Resolve<MarkedConstructorService>();

        service.ConstructorName.Should().Be("marked");
    }

    [Test]
    public void Resolve_WithAutoConstructorEnabled_ShouldUseMostResolvableConstructor()
    {
        var depository = DepositoryFactory.CreateNew();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        depository.AddSingleton<AutoConstructorService>();

        var service = depository.Resolve<AutoConstructorService>();

        service.ConstructorName.Should().Be("resolvable");
        service.GuidGenerator.Should().BeOfType<EmptyGuidGenerator>();
    }

    [Test]
    public void Resolve_WithAutoConstructorDisabledAndMultipleConstructors_ShouldThrow()
    {
        var depository = DepositoryFactory.CreateNew(option =>
            option.CheckerOption.AutoConstructor = false);
        depository.AddSingleton<AutoConstructorService>();

        var action = () => depository.Resolve<AutoConstructorService>();

        action.Should().Throw<Exception>()
            .WithMessage("*More than one constructor*");
    }

    [Test]
    public void Resolve_WithNoPublicConstructors_ShouldThrowInitializationException()
    {
        var depository = DepositoryFactory.CreateNew();
        depository.AddSingleton<PrivateConstructorService>();

        var action = () => depository.Resolve<PrivateConstructorService>();

        action.Should().Throw<Exception>()
            .WithMessage("*Cannot initialize*");
    }

    [Test]
    public void Resolve_WithFixedImplementations_ShouldMakeConstructorResolvableForSelection()
    {
        var depository = DepositoryFactory.CreateNew();
        depository.AddSingleton<AutoConstructorService>();
        var fixedImplementations = new Dictionary<Type, Dictionary<string, object>>
        {
            [typeof(IGuidGenerator)] = new()
            {
                [string.Empty] = new EmptyGuidGenerator()
            }
        };

        var service = depository.Resolve<AutoConstructorService>(fixedImplementations: fixedImplementations);

        service.ConstructorName.Should().Be("resolvable");
        service.GuidGenerator.Should().BeOfType<EmptyGuidGenerator>();
    }

    [Test]
    public void GetOrCreatePipeline_ShouldCreateAndReusePipelineDependency()
    {
        var depository = DepositoryFactory.CreateNew();
        var method = typeof(Core.Depository).GetMethod(
            "GetOrCreatePipeline",
            BindingFlags.Instance | BindingFlags.NonPublic)!
            .MakeGenericMethod(typeof(ReflectionPipelineContext), typeof(string));

        var firstPipeline = method.Invoke(depository, null);
        var secondPipeline = method.Invoke(depository, null);

        firstPipeline.Should().BeOfType<PipelineHub<ReflectionPipelineContext, string>>();
        secondPipeline.Should().BeSameAs(firstPipeline);
    }

    public sealed class MarkedConstructorService
    {
        public MarkedConstructorService()
        {
            ConstructorName = "default";
        }

        [Abstraction.Models.DepositoryActivatorConstructor]
        public MarkedConstructorService(string value = "marked")
        {
            ConstructorName = value;
        }

        public string ConstructorName { get; }
    }

    public sealed class AutoConstructorService
    {
        public AutoConstructorService()
        {
            ConstructorName = "default";
        }

        public AutoConstructorService(IGuidGenerator guidGenerator)
        {
            ConstructorName = "resolvable";
            GuidGenerator = guidGenerator;
        }

        public string ConstructorName { get; }
        public IGuidGenerator? GuidGenerator { get; }
    }

    public sealed class PrivateConstructorService
    {
        private PrivateConstructorService()
        {
        }
    }

    public sealed class ReflectionPipelineContext : IPipelineContext<ReflectionPipelineContext, string>
    {
        public List<IPipelineMiddleware<ReflectionPipelineContext, string>> Middlewares { get; set; } = new();
        public int CurrentIndex { get; set; }
    }
}
