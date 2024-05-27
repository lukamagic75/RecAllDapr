using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using RecAll.Infrastructure.Identity.Api.Models;

namespace RecAll.Infrastructure.Identity.Api.Services;

public class ProfileService : IProfileService {
    private UserManager<ApplicationUser> _userManager;

    public ProfileService(UserManager<ApplicationUser> userManager) {
        _userManager = userManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context) {
        var subject = context.Subject ??
            throw new ArgumentNullException(nameof(context.Subject));
        var subjectId =
            subject.Claims.FirstOrDefault(p => p.Type == "sub")?.Value ??
            throw new ArgumentException("Invalid subject identifier");
        var user = await _userManager.FindByIdAsync(subjectId) ??
            throw new ArgumentException("Invalid subject identifier");
        context.IssuedClaims = GetClaimListFromUser(user);
    }

    public async Task IsActiveAsync(IsActiveContext context) {
        var subject = context.Subject ??
            throw new ArgumentNullException(nameof(context.Subject));
        var subjectId =
            subject.Claims.FirstOrDefault(p => p.Type == "sub")?.Value ??
            throw new ArgumentException("Invalid subject identifier");
        var user = await _userManager.FindByIdAsync(subjectId) ??
            throw new ArgumentException("Invalid subject identifier");

        context.IsActive = false;

        if (_userManager.SupportsUserSecurityStamp) {
            var securityStamp = subject.Claims
                .Where(p => p.Type == "security_stamp").Select(p => p.Value)
                .SingleOrDefault();
            if (securityStamp != null) {
                var dbSecurityStamp =
                    await _userManager.GetSecurityStampAsync(user);
                if (dbSecurityStamp != securityStamp) {
                    return;
                }
            }
        }

        context.IsActive = !user.LockoutEnabled || !user.LockoutEnd.HasValue ||
            user.LockoutEnd <= DateTime.Now;
    }

    private List<Claim> GetClaimListFromUser(ApplicationUser user) {
        var claimList = new List<Claim> {
            new(JwtClaimTypes.Subject, user.Id),
            new(JwtClaimTypes.PreferredUserName, user.UserName),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName)
        };

        if (!string.IsNullOrWhiteSpace(user.UserName)) {
            claimList.Add(new Claim("user_name", user.Email));
        }

        if (_userManager.SupportsUserEmail) {
            claimList.AddRange(new Claim[] {
                new(JwtClaimTypes.Email, user.Email),
                new(JwtClaimTypes.EmailVerified,
                    user.EmailConfirmed ? "true" : "false",
                    ClaimValueTypes.Boolean)
            });
        }

        if (_userManager.SupportsUserPhoneNumber &&
            !string.IsNullOrWhiteSpace(user.PhoneNumber)) {
            claimList.AddRange(new[] {
                new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber),
                new Claim(JwtClaimTypes.PhoneNumberVerified,
                    user.PhoneNumberConfirmed ? "true" : "false",
                    ClaimValueTypes.Boolean)
            });
        }

        return claimList;
    }
}