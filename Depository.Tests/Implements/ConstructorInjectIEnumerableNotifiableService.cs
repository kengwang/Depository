﻿using Depository.Abstraction.Interfaces;
using Depository.Demo.Interfaces;

namespace Depository.Demo.Implements;

public class ConstructorInjectIEnumerableNotifiableService : IConstructorInjectService, INotifyDependencyChanged<IGuidGenerator>, ICheckIsNormal
{
    private readonly IEnumerable<IGuidGenerator> _generators;

    public ConstructorInjectIEnumerableNotifiableService(IEnumerable<IGuidGenerator> generators)
    {
        _generators = generators;
    }

    public bool IsNormal { get; set; }

    public Task OnDependencyChanged(IGuidGenerator? marker)
    {
        IsNormal = true;
        return Task.CompletedTask;
    }
}