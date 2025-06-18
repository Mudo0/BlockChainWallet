using System.Text.Json;
using BlockchainWallet.DataAccess;

namespace BlockchainWallet.Repositories
{
    public class BlockRepository
    {
        private IDataAccess _neo4jAccess;
        private readonly ILogger<BlockRepository> _logger;

        public BlockRepository(IDataAccess dataAccess, ILogger<BlockRepository> logger)
        {
            _neo4jAccess = dataAccess;
            _logger = logger;
        }

        public async Task<List<Dictionary<string, object>>> GetBlockByHashAsync(string blockHash)
        {
            _logger.LogInformation("Retrieving block with hash: {BlockHash}", blockHash);
            string query = @"MATCH (b :block)-[:inc]->(tx :tx)
                            WHERE b.hash='$blockHash'
                            RETURN b {blockhash : b.hash, size : b.size}";
            var parameters = new Dictionary<string, object>
            {
                { "blockHash", blockHash }
            };

            _logger.LogDebug("Sending query: {query}", query);
            _logger.LogDebug("With parameters: {parameters}", JsonSerializer.Serialize(parameters));

            var result = await _neo4jAccess.ExecuteReadDictionaryAsync(query, "b", parameters);

            return result;
        }
    }
}