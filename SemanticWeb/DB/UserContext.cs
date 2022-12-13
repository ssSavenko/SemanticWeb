using Microsoft.EntityFrameworkCore;

namespace SemanticWeb.DB
{
    public class UserContext : DbContext
    {  
        public DbSet<User> Users { get; set; }
        
        public UserContext()
        {

        }

        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        } 
    }
}
