using Microsoft.AspNetCore.Identity;
using RecAll.Infrastructure.Identity.Api.Models;

namespace RecAll.Infrastructure.Identity.Api.Data;

public class ApplicationDbContextSeed {
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher =
        new PasswordHasher<ApplicationUser>();

    public async Task SeedAsync(ApplicationDbContext context,
        int retry = 0) {
        var retryForAvailability = retry;
        try {
            if (!context.Users.Any()) {
                context.Users.AddRange(GetDefaultUser());
                await context.SaveChangesAsync();
            }
        } catch (Exception exception) {
            if (retryForAvailability < 10) {
                retryForAvailability++;
                await SeedAsync(context, retryForAvailability);
            }
        }
    }

    private IEnumerable<ApplicationUser> GetDefaultUser() {
        var user = new ApplicationUser {
            Email = "dev@recall.app",
            Id = Guid.NewGuid().ToString(),
            PhoneNumber = "13800138000",
            UserName = "dev@recall.app",
            NormalizedEmail = "DEV@RECALL.APP",
            NormalizedUserName = "DEV@RECALL.APP",
            SecurityStamp = Guid.NewGuid().ToString("D")
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, "Pass@word1");
        return new List<ApplicationUser> {
            user
        };
    }
}