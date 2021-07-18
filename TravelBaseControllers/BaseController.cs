using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travel.Model.Base;
using Travel.Model.Security;
using Travel.Core.Utilities;

namespace Travel.Controllers.Base
{
    /// <summary>
    /// Esta función se encarga de evaluar los encabezados para determinar las llaves y tokens que serán pasados el servicio de seguridad.
    /// </summary>
    /// <param name="controller">Es el request http REST</param>
    /// <param name="actionKey">Es la operación que se desea validar.</param>
    /// <returns></returns>
    public delegate bool SecureAuthorizationsDelegate(BaseController controller, string actionKey);

    /// <summary>
    /// Esta clase es el controlador base para los microservicios de las diferentes unidades de negocios, por ejemplo la línea de negocio biblioteca. La cual despliega la lista de autores. Esta implementación es de tipo: <b>State-less</b> excepto en el servicio de Autenticación la cual sí maneja estados en memoria.
    /// </summary>
    public abstract class BaseController : Controller
    {
        //Definción de variables estáticas.
        public const string EmptyArray = "[]";

        private string Buffer = null;

        protected readonly Random random = new();

        /// <summary>
        /// Método por defecto que permite el acceso a cualquier funcionalidad.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="actionKey"></param>
        /// <returns></returns>
        private static bool PublicAccess(BaseController controller, string actionKey) => true;

        private static readonly Type BaseType = typeof(BaseController);

        /// <summary>
        /// Este diccionario contiene renderizado los contenidos de las vistas de documentación. Su propósito no es ser un servidor de vistas.
        /// </summary>
        private static readonly Dictionary<string, ActionResult> ViewContent = new();

        /// <summary>
        /// Este diccionario contiene el content type de los archivos.
        /// </summary>
        private static readonly Dictionary<string, string> ViewContentType = new();

        //Esta función implementa la autenticación contra un servidor de autenticación.
        private static readonly Dictionary<Type, SecureAuthorizationsDelegate> Authorizations = new()
        {
            { BaseType, PublicAccess }
        };

        /// <summary>
        /// Esta propiedad privada contiene en un diccionario las variables usada para transmitir la seguridad, es decir, los Headers de un reuqest HTTP.
        /// </summary>
        private readonly IDictionary<string, object> RequestContext = new Dictionary<string, object>();

        protected Session Session { set; get; } = null;

