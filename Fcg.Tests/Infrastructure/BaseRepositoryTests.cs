using Fcg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace Fcg.Infrastructure.Tests
{
    public class BaseRepositoryTests : IDisposable
    {
        protected readonly FcgDbContext _context;

        public BaseRepositoryTests()
        {
            // Usamos um nome de banco de dados único para cada instância de teste,
            // garantindo que os testes sejam isolados uns dos outros.
            var options = new DbContextOptionsBuilder<FcgDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new FcgDbContext(options);
            _context.Database.EnsureCreated(); // Garante que o banco de dados em memória seja criado
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted(); // Limpa o banco de dados após cada teste
            _context.Dispose();
        }
    }
}