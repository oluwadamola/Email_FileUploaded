using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace EmailUploadedDocument.Models
{
    public class DataEntities : DbContext
    {
        public DataEntities() : base("DataEntities")
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}