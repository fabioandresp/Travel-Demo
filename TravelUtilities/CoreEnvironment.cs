using System;
using System.Collections.Generic;

namespace Travel.Core
{


    public static class CoreEnvironment
    {
        public static string EncryptedConnection;

        public static string ProxyPass;

        public static string ProxyBase;

        private static readonly IDictionary<string, IDictionary<string, string>> Routes = new Dictionary<string, IDictionary<string, string>>();

        /// <summary>
        /// Esta función se encarga de crear el mapa de rutas del proxy contra el back end para evitar el problema de CORS en navegadores.
        /// </summary>
        /// <param name="controller">Nombre del controlador</param>
        /// <param name="action">Nombre de la función</param>
        /// <param name="url">URL que será usada para redirigir las acciones.</param>
        /// <returns></returns>
        public static bool AddRoute(string controller, string action, string url)
        {
            if (Routes.TryGetValue(controller, out var controllerRoute))
            {
                
            }
            else
            {
                controllerRoute = new Dictionary<string, string>();
                Routes.TryAdd(controller, controllerRoute);
            }

            return controllerRoute?.TryAdd(action, url) ?? false;
        }

        public static bool TryGetRoute(string controller, string action, out string url)
        {
            url = null;
            return
                Routes.TryGetValue(controller, out var controllerRoutes)
                &&
                (controllerRoutes?.TryGetValue(action, out url) ?? false);
        }

        public const string AllowAllHeaders = nameof(AllowAllHeaders);

        

        /// <summary>
        /// Este método permite registrar logs en la base de datos.
        /// </summary>
        /// <param name="type">ERROR, LOG, DEBUG</param>
        /// <param name="block">Es el código o llave del mensaje. Por ejemplo ERROR:90001 </param>
        /// <param name="message">Es el mensaje que será registrado.</param>
        /// <param name="hnd">Este el manejador a un objeto que puede ser pasado, para implementar colas de logs de hilos diferentes.</param>
        public static void Logger(string type, string block, string message, ILogger hnd)
        { 
            System.Console.WriteLine(type + " | "  + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") +"|" + block + "|" + message);
        }
    }
}
