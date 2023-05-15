using Ardalis.Specification.EntityFrameworkCore;
using FinancialAccountManagementApi.Common.Contracts;

namespace FinancialAccountManagementApi.Persistence;

public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T> where T : class, IAggregateRoot
{
    public EfRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}