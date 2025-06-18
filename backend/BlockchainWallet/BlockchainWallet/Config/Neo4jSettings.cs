using System.ComponentModel.DataAnnotations;

namespace BlockchainWallet.Config
{
    /// <summary>
    /// This class is for storing the settings in appsettings.json
    /// for connecting to a Neo4j database.
    /// </summary>
    public class Neo4jSettings
    {
        public Uri Neo4jConnection { get; set; }
        public string Neo4jUser { get; set; }
        public string Neo4jPassword { get; set; }
        public string Neo4jDatabase { get; set; }
    }
}