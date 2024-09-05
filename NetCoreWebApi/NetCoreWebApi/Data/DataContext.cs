using Microsoft.EntityFrameworkCore;
using NetCoreWebApi.Models;

namespace NetCoreWebApi.Data

{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {
        }

        public DbSet<Cliente> Cliente {get; set; }
          

    }
}
