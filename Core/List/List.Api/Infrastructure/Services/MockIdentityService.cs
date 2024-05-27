namespace RecAll.Core.List.Api.Infrastructure.Services;

public class MockIdentityService : IIdentityService {
    public string GetUserIdentityGuid() {
        return "04be6c0f-205b-487e-89ad-8aebb11b32f1";
    }
}