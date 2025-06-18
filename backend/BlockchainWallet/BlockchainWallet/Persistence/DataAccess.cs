using BlockchainWallet.Config;
using BlockchainWallet.DataAccess;
using Microsoft.Extensions.Options;
using Neo4j.Driver;

namespace BlockchainWallet.Persistence
{
    /// <summary>
    /// This class manages the sessions and transactions with the Neo4j database.
    /// </summary>
    public class DataAccess : IDataAccess
    {
        private IAsyncSession _session;
        private ILogger<DataAccess> _logger;
        private string _database;

        public DataAccess(IDriver driver, ILogger<DataAccess> logger,
            IOptions<Neo4jSettings> appsettingsOptions)
        {
            _logger = logger;
            _logger.LogInformation("Opening Database Session");
            _database = appsettingsOptions.Value.Neo4jDatabase ?? "neo4j";
            _session = driver.AsyncSession(o => o.WithDatabase(_database));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources asynchronously.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            _logger.LogInformation("Closing Database Session");
            await _session.CloseAsync();
        }

        /// <summary>
        /// Execute read list as an asynchronous operation.
        /// </summary>
        public async Task<List<string>> ExecuteReadListAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters = null)
        {
            //Su único trabajo es llamar al método privado ExecuteReadTransactionAsync
            //con un tipo genérico ya definido.
            //Esto simplifica las llamadas desde el repositorio cuando sabes que quieres una
            //lista de cadenas o una lista de diccionarios.
            return await ExecuteReadTransactionAsync<string>(query, returnObjectKey, parameters);
        }

        /// <summary>
        /// Execute read dictionary as an asynchronous operation.
        /// </summary>

        public async Task<List<Dictionary<string, object>>> ExecuteReadDictionaryAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters = null)
        {
            //atajo para llamar al método privado ExecuteReadTransactionAsync
            return await ExecuteReadTransactionAsync<Dictionary<string, object>>
                (query, returnObjectKey, parameters);
        }

        /// <summary>
        /// Execute read scalar as an asynchronous operation.
        /// </summary>
        public async Task<T> ExecuteReadScalarAsync<T>(string query, IDictionary<string, object>? parameters = null)
        {
            //Ejecuta una consulta que devuelve un único valor (un escalar).
            // Para consultas de agregación como COUNT, SUM
            // para obtener una sola propiedad de un único nodo.
            try
            {
                //1. validacion de que los parámetros no sean nulos
                parameters = parameters == null ?
                    new Dictionary<string, object>() : //si son nulos crea un diccionario vacio
                    parameters; //si no, utiliza los parametros del método

                //2. ejecución de la consulta
                // se llama a la sesion creada por el driver para realizar una lectura/escritura
                // el driver maneja automaticamente la creacion, el commit o el rollback de las transacciones
                _logger.LogInformation("Executing ReadAsync");
                var result = await _session.ExecuteReadAsync(
                    async tx => // async tx => representa la sesion activa
                {//3.Lógica que se ejecuta dentro de la transacción
                    // crea la variable scalar para almacenar el resultado
                    //del tipo T especificado por el método y la inicia
                    T scalar = default;

                    //4. se ejecuta la query Cypher
                    //los parameters son para evitar inyeccion
                    //devuelve un IResultCursor que contiene los resultados
                    _logger.LogInformation("Executing Transaction");
                    var res = await tx.RunAsync(query, parameters);

                    //5. se obtiene el primer resultado de la consulta
                    //SingleAsync devuelve un único elemento de la secuencia
                    //si devuelve 0 o mas de 1 elementos lanza una excepción
                    //[0] devuelve la primera columna del resultado
                    scalar = (await res.SingleAsync())[0].As<T>();//lo convierte al tipo asignado por método
                    return scalar;
                });
                return result;
            }
            catch (Exception e)
            {
                //guardamos el error en logs
                _logger.LogError(e, "There was a problem executing database query.");
                throw;
            }
        }

        /// <summary>
        /// Execute write transaction
        /// </summary>
        public async Task<T> ExecuteWriteTransactionAsync<T>(string query, IDictionary<string, object>? parameters = null)
        {
            try
            {
                parameters = parameters == null ? new Dictionary<string, object>() : parameters;
                _logger.LogInformation("Executing WriteAsync");
                var result = await _session.ExecuteWriteAsync(async tx =>
                {
                    T scalar = default;

                    _logger.LogInformation("Executing Transaction");
                    var res = await tx.RunAsync(query, parameters);

                    _logger.LogInformation("Fetching records");
                    scalar = (await res.SingleAsync())[0].As<T>();
                    return scalar;
                });
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "There was a problem executing database query.");
                throw;
            }
        }

        /// <summary>
        /// Execute read transaction as an asynchronous operation.
        /// </summary>

        private async Task<List<T>> ExecuteReadTransactionAsync<T>(string query, string returnObjectKey, IDictionary<string, object>? parameters = null)
        {
            try
            {
                parameters = parameters == null ? new Dictionary<string, object>() : parameters;
                _logger.LogInformation("Executing ReadAsync");
                var result = await _session.ExecuteReadAsync(async tx =>
                {
                    var data = new List<T>();
                    _logger.LogInformation("Executing Transaction");
                    var res = await tx.RunAsync(query, parameters);
                    //ahora el resultado puede ser multiple
                    //guardamos el resultado en una lista de registros
                    _logger.LogInformation("Fetching records");
                    var records = await res.ToListAsync();
                    // Select() itera sobre cada registro
                    // x => es un registro y x.Values tiene un diccionario donde las claves
                    //seran los nombres de las propiedades de los nodos o relaciones
                    // para obtener una propiedad especifica usamos el [returnObjectKey]
                    //accediendo solo a esa clave
                    data = records.Select(x => (T)x.Values[returnObjectKey]).ToList();
                    //por ultimo lo convertimos en tipo T y luego convertimos todos los registros a una lista

                    _logger.LogInformation("Returning data: {data}", data);
                    return data;
                });
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "There was a problem executing database query.");
                throw;
            }
        }
    }
}