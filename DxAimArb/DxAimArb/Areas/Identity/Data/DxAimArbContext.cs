using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DxAimArb.Data;

public class DxAimArbContext : IdentityDbContext<ApplicationUser>
{
    public DxAimArbContext(DbContextOptions<DxAimArbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>()
            .Property(e => e.firstName)
            .HasMaxLength(250);

        modelBuilder.Entity<ApplicationUser>()
            .Property(e => e.lastName)
            .HasMaxLength(250);

    }

}