        /// <summary>
        /// Esta función se encarga de verificar si la operación de tipo actionKey es permitida para la sesión actual.
        /// </summary>
        /// <param name="actionKey">Es la llave de la operación que se desea realizar.</param>
        /// <returns>Sí el usuario puede o no realizar la acción solicitada.</returns>
        protected virtual bool SecureSession<T>(string actionKey) where T : BaseController
        {
            if (Authorizations.TryGetValue(typeof(T), out var secureMethod))
            {
                return secureMethod?.Invoke(this, actionKey) ?? false;
            }
            else
            {
                //Sí no se sobre-escribe la seguridad del controlador, por ejemplo la del controlador AuthorController, se usará por defecto el controlador
                if (Authorizations.TryGetValue(BaseType, out secureMethod))
                {
                    return secureMethod?.Invoke(this, actionKey) ?? false;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Este método permite sobre-escribir la implementacio de seguridad. Es una forma de hacer injección de funcionaldes (dependencias) sobre la clase BaseController, 
        /// Por ejemplo: 
        /// AuthenticationServices, por sí mismo (per se) no depende de la seguridad de otros servicios, pues este es el módulo de seguridad.
        /// Por otra Parte El MicroServicio BusinessUnity01_port:5000 requiere implementar este método para poder solicitar al primer micro-servicio la autorización.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool OverRideSecureImplementation<T>(SecureAuthorizationsDelegate method) where T : Base.BaseController
        {
            return Authorizations.TryAdd(typeof(T), method);
        }

        /// <summary>
        /// Este método maneja las exceptions por defectos.
        /// </summary>
        /// <param name="eHnd">Manerjador de la excepción normalmente es la excepción.</param>
        /// <param name="code">Código de error para documentar la excepción.</param>
        /// <returns></returns>
        protected ActionResult HandleException(Exception eHnd , string code)
        {
            //Chiste del día 5 de la prueba, lo que odian los desarrolladores.
            Response.StatusCode = 500;
            Core.CoreEnvironment.Logger("ERR", GetType().FullName + " | " + code , eHnd.Message, null);
            return Content(eHnd.Message);
        }

        /// <summary>
        /// Esta función permite cargar el contenido de las vistas que documentan las APIs, se usa un caché de tipo File-Singleton para guardar el contenido.
        /// </summary>
        /// <param name="path">Es la ruta del contenido que se deberá descargar.</param>
        /// <param name="httpStatus">Es el status html, 200 OK, 404 Not Found.</param>
        /// <returns>Retorna un contenido estático cargado en memoria.</returns>
        protected ActionResult Load(string path, int httpStatus)
        {
            string viewPath =
                Path.Combine(Directory.GetCurrentDirectory(), path);

            if (ViewContent.TryGetValue(path, out var content))
            {
                if (ViewContentType.TryGetValue(path, out var contentType))
                {
                    Response.ContentType = contentType;
                }
                return content;
            }

            if (System.IO.File.Exists(viewPath))
            {
                try
                {
                    string buffer = System.IO.File.ReadAllText(viewPath);
                    ActionResult contentResponse =  Content(buffer);
                    //Guarda el contenido en memoria.
                    if (ViewContent.TryAdd(viewPath, contentResponse))
                    {
                        ViewContentType.TryAdd(viewPath, "html");
                    }
                    return contentResponse;
                }
                catch (Exception e)
                {
                    return Content("Api Author was not found.");
                }
            }
            else
            {
                return Content("Api Author was not found.");
            }
        }

        /// <summary>
        /// Este método convierte una Lista en un json-String
        /// </summary>
        /// <typeparam name="T">Tipo de objeto contenido en la lista. Debe heredar de <b>BaseModel</b></typeparam>
        /// <param name="list"></param>
        /// <returns>Una cadena en formato json del objeto serializado.</returns>
        protected ActionResult ToString<T>(IList<T> list)  where T : BaseModel
        {
            if (list != default)
            {
                if (TravelUtilities.TryAsString(list, out var json))
                {
                    return Content(json);
                }
                else
                {
                    return Content(EmptyArray);
                }
            }
            else
            {
                return Content(EmptyArray);            
            }
        }


        protected ActionResult ToString(IDictionary<string,object> jsonObject) 
        {
            if (jsonObject != null)
            {
                if (TravelUtilities.TryAsString(jsonObject, out var json))
                {
                    return Content(json);
                }
                else
                {
                    return Content(EmptyArray);
                }
            }
            else
            {
                return Content(EmptyArray);
            }
        }


        /// <summary>
        /// Esta función devuelve el contenido del body en formato string, el cual es enviado como contenido del Request.
        /// </summary>
        /// <returns>Devuelve el cuerpo (Body) del Request en formato string.</returns>
        protected string Body()
        {
            try
            {
                if (Buffer != null)
                {
                    
                }
                else
                {
                    Request.EnableBuffering();
                    var buffer = new byte[Convert.ToInt32(Request.ContentLength)];
                    Request.Body.ReadAsync(buffer, 0, buffer.Length);
                    Buffer = Encoding.UTF8.GetString(buffer);
                }
                return Buffer;
            }
            catch
            {
                return default;
            }
        }


        /// <summary>
        /// Hace la respectiva vinculación de el contenido del json (deserializa) con relación a un valor de tipo T
        /// </summary>
        /// <typeparam name="T">Es el tipo que se desea convertir.</typeparam>
        /// <returns>Retorna un instancia de tipo T en función del json.</returns>
        protected T Body<T>()
        {
            return TravelUtilities.Deserializate<T>(Body());
        }

        /// <summary>
        /// Esta función decoradora registra un mensage de error en el logger.
        /// </summary>
        /// <param name="type">Tipos : ERR , DEBUG, LOG, INFO </param>
        /// <param name="block"></param>
        /// <param name="message"></param>
        protected virtual void Log(string type, string block, string message)
        {
            Logger(type, this.GetType().Name + ">" + block, message);
        }

        protected static void Logger(string type,string block, string message)
        {
            Travel.Core.CoreEnvironment.Logger(type, block, message, null);
        }

        public static void Error(string block, string message)
        {
            Logger("ERR",block, message);
        }

        public static void Debug(string block, string message)
        {
            Logger("DEBUG", block, message);
        }

        /// <summary>
        /// Esta función devuelve el contenido
        /// </summary>
        /// <returns>Diccionario con el encabezado del HTTP/S Requets</returns>
        public IDictionary<string, object> AuthenticationHeader { 
            get 
            {
                if (RequestContext.Count == 0)
                {
                    try
                    {
                        foreach (var item in Request.Headers)
                        {
                            RequestContext.TryAdd(item.Key, item.Value);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger("ERR", this.GetType().Name, e.Message);
                    }
                }

                return RequestContext;
            }
        }
        
    }
}
