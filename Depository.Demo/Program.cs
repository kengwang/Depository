using Depository.Core;
using Depository.Demo;
using Depository.Extensions;

var depository = DepositoryFactory.CreateNew();
await depository.AddSingletonAsync<IGuidGenerator, GuidGenerator>();
await depository.AddSingletonAsync<IRandomProvider, MockRandomProvider>();
await depository.AddSingletonAsync<IRandomProvider, SharedRandomProvider>();
var guidGeneratorA = await depository.Resolve<IGuidGenerator>();
Console.WriteLine($"Previous Random is {guidGeneratorA.GetRandom()}");
await depository.ChangeFocusingRelation<IRandomProvider, MockRandomProvider>();
Console.WriteLine($"Current Random is {guidGeneratorA.GetRandom()}");


Console.Read();