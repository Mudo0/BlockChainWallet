namespace BlockchainApi.Models
{
    public class Coinbase
    {
        public string Cbid { get; set; }
        public List<string> Addresses { get; set; }
        public double Value { get; set; }
    }
}
