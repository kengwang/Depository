﻿using System.Runtime.CompilerServices;

namespace Depository.Core;

public partial class Depository
{
    
    private readonly ConditionalWeakTable<object, List<WeakReference>> _fatherToChildRelation = new();
    private readonly ConditionalWeakTable<object, List<WeakReference>> _childToFatherRelation = new();
    public List<object> GetChildren(object father)
    {
        if (_fatherToChildRelation.TryGetValue(father, out var refs))
        {
            return refs.Select(t => t.Target).ToList();
        }

        return new List<object>();
    }

    public List<object> GetParents(object child)
    {
        if (_childToFatherRelation.TryGetValue(child, out var refs))
        {
            return refs.Select(t => t.Target).ToList();
        }

        return new List<object>();
    }
}