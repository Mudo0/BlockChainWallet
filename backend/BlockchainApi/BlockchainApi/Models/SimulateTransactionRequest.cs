namespace BlockchainApi.Models
{
    public class SimulateTransactionRequest
    {
        public string BlockHash { get; set; }
        public int TxSize { get; set; } 
        public string Address { get; set; }
        public double Value { get; set; }
    }


}
