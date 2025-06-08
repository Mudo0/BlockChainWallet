using System;

namespace BlockchainWallet.Repositories
{
    public class RepositorioTest : IRepository
    {
        private readonly IDataAccess _neo4JAccess;
        private readonly ILogger<RepositorioTest> _logger;

        public RepositorioTest(IDataAccess dataAccess, ILogger<RepositorioTest> logger)
        {
            _neo4JAccess = dataAccess;
            _logger = logger;
        }

        public async Task<bool> AddPerson(string name, int age)
        {
            _logger.LogInformation("Adding person with name: {Name} and age: {Age}", name, age);
            if (!string.IsNullOrWhiteSpace(name))
            {
                var query = @"MERGE (p:Person {name: $name}) ON CREATE SET p.age = $age
                            ON MATCH SET p.age = $age, p.updatedAt = timestamp() RETURN true";
                IDictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "name", name},
                    { "age", age}
                };
                return await _neo4JAccess.ExecuteWriteTransactionAsync<bool>(query, parameters);
            }
            else
            {
                _logger.LogError("Error adding a person");
                throw new System.ArgumentNullException(name, "Person must not be null");
            }
        }

        public async Task<List<Dictionary<string, object>>> SearchPersonsByName(string searchString)
        {
            _logger.LogInformation("Searching persons with name containing: {SearchString}", searchString);
            var query = @"MATCH (p:Person) WHERE toUpper(p.name) CONTAINS toUpper($searchString)
                                RETURN p{ name: p.name, age: p.age } ORDER BY p.Name LIMIT 5";

            IDictionary<string, object> parameters = new Dictionary<string, object> { { "searchString", searchString } };

            var persons = await _neo4JAccess.ExecuteReadDictionaryAsync(query, "p", parameters);

            return persons;
        }
    }

    public interface IRepository
    {
        Task<bool> AddPerson(string name, int age);
        Task<List<Dictionary<string, object>>> SearchPersonsByName(string searchString);
    }
}