﻿using Depository.Demo.Interfaces;

namespace Depository.Demo.Implements;

public class ConstructorInjectService : IConstructorInjectService, ICheckIsNormal
{
    private IGuidGenerator _generator;
    
    public ConstructorInjectService(IGuidGenerator generator)
    {
        _generator = generator;
        IsNormal = true;
    }

    public bool IsNormal { get; set; }
}