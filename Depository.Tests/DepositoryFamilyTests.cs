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
        depository.AddSingleton<IConstructorInjectService, CustomFatherService>();
        var guidGen = new EmptyGuidGenerator();

        // Action
        var result = depository.Resolve<IConstructorInjectService>(fatherImplementations: new Dictionary<Type, object>
            { { typeof(IGuidGenerator), guidGen } });
        var fathers = depository.GetParents(result);
        var children = depository.GetChildren(guidGen);
        
        // Assert
        result.Should().NotBeNull();
        children.Should().OnlyContain(t => t.Equals(result));
        fathers.Should().OnlyContain(t => t.Equals(guidGen));

    }
}