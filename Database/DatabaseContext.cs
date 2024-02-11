using ConsoleAppListOfProducts.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppListOfProducts.Database
{
    public class DatabaseContext :DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=HOME-PC; Database=ZemerovaListOfProducts; Trusted_Connection=True; MultipleActiveResultSets=true; TrustServerCertificate=True;");
        }
    }
}