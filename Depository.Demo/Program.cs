using Depository.Core;
using Depository.Demo;
using Depository.Extensions;

var depository = DepositoryFactory.CreateNew();
await depository.AddSingletonAsync<IGuidGenerator, GuidGenerator>();
await depository.AddSingletonAsync<IGuidGenerator, ActualGuidGenerator>();
await depository.AddSingletonAsync<IGuidGenerator, FakeGuidGenerator>();

await depository.AddSingletonAsync<IRandomProvider, MockRandomProvider>();
await depository.AddSingletonAsync<IRandomProvider, SharedRandomProvider>();

await depository.AddSingletonAsync(typeof(ITypeGeneric<>), typeof(TypeGeneric<>));

var typeGeneric = await depository.ResolveAsync<ITypeGeneric<string>>();


Console.WriteLine(typeGeneric);

Console.Read();