using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Travel.Controllers.Base;
using Travel.Model.Security;

namespace Travel.Security.Authentication
{
    [ApiVersion("1")]
    [Route("")]
    [ApiController]
    public sealed class AuthenticationController : BaseController
    {
        public const string ActionNameAuthentication = "Autenticacion";
        public const string ActionNameAuthorization = "Autorizacion";

        private static readonly string PrivateKeyString = null;
        private static readonly byte[] PrivateKey = null;
        private static readonly byte[] PrivateKeyIV = null;

        private static readonly User PublicUser = new() { Id = 1, UserName = "guest", EMail = "no-reply@email.com", Password = "123", Role = "guest" };

        /// <summary>
        /// Esta variable contiene las sesiones se usuarios en memoria, es el equivalente a una autenticación tipo LDAP, oAuth...
        /// </summary>
        private static readonly IDictionary<string, Model.Security.User> Sessions
            = new Dictionary<string, Model.Security.User>() {
                { PublicUser.UserName , PublicUser }
            };

        private static readonly IDictionary<string, string> SessionsPrivateKeys
            = new ConcurrentDictionary<string, string>();


        /// Diccionario o colección de Roles
        private static readonly IDictionary<string, string> Roles
            = new Dictionary<string, string>()
            {
                { "admin" , "Administrador"} ,
                { "guest" , "Invitado"} 
            };

        /// Diccionario o colección de Los Permisos de Cada Rol.
        private static readonly IDictionary<string, IDictionary<string, string>> Permissions
            = new Dictionary<string, IDictionary<string, string>>()
            {
                { "admin" , new Dictionary<string,string>(){ 
                    {"Listar","List"} ,
                    {"Mostrar","Display"},
                    {"Crear","Create"},
                    {"Actualizar","Actualizar"},
                    {"Eliminar","Delete"},
                } } ,

                { "guest" , new Dictionary<string,string>(){
                    {"Listar","List"},
                    {"Mostrar","Display"},
                } } ,
            };

        /// <summary>
        /// Diccionario o colección de Usuarios
        /// </summary>
        private static readonly IDictionary<string, Model.Security.User> Users
            = new Dictionary<string, Model.Security.User>()
            {
                { "root"  , new User() { Id = 0 , UserName="root" , EMail="root@email.com" , Password = "123" , Role="admin" } },
                { PublicUser.UserName , PublicUser }
            };

        public static bool TryGetSession(string key, out Model.Security.User user) => Sessions.TryGetValue(key, out user);

        static AuthenticationController()
        {
            PrivateKeyString    = Security.SecurityHelper.PrivateKeyString;
            PrivateKey          = Security.SecurityHelper.PrivateKey;
            PrivateKeyIV        = Security.SecurityHelper.PrivateKeyIV;
        }

    /// <summary>
    /// Función encargada de decodificar la información.
    /// </summary>
    /// <param name="privateKey">LLave privada de seguridad.</param>
    /// <param name="encoded">Cadena cifrada.</param>
    /// <returns></returns>
    private static string Decode(string privateKey, string encoded)
        {
            return encoded;
        }

        private static string Encode(string privateKey, string plain)
        {
            string encoded = plain;
            return encoded;
        }

        private static string GetPrivateKey(string session)
        {
            return SessionsPrivateKeys.TryGetValue(session, out var value) ? value : null;
        }

        private static int SessionCounter = 0;
        
        /// <summary>
        /// Función para generar las llaves,
        /// </summary>
        /// <returns></returns>
        private static string GenerateSession()
        {
            SessionCounter = SessionCounter + 1;
            return "KEY_" + SessionCounter.ToString().PadLeft(8, '0');
        }

        /// <summary>
        /// Se crea una sesión y una llave única por sesión para hacer el cifrado de cada sesión
        /// </summary>
        /// <param name="key"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private static bool CreateSession(string key, User user)
        {
            return Sessions.TryAdd(key, user) && SessionsPrivateKeys.TryAdd(key, "_" + key + "_123") ;

        }


