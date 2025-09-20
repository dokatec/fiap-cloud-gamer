using Fcg.Api;
using Fcg.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Linq;

namespace Fcg.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Encontra e remove a configuração original do DbContext
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<FcgDbContext>));

                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                // Adiciona um novo DbContext que usa um banco de dados em memória
                // Usamos um nome de banco de dados único para garantir o isolamento entre as classes de teste
                services.AddDbContext<FcgDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"FcgTestDb-{System.Guid.NewGuid()}");
                });
            });

            builder.UseEnvironment("Development");
        }
    }
}