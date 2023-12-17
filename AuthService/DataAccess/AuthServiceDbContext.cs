using AuthService.DataAccess.Objects;
using Microsoft.EntityFrameworkCore;

namespace AuthService.DataAccess;

public class AuthServiceDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public AuthServiceDbContext() { }

    public AuthServiceDbContext(DbContextOptions<AuthServiceDbContext> options) : base(options)
    {
    }
}