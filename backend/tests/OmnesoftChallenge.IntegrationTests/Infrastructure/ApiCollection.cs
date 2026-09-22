namespace OmnesoftChallenge.IntegrationTests.Infrastructure;

[CollectionDefinition(Name)]
public class ApiCollection : ICollectionFixture<OmnesoftChallengeApiFactory>
{
    public const string Name = "OmnesoftChallenge API";
}
