# Depository

An IoC Container with fantastic feature.

| Unit Tests |                                               NuGet                                               |
|:----------:|:-------------------------------------------------------------------------------------------------:|
|[![Depository.Core Unit Tests](https://github.com/kengwang/Depository/actions/workflows/unit-test.yml/badge.svg)](https://github.com/kengwang/Depository/actions/workflows/unit-test.yml) | [![Nuget](https://img.shields.io/nuget/v/Depository)](https://www.nuget.org/packages/Depository/) |

## Feature

* Mutable Dependency Implementation
* Notify Implementation Change
* Async Construct (resolve `Task<T>` or fire-and-forget)
* Resolve with Constructor has Parameter That Are Not In Container
* Multiple Resolve Support (not only `IEnumerable<T>` but  `ResolveMultiple`)
* Nullable Injection
* Proxy/Decoration Service Support
* `Microsoft.Extension.Hosting` intergration
* Notification / Pipeline structure
* ~~Seemly high perfomance~~
* ......

Checkout [Wiki](https://github.com/kengwang/Depository/wiki)

## Licence

```
MIT License

Copyright (c) 2024 Kengwang

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

