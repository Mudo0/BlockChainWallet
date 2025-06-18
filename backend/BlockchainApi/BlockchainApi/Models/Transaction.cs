namespace BlockchainApi.Models
{
    public class Transaction
    {
        public string Txid { get; set; }
        public int Version { get; set; }
        public int TxSize { get; set; }
        public List<Coinbase> Coinbases { get; set; }
    }
}
