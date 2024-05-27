using Microsoft.AspNetCore.Authentication;

namespace RecAll.Infrastructure.Identity.Api.Services;

public interface ILoginService<T> {
    Task<bool> ValidateCredentialAsync(T user, string password);

    Task<T> FindByUsernameAsync(string username);

    Task SignInAsync(T user, AuthenticationProperties properties,
        string authenticationMethod = null);
}