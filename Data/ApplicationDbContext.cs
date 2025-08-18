using Microsoft.EntityFrameworkCore;
using Models.Domain;

namespace Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Resource> Resources { get; set; }
        public DbSet<UnitOfMeasurement> UnitsOfMeasurement { get; set; }
        public DbSet<IncomingDocument> IncomingDocuments { get; set; }
        public DbSet<IncomingResource> IncomingResources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Resource>()
                .HasIndex(r => r.Name)
                .IsUnique();

            modelBuilder.Entity<UnitOfMeasurement>()
                .HasIndex(u => u.Name)
                .IsUnique();

            modelBuilder.Entity<IncomingDocument>()
                .HasIndex(d => d.Number)
                .IsUnique();
            
            modelBuilder.Entity<IncomingResource>()
                .HasOne(ir => ir.IncomingDocument)
                .WithMany(d => d.IncomingResources)
                .HasForeignKey(ir => ir.IncomingDocumentId)
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}