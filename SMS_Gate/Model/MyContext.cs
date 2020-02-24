using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS_Gate.Model
{
    public class MyContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }

        public MyContext() { }
        public MyContext(DbContextOptions<MyContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=DataBase.db");

        #region Required
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                .Property(b => b.phone_num)
                .IsRequired();
        }
        #endregion
    }
}
