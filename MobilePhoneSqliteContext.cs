using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace MobileShop
{
    public class MobilePhoneSqliteContext : DbContext
    {
        public DbSet<MobilePhone> MobilePhones { get; set; } = null!;
        public SqliteConnection Connection { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            optionsBuilder.UseSqlite(connectionString);

            Connection = new SqliteConnection(connectionString);
            Connection.Open();
        }
    }
}
