using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Travel.Controllers.Base;

namespace Travel.Security.Authentication
{
    [ApiVersion("1")]
    [Route("")]
    [ApiController]
    public sealed class AuthenticationController : BaseController
    {
        public const string ActionNameAuthentication = "Autenticacion";
        public const string ActionNameAuthorization = "Autorizacion";
        
        [HttpPost, HttpGet]
        [Route(ActionNameAuthentication)]
        public ActionResult Authentication()
        {
            return ToString(
                new Dictionary<string, object>()
                {
                    { "action" ,nameof(Authentication) },
                    { "success" ,"true" }
                });
        }



        [HttpPost, HttpGet]
        [Route(ActionNameAuthorization)]
        public ActionResult Authorization()
        {
            Debug(nameof(Authentication), "Un State-less Service ha solictado permiso de autorización para : hacer algo...");
            Debug(nameof(Authentication), "Aquí se desencripta el bearer del JWT");
            Debug(nameof(Authentication), Body());
            Debug(nameof(Authentication), "Para efectos prácticos siempre retorna exitoso.");

            return ToString(
                new Dictionary<string, object>()
                {
                    { "action" ,nameof(Authorization) },
                    { "success" ,"true" }
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
