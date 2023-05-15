using Ardalis.Specification;

namespace FinancialAccountManagementApi.Common.Contracts;

public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
{
}