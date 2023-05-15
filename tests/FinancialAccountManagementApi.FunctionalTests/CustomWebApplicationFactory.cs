using FinancialAccountManagementApi.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialAccountManagementApi.FunctionalTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder
        .ConfigureServices(services =>
        {
          ServiceDescriptor? descriptor = services.SingleOrDefault(
          d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
  
          if (descriptor != null)
          {
            services.Remove(descriptor);
          }
  
          string inMemoryCollectionName = Guid.NewGuid().ToString();
  
          services.AddDbContext<AppDbContext>(options =>
          {
            options.UseInMemoryDatabase(inMemoryCollectionName);
          });
        });
  }
}