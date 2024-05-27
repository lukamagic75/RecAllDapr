using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using RecAll.Infrastructure.Identity.Api.Models;

namespace RecAll.Infrastructure.Identity.Api.Services;

public class EFLoginService : ILoginService<ApplicationUser> {
    private UserManager<ApplicationUser> _userManager;
    private SignInManager<ApplicationUser> _signInManager;

    public EFLoginService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager) {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<bool> ValidateCredentialAsync(ApplicationUser user,
        string password) =>
        await _userManager.CheckPasswordAsync(user, password);


    public async Task<ApplicationUser> FindByUsernameAsync(string username) =>
        await _userManager.FindByNameAsync(username);

    public Task SignInAsync(ApplicationUser user,
        AuthenticationProperties properties,
        string authenticationMethod = null) =>
        _signInManager.SignInAsync(user, properties, authenticationMethod);
}