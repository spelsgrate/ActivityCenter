using Microsoft.EntityFrameworkCore;

namespace BeltExam.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> UserList { get; set; }
        public DbSet<Occasion> ActList { get; set; }
        public DbSet<Join> Joinee { get; set; }
    }
}