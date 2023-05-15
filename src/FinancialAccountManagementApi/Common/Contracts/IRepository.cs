using Ardalis.Specification;

namespace FinancialAccountManagementApi.Common.Contracts;

public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
{
}