        private static readonly string StringErrorContent =
            Core.Utilities.TravelUtilities.AsString(new Dictionary<string, object>()
                {
                    { "action" ,nameof(Authentication) },
                    { "success" ,"true" },
                    { "status" ,"403" }
                });


        [HttpPost, HttpGet]
        [Route(ActionNameAuthentication)]
        public ActionResult Authentication(string username , string password)
        {
            var user =
                Users
                .Where(x => x.Value.UserName == username && x.Value.Password == Decode(PrivateKeyString, password))?
                .FirstOrDefault().Value ?? null;

            if (user != null)
            {
                var key = GenerateSession();
                if (CreateSession(key, user))
                {
                    string response =
                        Encode( PrivateKeyString ,
                        Core.Utilities.TravelUtilities.AsString(new Dictionary<string, object>()
                        {
                            { "success" ,"true" },
                            { "status" ,"200" },
                            { "data" , user },
                            { "realm" , "Travel" },
                            { "key" , key }
                        }));

                    return Content(response);
                }
                else
                {
                    return Content(StringErrorContent);
                }
            }
            else
            {
                return Content(StringErrorContent); 
            }
        }

        private static string GetAuthorizedAction(string bearer, string action)
        {
            //Valida sí la acción está contenida dentro del bearer.
            //Retor el id del usuario sí la acción es válida.
            return PublicUser.UserName;
        }

        [HttpPost, HttpGet]
        [Route(ActionNameAuthorization)]
        public ActionResult Authorization()
        {
            Debug(nameof(Authentication), "Un State-less Service ha solictado permiso de autorización para : hacer algo...");
            Debug(nameof(Authentication), "Aquí se desencripta el bearer del JWT");
            Debug(nameof(Authentication), Body());
            Debug(nameof(Authentication), "Para efectos prácticos siempre retorna exitoso.");

            Request.Headers.TryGetValue("Authorization", out var auth);

            var action = Body();

            if ( Sessions.TryGetValue(GetAuthorizedAction(auth.ToString(), action), out var user))
            {
            }
            else
            {
                Sessions.TryGetValue("guest", out user);
            }

            string authorized = "";
            string success = "true";

            if (user != null)
            {
                if (Roles.TryGetValue(user.Role, out var foundRole)
                     &&
                     foundRole != null
                     &&
                     Permissions.TryGetValue(user.Role, out var permissions)
                     &&
                     permissions != null
                     &&
                     permissions.TryGetValue(action, out var actionFound)
                    )
                {
                    authorized = "true";
                    action = actionFound;
                }
                else
                {
                    authorized = "false";
                }
            }
            else
            {
                authorized = "false";
            }


            return ToString(new Dictionary<string, object>()
            {
                {"success",success },
                {"action",action },
                {"authorized",authorized },
                {"user", user with { Password = "****"  } }
            });

        }


        /// <summary>
        /// Singleton de respuesa base.
        /// </summary>
        private static ActionResult indexResponse = null;

        [HttpPost, HttpGet]
        [Route("")]
        public ActionResult Index()
        {
            if (indexResponse == null)
            {
                indexResponse =
                    ToString(
                new Dictionary<string, object>()
                {
                    { "service" ,"Micro Servicio de Autenticación" },
                    { "status" ,"running" },
                    { "services" ,
                        new Dictionary<string,object>()
                        {
                            { nameof(Authentication) ,
                                new Dictionary<string,object>()
                                {
                                    {"route", ActionNameAuthentication },
                                    {"description", "Servicio de autenticación." }
                                }
                            },

                            { nameof(Authorization) ,
                            new Dictionary<string,object>()
                                {
                                    {"route", ActionNameAuthorization },
                                    {"description", "Servicio de autorización." }
                                }
                            },
                        }
                    },
                });
            }

            return indexResponse;
        }


    }
}
