
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Travel.Controllers.Base;

namespace Travel.Controllers
{
    /// <summary>
    /// Este controlador abstracto es el padre de todos los controladores de la unidad de negocio 01 de Travel, las implementaciones de controladores hijas, este controlador validará todas las autorizaciones usando el micro servicio AuthenticationService puerto 5010
    /// Para esto es suficiente con usar este bloque: 
    /// </summary>
    public abstract class TravelController : BaseController
    {
        /// <summary>
        /// Este método recibe un BaseController Implementation y una acción definida por el sistema y valida contra el servidor de autenticación sí la acci+on puede ser realizada o no.
        /// De esta forma se delega al micro servicio AuthenticationService que corre en el puerto 5010 (http) la responsabilidad de la seguridad.
        /// </summary>
        /// <example>
        /// if (!SecureSession<TravelController>(ActionNameList)) return Unauthorized();
        /// </example>
        /// <param name="controller"></param>
        /// <param name="actionKey"></param>
        /// <returns></returns>

        private static readonly string[] keys
            = new string[] { "Authorization" };

        /// <summary>
        /// Esta funcionalidad heredable a todos los controladores consultará al micro servicio de Autenticación usando el servicio. <b>"http://localhost:5010/Autorizacion"</b>
        /// </summary>
        /// <param name="controller">Es el request HTTP/S</param>
        /// <param name="actionKey">Es el nombre de la acción, sin embargo el JWT Token contiene en su encabezado la acción que será realizada.</param>
        /// <returns></returns>
        private static bool JWTAuthorization(BaseController controller, string actionKey)
            {
                string url = "https://localhost:5010/Autorizacion";
                IDictionary<string, object> jsonAuth = null;

                string response = "";

                Debug("Authorization Request", url);
                try
                {
                    var webRequest = System.Net.WebRequest.Create(url);
                    if (webRequest != null)
                    {
                        webRequest.Method = "POST";
                        webRequest.Timeout = 3000;
                        webRequest.ContentType = "application/json";

                        Stream dataStream = webRequest.GetRequestStream();
                        byte[] byteArray = Encoding.ASCII.GetBytes(actionKey); 
                        dataStream.Write(byteArray, 0, byteArray.Length);

                    foreach (var key in keys)
                        {
                            if (controller.Request.Headers.TryGetValue(key, out var value))
                            {
                                webRequest.Headers.Add(key, value);
                            }
                        }

                        using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                        {
                            using (System.IO.StreamReader sr = new(s))
                            {
                                response = sr.ReadToEnd();
                                jsonAuth = Travel.Core.Utilities.TravelUtilities.Deserializate<Dictionary<string, object>>(response);
                            }
                        }
                    }

                    if (jsonAuth == null)
                    {
                        return false;
                    }
                    else
                    {
                        if (jsonAuth.TryGetValue("success", out var successValue) &&
                            successValue is string strValue && 
                            strValue == "true"
                        )
                        {
                            //Autorización es exitosa.
                            Debug("Autorización OK según MS de Seguridad.", url);
                            Debug("json",response);
                        return true;
                        }
                        else
                        {
                            return false;
                        }

                    }

                }
                catch (Exception e)
                {
                    Error("Authorization", e.Message);
                    return true;
                }
            }

            /// <summary>
            /// Constructor Estático que permite implementar(injectar) la funcinalidad de Autorización
            /// </summary>
            static TravelController()
            {
                OverRideSecureImplementation<TravelController>(JWTAuthorization);
            }
    }
}
