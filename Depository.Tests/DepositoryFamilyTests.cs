using Depository.Core;
using Depository.Extensions;
using Depository.Tests.Implements;
using Depository.Tests.Interfaces;
using FluentAssertions;
using Xunit;

namespace Depository.Tests;

public class DepositoryFamilyTests
{
    [Fact]
    public async void InjectCustomFather()
    {
        // Arrange
        var depository = DepositoryFactory.CreateNew();
        await depository.AddSingletonAsync<IConstructorInjectService, CustomFatherService>();
        var guidGen = new EmptyGuidGenerator();

        // Action
        var result = await depository.ResolveAsync<IConstructorInjectService>(fatherImplementations: new Dictionary<Type, object>
            { { typeof(IGuidGenerator), guidGen } });
        var fathers = await depository.GetParentsAsync(result);
        var children = await depository.GetChildrenAsync(guidGen);
        
        // Assert
        result.Should().NotBeNull();
        children.Should().OnlyContain(t => t.Equals(result));
        fathers.Should().OnlyContain(t => t.Equals(guidGen));

    }
}