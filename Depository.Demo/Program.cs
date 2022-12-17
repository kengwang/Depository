using Depository.Core;
using Depository.Demo;
using Depository.Extensions;

var depository = DepositoryFactory.CreateNew();
await depository.AddSingletonAsync<IGuidGenerator, GuidGenerator>();
await depository.AddSingletonAsync<IRandomProvider, MockRandomProvider>();
await depository.AddSingletonAsync<IRandomProvider, SharedRandomProvider>();
var guidGeneratorA = await depository.Resolve<IGuidGenerator>();
Console.WriteLine($"Previous Random is {guidGeneratorA.GetRandom()}");
var depDes = await depository.GetDependencyAsync(typeof(IRandomProvider));
var relations = await depository.GetRelationsAsync(depDes!);
await depository.ChangeFocusingRelationAsync(depDes!,
    relations.First(relation => relation.ImplementType == typeof(MockRandomProvider)));
Console.WriteLine($"Current Random is {guidGeneratorA.GetRandom()}");


Console.Read();