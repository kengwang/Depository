using Depository.Abstraction.Models;

namespace Depository.Abstraction.Interfaces;

public interface IDepository :
    IDepositoryDependency,
    IRelationDepository,
    IDepositoryResolve,
    IDepositoryImplementation,
    IDepositoryFamily,
    IDisposable
{
}