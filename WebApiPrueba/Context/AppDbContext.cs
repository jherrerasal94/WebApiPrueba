using Microsoft.EntityFrameworkCore;
using WebApiPrueba.Models;

namespace WebApiPrueba.Context
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }

        public DbSet<ClientesPs> ClientesPs { get; set; }

    }
}
