using Neo4j.Driver;

namespace BlockchainApi.Services
{
    public class Neo4jService : IAsyncDisposable
    {
        private readonly IDriver _driver;

        public Neo4jService(IConfiguration configuration)
        {
            var uri = configuration["Neo4j:Uri"];
            var user = configuration["Neo4j:User"];
            var password = configuration["Neo4j:Password"];

            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        public IAsyncSession GetSession()
        {
            return _driver.AsyncSession();
        }

        public async ValueTask DisposeAsync()
        {
            await _driver.CloseAsync();
        }
    }
}