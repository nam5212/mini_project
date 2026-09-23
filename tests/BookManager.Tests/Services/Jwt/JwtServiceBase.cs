using Microsoft.Extensions.Configuration;

namespace BookManager.Tests.Services.Jwt;

public abstract class JwtServiceBase
{
    protected const string TestSecret = "this_is_a_very_long_secure_secret_key_for_unit_tests_123456";
    protected const string TestIssuer = "BookManagerTestIssuer";
    protected const string TestAudience = "BookManagerTestAudience";

    protected IConfiguration CreateConfiguration(Dictionary<string, string?>? inMemorySettings = null)
    {
        var settings = inMemorySettings ?? new Dictionary<string, string?>
        {
            { "JWT_SECRET", TestSecret },
            { "JWT_ISSUER", TestIssuer },
            { "JWT_AUDIENCE", TestAudience },
            { "JWT_EXPIRES_MINUTES", "30" },
            { "JWT_REFRESH_EXPIRES_DAYS", "14" }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }
}
