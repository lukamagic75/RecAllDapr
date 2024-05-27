namespace RecAll.Infrastructure.Identity.Api.Services;

public interface IRedirectService {
    string ExtractRedirectUriFromReturnUrl(string url);
}