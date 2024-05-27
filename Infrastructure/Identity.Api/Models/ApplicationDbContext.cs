using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RecAll.Infrastructure.Identity.Api.Models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
}

public class
    ListContextDesignFactory : IDesignTimeDbContextFactory<
    ApplicationDbContext> {
    public ApplicationDbContext CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(
                "Server=.;Initial Catalog=RecAll.IdentityDb;Integrated Security=true")
            .Options);
}