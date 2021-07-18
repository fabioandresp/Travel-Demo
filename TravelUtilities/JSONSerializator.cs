using System;

namespace Travel.Core.Utilities
{
    /// <summary>
    /// Este Delegado es la impelemntación del deserializador JSON.
    /// </summary>
    /// <typeparam name="T">Es el tipo del Objeto a deserializar</typeparam>
    /// <param name="instance">Es el objeto que será deserializado.</param>
    /// <returns></returns>
    public delegate string DeserializatorDelegate<T>(object instance);

    /// <summary>
    /// Implementación de un deserializador rápido, que puede ser cambiado, esto se complementar para las extensiones del BaseModel Travel, por lo tanto la implementan Author, Book, Issuer, y las relaciones util para retornar string y mantener singletons. .
    /// </summary>

    public static class TravelUtilities
    {
        static TravelUtilities()
        {
            //Cargar Deserializador 
        }

        /// <summary>
        /// Des-Encriptación plana de la cadena de conexión.
        /// </summary>
        /// <param name="encoded">Cadena encriptada</param>
        /// <returns></returns>
        public static string Decode(string encoded)
        {
            return encoded;
        }

        /// <summary>
        /// Des-Encriptación plana de la cadena de conexión.
        /// </summary>
        /// <param name="encoded">Cadena encriptada</param>
        /// <param name="method">Método de encriptación</param>
        /// <returns></returns>
        public static string Decode(string encoded, string method)
        {
            return encoded;
        }

        /// <summary>
        /// Implementación de un deserializador rápido, que puede ser cambiado, esto se complementar para las extensiones del BaseModel Travel, por lo tanto la implementan Author, Book, Issuer, y las relaciones util para retornar string y mantener singletons. 
        /// Patrón decorador implementado para soportar el cambio de Serializadores en todo el projecto Travel.
        /// <b>El serializador intentará un deep serialization, pero esta será garantizada para propiedades y atributos visibles.</b>
        /// </summary>
        /// <typeparam name="T">Tipo del objeto que será serilizado.</typeparam>
        /// <param name="instance">objeto que será convertido a JSON String</param>
        /// <returns>La cadena serializada en formato json.</returns>
        public static string AsString<T>(T instance) => TryAsString<T>(instance, out var json) ? json : null;

        /// <summary>
        /// Implementación del serializador con el patrón TryDo
        /// </summary>
        /// <typeparam name="T">Tipo del objeto que será serilizado.</typeparam>
        /// <param name="instance">objeto que será convertido a JSON String</param>
        /// <param name="json">Retorna la cadena serializada en formato json.</param>
        /// <returns>Sí el objeto pudo ser convertido o no, <b>el serializador intentará un deep serialization, pero esta será garantizada para propiedades y atributos visibles.</b></returns>
        public static bool TryAsString<T>(T instance, out string json) 
        {
            try
            {
                //Aquí se podrá re-configurar el serializador.
                json = Utf8Json.JsonSerializer.ToJsonString(instance);
                return true;
            }
            catch
            {
                json = null;
                return false;
            }
        }


        /// <summary>
        /// Esta función deserializa un string a un objeto.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool TryDeserializate<T>(string json, out T instance) 
        {
            var t = typeof(T).Name;
            try
            {
                instance = Utf8Json.JsonSerializer.Deserialize<T>(json);
                return true;
            }
            catch (Exception e)
            {
                Core.CoreEnvironment.Logger("ERR", "Deserializate", e.Message, null);
                instance = default;
                return false;
            }
        }

        public static T Deserializate<T>(string buffer)
        {
            TryDeserializate<T>(buffer, out var instance);
            return instance;
        }
    }
}
