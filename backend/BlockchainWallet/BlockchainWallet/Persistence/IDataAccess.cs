using Swashbuckle.AspNetCore.SwaggerGen;

namespace BlockchainWallet.DataAccess
{
    /// <summary>
    /// Interface for data access operations.
    /// </summary>
    public interface IDataAccess : IAsyncDisposable
    {
        /// <summary>
        /// Executes a read query and returns a list of values.
        /// </summary>
        /// <param name="query"> Cypher query </param>
        /// <param name="returnObjectKey"> Prop to be extracted from the specified node </param>
        /// <param name="parameters"> Optional parameters </param>
        /// <returns>
        /// An asynchronous <see cref="Task"/> that, upon completion,
        /// contains a <see cref="List{T}"/> of <see cref="string"/>.
        /// Each string in the list represents a scalar value extracted from the Cypher query result.
        /// </returns>
        Task<List<string>> ExecuteReadListAsync(string query, string returnObjectKey,
            IDictionary<string, object>? parameters = null);

        /// <summary>
        /// Executes a read query and returns a list of dictionaries.
        /// </summary>
        /// <param name="query"> Cypher query</param>
        /// <param name="returnObjectKey"> Property to filter or group the results </param>
        /// <param name="parameters"> Optional parameters </param>
        /// <returns>
        /// An asynchronous <see cref="Task"/> that, upon completion,
        /// contains a <see cref="List{T}"/> of <see cref="Dictionary{TKey,TValue}"/>.
        /// Each dictionary in the list represents a record or node/relationship, with keys being property names
        /// and values being the corresponding data.
        /// </returns>

        Task<List<Dictionary<string, object>>> ExecuteReadDictionaryAsync(string query, string returnObjectKey,
            IDictionary<string, object>? parameters = null);

        /// <summary>
        /// Executes a read query and returns a single value.
        /// </summary>
        /// <typeparam name="T"> Type to return</typeparam>
        /// <param name="query"> Cypher query </param>
        /// <param name="parameters"> Optional parameters</param>
        /// <returns>
        /// An asynchronous <see cref="Task"/> that, upon completion,
        /// contains a scalar value of type <typeparamref name="T"/>.
        /// </returns>

        Task<T> ExecuteReadScalarAsync<T>(string query, IDictionary<string, object>? parameters = null);

        /// <summary>
        /// Executes a write transaction and returns a single value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns>
        /// An asynchronous <see cref="Task"/> that, upon completion,
        /// contains a value of type <typeparamref name="T"/>, representing the result of the transaction.
        /// </returns>

        Task<T> ExecuteWriteTransactionAsync<T>(string query, IDictionary<string, object>? parameters = null);
    }
}