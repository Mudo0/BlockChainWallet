using System.Transactions;

namespace BlockchainApi.Models
{
    public class Block
    {
        public string Hash { get; set; }
        public int Size { get; set; }
        public long Timestamp { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
