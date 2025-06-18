namespace BlockchainApi.Models
{
    public class CreateTransactionRequest
    {
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public double Amount { get; set; }
        public int Version { get; set; } = 1;
        public int TxSize { get; set; } = 0;
        public string? PrevBlockHash { get; set; } // Hash del bloque anterior (opcional)
    }
}
