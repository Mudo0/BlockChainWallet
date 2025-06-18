using Microsoft.AspNetCore.Mvc;
using BlockchainApi.Services;
using Neo4j.Driver;
using BlockchainApi.Models;

namespace BlockchainApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlockController : ControllerBase
    {
        private readonly Neo4jService _neo4j;

        public BlockController(Neo4jService neo4j)
        {
            _neo4j = neo4j;
        }

        [HttpGet("all")] //Trae todos los bloques hash, size y timestamp
        public async Task<IActionResult> GetAllBlocks()
        {
            var session = _neo4j.GetSession();
            var query = "MATCH (b:block) RETURN b.hash AS hash, b.size AS size, b.timestamp AS timestamp";

            var result = await session.RunAsync(query);

            var blocks = new List<object>();

            await result.ForEachAsync(record =>
            {
                blocks.Add(new
                {
                    Hash = record["hash"].As<string>(),
                    Size = record["size"].As<int>(),
                    Timestamp = record["timestamp"].As<long>()
                });
            });

            await session.CloseAsync();

            return Ok(blocks);
        }

        [HttpGet("{hash}")]//Trae los bloques por hash
        public async Task<IActionResult> GetBlockByHash(string hash)
        {
            var session = _neo4j.GetSession();
            var query = @"
                 MATCH (b:block {hash: $hash})
                 RETURN b.hash AS hash, b.size AS size, b.timestamp AS timestamp, b.prevblock AS prevblock
                 ";

            var result = await session.RunAsync(query, new { hash });

            if (await result.FetchAsync())
            {
                var record = result.Current;
                var block = new
                {
                    Hash = record["hash"].As<string>(),
                    Size = record["size"].As<int>(),
                    Timestamp = record["timestamp"].As<long>(),
                    PrevBlock = record.Keys.Contains("prevblock") ? record["prevblock"].As<string>() : null
                };

                await session.CloseAsync();
                return Ok(block);
            }
            else
            {
                await session.CloseAsync();
                return NotFound();
            }
        }

        //simula una transaccion
        [HttpPost("simulate")]
        public async Task<IActionResult> SimulateTransaction([FromBody] SimulateTransactionRequest request)
        {
            var session = _neo4j.GetSession();

            // Crear un txid nuevo (uuid) para la transacción
            var txId = Guid.NewGuid().ToString();

            var query = @"
                MATCH (b:block {hash: $blockHash})
                CREATE (t:tx {txid: $txId, version: 1, locktime: 0, size: $txSize, weight: 0, segwit: false})
                CREATE (c:coinbase {index: 0, value: $value, addresses: [$address]})
                CREATE (b)-[:inc]->(t)
                CREATE (t)-[:coinbase]->(c)
                RETURN t.txid AS txId
                ";

            try
            {
                var result = await session.RunAsync(query, new
                {
                    blockHash = request.BlockHash,
                    txId = txId,
                    txSize = request.TxSize,  // suponiendo que tu request tiene este campo
                    address = request.Address,
                    value = request.Value
                });

                var hasRecord = await result.FetchAsync();

                if (!hasRecord)
                {
                    return NotFound($"Block with hash {request.BlockHash} not found.");
                }

                var record = result.Current;
                var txIdFromDb = record["txId"].As<string>();

                return Ok(new { Message = "Transaction simulated", TxId = txIdFromDb });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        //muestra las transacciones por bloque, de este modo savemos la actividad en la red,se puede hacer un grafico con esto
        [HttpGet("stats/transactions-per-block")]
        public async Task<IActionResult> GetTransactionCountsPerBlock()
        {
            var session = _neo4j.GetSession();
            var query = @"
                 MATCH (b:block)
                 OPTIONAL MATCH (b)-[:inc]->(t:tx)
                 RETURN b.hash AS blockHash, count(t) AS txCount
                 ";

            try
            {
                var result = await session.RunAsync(query);

                var stats = new List<object>();
                await result.ForEachAsync(record =>
                {
                    stats.Add(new
                    {
                        BlockHash = record["blockHash"].As<string>(),
                        TxCount = (int)record["txCount"].As<long>()
                    });
                });

                return Ok(stats);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        [HttpGet("by-block/{blockHash}")]
        public async Task<IActionResult> GetTransactionsByBlock(string blockHash)
        {
            var session = _neo4j.GetSession();
            var query = @"
                 MATCH (b:block {hash: $blockHash})-[:inc]->(t:tx)
                 OPTIONAL MATCH (t)-[:out]->(c:coinbase)
                 RETURN t, c
                 ";

            var result = await session.RunAsync(query, new { blockHash });

            var txList = new List<object>();

            await result.ForEachAsync(record =>
            {
                var t = record["t"].As<INode>();
                var c = record["c"]?.As<INode>();

                // Leer propiedades txid y version por ejemplo
                var txid = t.Properties.ContainsKey("txid") ? t.Properties["txid"].As<string>() : null;
                var version = t.Properties.ContainsKey("version") ? (int?)t.Properties["version"].As<int>() : null;

                // Para coinbase extraer addresses con seguridad
                List<string> addresses = new();
                if (c != null && c.Properties.ContainsKey("addresses"))
                {
                    var rawAddresses = c.Properties["addresses"];
                    if (rawAddresses is List<object> objList)
                    {
                        addresses = objList.Select(a => a.ToString()).ToList();
                    }
                    else if (rawAddresses is List<string> strList)
                    {
                        addresses = strList;
                    }
                }

                var coinbase = c != null ? new
                {
                    Addresses = addresses,
                    Value = c.Properties.ContainsKey("value") ? Convert.ToDouble(c.Properties["value"]) : 0
                } : null;

                txList.Add(new
                {
                    TxId = txid,
                    Version = version,
                    Coinbase = coinbase
                });
            });

            await session.CloseAsync();

            return Ok(txList);
        }
    }
}