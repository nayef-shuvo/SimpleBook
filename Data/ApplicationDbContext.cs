using Microsoft.EntityFrameworkCore;
using SimpleBook.Entities;

namespace SimpleBook.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; } 
    public DbSet<UserRole> UserRoles { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
}
