using System.Text;
using System.Text.Json;

namespace BlockchainApi.Examples
{
    public class TransactionExample
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public TransactionExample(string baseUrl = "https://localhost:7001")
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Ejemplo de cómo crear una nueva transacción con un nuevo bloque
        /// </summary>
        public async Task<string> CreateNewTransactionAndBlock()
        {
            var request = new
            {
                fromAddress = "1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa",
                toAddress = "3J98t1WpEZ73CNmQviecrnyiWrnqRhWNLy",
                amount = 0.001,
                version = 1,
                txSize = 250,
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                $"{_baseUrl}/api/block/create-transaction",
                content
            );

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Transacción creada exitosamente: {responseContent}");
                return responseContent;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error al crear transacción: {errorContent}");
                return null;
            }
        }

        /// <summary>
        /// Ejemplo de cómo agregar una transacción a un bloque existente
        /// </summary>
        public async Task<string> AddTransactionToExistingBlock(string blockHash)
        {
            var request = new
            {
                fromAddress = "1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa",
                toAddress = "bc1qxy2kgdygjrsqtzq2n0yrf2493p83kkfjhx0wlh",
                amount = 0.002,
                version = 1,
                txSize = 280,
                prevBlockHash = blockHash,
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                $"{_baseUrl}/api/block/add-transaction-to-block",
                content
            );

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(
                    $"Transacción agregada al bloque exitosamente: {responseContent}"
                );
                return responseContent;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error al agregar transacción: {errorContent}");
                return null;
            }
        }

        /// <summary>
        /// Ejemplo de cómo obtener todas las transacciones de un bloque
        /// </summary>
        public async Task<string> GetTransactionsByBlock(string blockHash)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/block/by-block/{blockHash}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Transacciones del bloque {blockHash}: {responseContent}");
                return responseContent;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error al obtener transacciones: {errorContent}");
                return null;
            }
        }

        /// <summary>
        /// Ejemplo de cómo obtener estadísticas de transacciones por bloque
        /// </summary>
        public async Task<string> GetTransactionStats()
        {
            var response = await _httpClient.GetAsync(
                $"{_baseUrl}/api/block/stats/transactions-per-block"
            );

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Estadísticas de transacciones: {responseContent}");
                return responseContent;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error al obtener estadísticas: {errorContent}");
                return null;
            }
        }

        /// <summary>
        /// Ejemplo completo de flujo de trabajo
        /// </summary>
        public async Task RunCompleteExample()
        {
            Console.WriteLine("=== Ejemplo de Creación de Transacciones y Bloques ===\n");

            // 1. Crear una nueva transacción con nuevo bloque
            Console.WriteLine("1. Creando nueva transacción con nuevo bloque...");
            var result1 = await CreateNewTransactionAndBlock();

            if (result1 != null)
            {
                // Extraer el hash del bloque de la respuesta (esto requeriría parsing del JSON)
                // Por simplicidad, usamos un hash de ejemplo
                var exampleBlockHash =
                    "000000000019d6689c085ae165831e934ff763ae46a2a6c172b3f1b60a8ce26f";

                // 2. Agregar otra transacción al bloque existente
                Console.WriteLine("\n2. Agregando transacción a bloque existente...");
                await AddTransactionToExistingBlock(exampleBlockHash);

                // 3. Obtener transacciones del bloque
                Console.WriteLine("\n3. Obteniendo transacciones del bloque...");
                await GetTransactionsByBlock(exampleBlockHash);

                // 4. Obtener estadísticas
                Console.WriteLine("\n4. Obteniendo estadísticas...");
                await GetTransactionStats();
            }

            Console.WriteLine("\n=== Ejemplo completado ===");
        }
    }
}
