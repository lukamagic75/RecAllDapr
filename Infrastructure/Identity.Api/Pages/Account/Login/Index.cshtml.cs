// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecAll.Infrastructure.Identity.Api.Models;
using RecAll.Infrastructure.Identity.Api.Services;

namespace Identity.Api.Pages.Login;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel {
    // private readonly TestUserStore _users;
    // private readonly IIdentityServerInteractionService _interaction;
    // private readonly IEventService _events;
    // private readonly IAuthenticationSchemeProvider _schemeProvider;
    // private readonly IIdentityProviderStore _identityProviderStore;

    private readonly IIdentityServerInteractionService _interaction;
    private readonly IClientStore _clientStore;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly ILoginService<ApplicationUser> _loginService;
    private readonly ILogger<Index> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public ViewModel View { get; set; } = default!;

    [BindProperty] public InputModel Input { get; set; } = default!;

    public Index(ILoginService<ApplicationUser> loginService,
        IIdentityServerInteractionService interaction, IClientStore clientStore,
        IAuthenticationSchemeProvider schemeProvider, ILogger<Index> logger,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration) {
        _loginService = loginService;
        _interaction = interaction;
        _clientStore = clientStore;
        _schemeProvider = schemeProvider;
        _logger = logger;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<IActionResult> OnGet(string? returnUrl) {
        await BuildModelAsync(returnUrl);

        if (View.IsExternalLoginOnly) {
            // we only have one option for logging in and it's an external provider
            return RedirectToPage("/ExternalLogin/Challenge", new {
                scheme = View.ExternalLoginScheme, returnUrl
            });
        }

        return Page();
    }

    public async Task<IActionResult> OnPost() {
        var context =
            await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);

        if (Input.Button != "login") {
            if (context == null) {
                return Redirect("~/");
            }

            await _interaction.DenyAuthorizationAsync(context,
                AuthorizationError.AccessDenied);

            if (context.IsNativeClient()) {
                return this.LoadingPage(Input.ReturnUrl);
            }

            return Redirect(Input.ReturnUrl);
        }

        if (ModelState.IsValid) {
            var user = await _loginService.FindByUsernameAsync(Input.Username);

            if (await _loginService.ValidateCredentialAsync(user,
                    Input.Password)) {
                _logger.LogInformation(
                    $"UserLoginSuccessEvent: {user.UserName}, {context?.Client.ClientId}");

                var tokenLifetime =
                    _configuration.GetValue("TokenLifetimeMinutes", 120);
                var props = new AuthenticationProperties {
                    ExpiresUtc =
                        DateTimeOffset.UtcNow.AddMinutes(tokenLifetime),
                    AllowRefresh = true,
                    RedirectUri = Input.ReturnUrl
                };
                if (LoginOptions.AllowRememberLogin && Input.RememberLogin) {
                    var permanentTokenLifetime =
                        _configuration.GetValue("PermanentTokenLifetimeDays",
                            365);
                    props.ExpiresUtc =
                        DateTimeOffset.UtcNow.AddDays(permanentTokenLifetime);
                    props.IsPersistent = true;
                }

                await _loginService.SignInAsync(user, props);

                if (context != null) {
                    if (context.IsNativeClient()) {
                        return this.LoadingPage(Input.ReturnUrl);
                    }

                    return Redirect(Input.ReturnUrl);
                }

                if (Url.IsLocalUrl(Input.ReturnUrl)) {
                    return Redirect(Input.ReturnUrl);
                }

                if (string.IsNullOrEmpty(Input.ReturnUrl)) {
                    return Redirect("~/");
                }

                throw new Exception("invalid return URL");
            }

            _logger.LogInformation(
                $"UserLoginFailureEvent: {Input.Username}, invalid credentials, {context?.Client.ClientId}");
            ModelState.AddModelError(string.Empty,
                LoginOptions.InvalidCredentialsErrorMessage);
        }

        await BuildModelAsync(Input.ReturnUrl);
        return Page();
    }

    private async Task BuildModelAsync(string returnUrl) {
        Input = new InputModel {
            ReturnUrl = returnUrl
        };

        var context =
            await _interaction.GetAuthorizationContextAsync(returnUrl);
        if (context?.IdP != null &&
            await _schemeProvider.GetSchemeAsync(context.IdP) != null) {
            var local = context.IdP ==
                IdentityServerConstants.LocalIdentityProvider;

            // this is meant to short circuit the UI and only trigger the one external IdP
            View = new ViewModel {
                EnableLocalLogin = local,
            };

            Input.Username = context?.LoginHint;

            if (!local) {
                View.ExternalProviders = new[] {
                    new ViewModel.ExternalProvider(context.IdP)
                };
            }

            return;
        }

        var schemes = await _schemeProvider.GetAllSchemesAsync();

        var providers = schemes.Where(x => x.DisplayName != null).Select(x =>
            new ViewModel.ExternalProvider(x.Name, x.DisplayName)).ToList();

        var allowLocal = true;
        var client = context?.Client;
        if (client != null) {
            allowLocal = client.EnableLocalLogin;
            if (client.IdentityProviderRestrictions != null &&
                client.IdentityProviderRestrictions.Any()) {
                providers = providers.Where(provider =>
                    client.IdentityProviderRestrictions.Contains(
                        provider.AuthenticationScheme)).ToList();
            }
        }

        View = new ViewModel {
            AllowRememberLogin = LoginOptions.AllowRememberLogin,
            EnableLocalLogin = allowLocal && LoginOptions.AllowLocalLogin,
            ExternalProviders = providers.ToArray()
        };
    }
}