using Microsoft.EntityFrameworkCore;

namespace ConsultantMicroservice
{
    public class ConsultantDBContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public ConsultantDBContext(DbContextOptions<ConsultantDBContext> dbContextOptions, IConfiguration configuration) : base(dbContextOptions)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            //options.UseSqlServer(_configuration.GetConnectionString("WebApiDatabase"));
            //options.UseSqlServer("Server=localhost\\MSSQLLocalDB;Database=ConsultantDB;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true");
            options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }
        public DbSet<Shared.ConsultantModel> Consultant { get; set; }

    }
}
