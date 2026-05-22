using BenchmarkDotNet.Attributes;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;
using Depository.Abstraction.Models.Options;
using Depository.Benchmarks.Implements;
using Depository.Benchmarks.Interfaces;
using Depository.Core;
using Depository.Extensions;

namespace Depository.Benchmarks;

public partial class Benchmarks
{
    private IDepository _singletonDepository = null!;
    private IDepository _transientConstructorDepository = null!;
    private IDepository _scopedNamedDepository = null!;
    private IDepositoryResolveScope _scope = null!;
    private IDepository _openGenericDepository = null!;
    private IDepository _decoratedDepository = null!;

    [GlobalSetup]
    public void SetupResolveHotPaths()
    {
        _singletonDepository = DepositoryFactory.CreateNew();
        _singletonDepository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();

        _transientConstructorDepository = DepositoryFactory.CreateNew();
        _transientConstructorDepository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        _transientConstructorDepository.AddTransient<ConstructorConsumer>();

        _scopedNamedDepository = DepositoryFactory.CreateNew();
        _scopedNamedDepository.AddScoped<IGuidGenerator, RandomGuidGenerator>("a");
        _scopedNamedDepository.AddScoped<IGuidGenerator, RandomGuidGenerator>("b");
        _scope = _scopedNamedDepository.CreateScope();

        _openGenericDepository = DepositoryFactory.CreateNew();
        _openGenericDepository.AddSingleton(typeof(IGenericService<>), typeof(GenericService<>));

        _decoratedDepository = DepositoryFactory.CreateNew();
        _decoratedDepository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        var description = _decoratedDepository.GetDependency(typeof(IGuidGenerator))!;
        _decoratedDepository.SetDependencyDecoration(description,
            new DependencyRelation(typeof(GuidGeneratorDecorator), IsDecorationRelation: true));
    }

    [Benchmark]
    public IGuidGenerator WarmSingletonResolve()
    {
        return _singletonDepository.Resolve<IGuidGenerator>();
    }

    [Benchmark]
    public ConstructorConsumer TransientConstructorResolve()
    {
        return _transientConstructorDepository.Resolve<ConstructorConsumer>();
    }

    [Benchmark]
    public IGuidGenerator NamedScopedResolveCacheHit()
    {
        return _scopedNamedDepository.ResolveInScope<IGuidGenerator>(_scope,
            new DependencyResolveOption { RelationName = "a" });
    }

    [Benchmark]
    public IGenericService<string> OpenGenericResolve()
    {
        return _openGenericDepository.Resolve<IGenericService<string>>();
    }

    [Benchmark]
    public IGuidGenerator DecoratedResolve()
    {
        return _decoratedDepository.Resolve<IGuidGenerator>();
    }

    public sealed class ConstructorConsumer
    {
        public ConstructorConsumer(IGuidGenerator generator)
        {
            Generator = generator;
        }

        public IGuidGenerator Generator { get; }
    }

    public interface IGenericService<T>
    {
    }

    public sealed class GenericService<T> : IGenericService<T>
    {
    }

    public sealed class GuidGeneratorDecorator : IGuidGenerator, IDecorationService
    {
        private readonly IGuidGenerator _inner;

        public GuidGeneratorDecorator(IGuidGenerator inner)
        {
            _inner = inner;
        }

        public Guid GetGuid()
        {
            return _inner.GetGuid();
        }
    }
}
