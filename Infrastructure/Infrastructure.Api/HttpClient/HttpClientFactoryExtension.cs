namespace Infrastructure.Api.HttpClient;

public static class HttpClientFactoryExtension {
    public const string DefaultClient = nameof(DefaultClient);

    public static System.Net.Http.HttpClient CreateDefaultClient(
        this IHttpClientFactory httpClientFactory) =>
        httpClientFactory.CreateClient(DefaultClient);
}