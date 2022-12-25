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

var guidGenerators = (await depository.Resolve<IEnumerable<IGuidGenerator>>()).ToList();

var guidGenerator = guidGenerators[0];

Console.WriteLine(guidGenerator.GetRandom());

Console.Read();