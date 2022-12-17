using Depository.Core;
using Depository.Demo;
using Depository.Extensions;

var depository = DepositoryFactory.CreateNew();
await depository.AddSingletonAsync<IGuidGenerator, GuidGenerator>();
var guidGeneratorA = await depository.Resolve<IGuidGenerator>();
var guidGeneratorB = await depository.Resolve<IGuidGenerator>();

Console.Read();