using BlockchainWallet.Config;
using Microsoft.Extensions.Options;
using Neo4j.Driver;

namespace BlockchainWallet.Repositories
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
            _database = appsettingsOptions.Value.Neo4jDatabase ?? "Instance01";
            _session = driver.AsyncSession(o => o.WithDatabase(_database));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources asynchronously.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
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
                var result = await _session.ExecuteReadAsync(
                    async tx => // async tx => representa la sesion activa
                {//3.Lógica que se ejecuta dentro de la transacción
                    // crea la variable scalar para almacenar el resultado
                    //del tipo T especificado por el método y la inicia
                    T scalar = default(T);

                    var res = await tx.RunAsync(query, parameters);
                    scalar = (await res.SingleAsync())[0].As<T>();
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

                var result = await _session.ExecuteWriteAsync(async tx =>
                {
                    T scalar = default(T);
                    var res = await tx.RunAsync(query, parameters);
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
                var result = await _session.ExecuteReadAsync(async tx =>
                {
                    var data = new List<T>();
                    var res = await tx.RunAsync(query, parameters);
                    var records = await res.ToListAsync();
                    data = records.Select(x => (T)x.Values[returnObjectKey]).ToList();
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