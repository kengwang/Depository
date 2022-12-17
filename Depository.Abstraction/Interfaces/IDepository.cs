using Depository.Abstraction.Models;

namespace Depository.Abstraction.Interfaces;

public interface IDepository : 
    IDependencyDepository,
    IRelationDepository,
    IDepositoryResolve,
    IDepositoryServiceRunner
{

    
}