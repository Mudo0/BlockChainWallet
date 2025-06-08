using BlockchainWallet.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BlockchainWallet.Controllers
{
    public class TestController : Controller
    {
        private readonly Neo4jSettings _neo4jSettings;

        public TestController(IOptions<Neo4jSettings> neo4JOptions)
        {
            _neo4jSettings = neo4JOptions.Value;
        }

        [HttpGet("/test")]
        public IActionResult Index()
        {
            // Aquí puedes usar _neo4jSettings para acceder a la configuración de Neo4j
            return Ok(new
            {
                Connection = _neo4jSettings.Neo4jConnection,
                User = _neo4jSettings.Neo4jUser,
                Password = _neo4jSettings.Neo4jPassword,
                Database = _neo4jSettings.Neo4jDatabase
            });
        }
    }
}