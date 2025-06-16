using Microsoft.EntityFrameworkCore;

namespace TextileDefectTracker
{
    public class TextileDefectContext : System.Data.Entity.DbContext
    {
        private string connectionString;

        public System.Data.Entity.DbSet<Fabric> Fabrics { get; set; }
        public System.Data.Entity.DbSet<Defect> Defects { get; set; }
        public System.Data.Entity.DbSet<User> Users { get; set; }
        public System.Data.Entity.DbSet<Inspection> Inspections { get; set; }
         protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Replace with your actual connection string
        optionsBuilder.UseSqlServer("Data Source=DESKTOP-154HLCL\SQLEXPRESS;Initial Catalog=TextileDefectTracker;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;");
    }
}
