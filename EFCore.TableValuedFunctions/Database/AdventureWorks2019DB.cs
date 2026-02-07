using EFCore.TableValuedFunctions.Models;
using Microsoft.EntityFrameworkCore;
using System;


namespace EFCore.TableValuedFunctions.Database
{
    public class AdventureWorks2019DB : DbContext
    {
        public AdventureWorks2019DB(DbContextOptions<AdventureWorks2019DB> dbContextOptions) : base(dbContextOptions) { }

        public AdventureWorks2019DB()
        {
        }

        public IQueryable<SalesOffer> ufn_GetSalesInformation(int specialOfferId) => FromExpression(() => ufn_GetSalesInformation(specialOfferId));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SalesOffer>().HasNoKey();

            modelBuilder.HasDbFunction(typeof(AdventureWorks2019DB).GetMethod(nameof(ufn_GetSalesInformation), new[] { typeof(int) })!)
                .HasName("ufn_GetSalesInformation")
                .HasSchema("dbo");

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.LogTo(message => Console.WriteLine(message));
                optionsBuilder.UseSqlServer(
                    @"Data Source=.;Initial Catalog=AdventureWorks2019;Integrated Security=SSPI;TrustServerCertificate=True");
            }
        }

    }
}
