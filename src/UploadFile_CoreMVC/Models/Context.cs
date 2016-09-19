using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UploadFile_CoreMVC.Models
{
    public class UploadDbContext : DbContext
    {
        public UploadDbContext(DbContextOptions<UploadDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<UserFiles> UserFiles { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Upload_Core;Trusted_Connection=True;");
        //}
    }
}