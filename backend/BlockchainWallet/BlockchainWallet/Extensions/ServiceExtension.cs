using BlockchainWallet.Config;
using Microsoft.Extensions.Options;
using Neo4j.Driver;

namespace BlockchainWallet.Extensions
{
    /// <summary>
    /// Clase de extensión de los servicios de la aplicación.
    /// Separa la logica de configuración para dejar el <see cref="Program"/> más limpio y legible.
    ///
    /// </summary>

    public static class ServiceExtension
    {
        ///<summary>
        /// Este método agrega la configuración de Neo4j a la colección de servicios.
        /// </summary>
        ///
        ///<param name="services"> Colección de servicios del builder de la aplicación</param>
        /// <param name="configuration"> Configuración del builder de la aplicación </param>
        /// <returns>
        ///  La <see cref="IServiceCollection"/> para que se puedan encadenar llamadas.
        ///
        /// </returns>

        public static IServiceCollection AddDbConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            //bindea los valores del appsettings.json a la clase Neo4jSettings
            //1. registra la clase indicada como un objeto IOptions dentro de la colección de servicios
            services.Configure<Neo4jSettings>(configuration.GetSection("Neo4jSettings"));

            //2. devuelve la colección de servicios para
            //encadenar llamadas
            return services;
        }

        /// <summary>
        ///  Este método agrega el driver de Neo4j a la colección de servicios.
        /// </summary>
        /// <param name="services"> Colección de servicios del builder de la aplicación</param>
        /// <param name="configuration"> Configuración del builder de la aplicación</param>
        /// <returns>
        /// La <see cref="IServiceCollection"/> para que se puedan encadenar llamadas.
        /// </returns>
        /// <exception cref="InvalidOperationException"></exception>

        public static IServiceCollection AddDrivers(this IServiceCollection services,
            IConfiguration configuration)
        {
            //1. crea una sola instancia del driver y lo registra como un servicio
            services.AddSingleton<IDriver>(provider =>
            //provider es el contenedor de servicios
            //es de donde viene la colección de llamadas de los servicios anteriores
            {
                //2. obtiene los valores de configuración de Neo4j ya registrados previamente
                var settings = provider.GetRequiredService<IOptions<Neo4jSettings>>().Value;

                //3. validación de las credenciales
                if (string.IsNullOrEmpty(settings.Neo4jConnection.ToString()) ||
                    string.IsNullOrEmpty(settings.Neo4jUser) ||
                    string.IsNullOrEmpty(settings.Neo4jPassword)
                   )
                {
                    throw new InvalidOperationException("Neo4j Credentials were not provided. Check appsettings.json");
                }
                //4. devuelve el driver creado
                return GraphDatabase.Driver(settings.Neo4jConnection,
                    AuthTokens.Basic(settings.Neo4jUser, settings.Neo4jPassword));
            });

            //5. devuelve la colección de servicios modificada
            //para encadenar llamadas
            return services;
        }
    }
}