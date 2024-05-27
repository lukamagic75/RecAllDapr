using System.Net;
using System.Text.RegularExpressions;

namespace RecAll.Infrastructure.Identity.Api.Services;

public class RedirectService : IRedirectService {
    public string ExtractRedirectUriFromReturnUrl(string url) {
        var decodedUrl = WebUtility.HtmlDecode(url);
        var splits = Regex.Split(decodedUrl, "redirect_uri=");
        if (splits.Length < 2) {
            return "";
        }

        var redirectUri = splits[1];

        var splitKey = redirectUri.Contains("signin-oidc")
            ? "signin-oidc"
            : "scope";

        splits = Regex.Split(redirectUri, splitKey);
        if (splits.Length < 2)
            return "";

        redirectUri = splits[0];

        return redirectUri.Replace("%3A", ":").Replace("%2F", "/")
            .Replace("&", "");
    }
}