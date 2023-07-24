﻿using Depository.Abstraction.Interfaces;

namespace Depository.Abstraction.Models.Options;

public class DependencyResolveOption
{
    public IDepositoryResolveScope? Scope { get; set; } = null;
    public bool IncludeDisabled { get; set; } = false;
    public string? RelationName { get; set; } = null;
    public bool CheckAsyncConstructor { get; set; } = true;
    public bool ThrowWhenNotExists { get; set; } = true;
    public Dictionary<Type,object>? FatherImplementations { get; set; } = null;
}