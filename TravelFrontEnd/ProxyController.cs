
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Travel.View
{
    /// <summary>
    /// Esta clase se encarga de implementar el proxy pass, adicionalmente delega la implementación de la seguridad, como validación de IPs
    /// </summary>
    [EnableCors(Core.CoreEnvironment.AllowAllHeaders)]
    [ApiVersion("1")]
    [ApiController]
    public sealed class ProxyController : Controller
    {



        public const string MESSAGE_ERR_01_01 = "Action was not found.";
        public const string URL_BASE_PROXY = "proxy";

        private string Buffer = null;

        static ProxyController()
        {

        }

        private string Body()
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

        private static byte[] Body(HttpContext controller)
        {
            byte[] buffer = null;
            try
            {
                controller.Request.EnableBuffering();
                buffer = new byte[Convert.ToInt32(controller.Request.ContentLength)];
                controller.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            }
            catch
            {
                
            }
            return buffer;
        }

        private static void Logger(string type, string code, string message)
        {
            System.Console.WriteLine(type + "|" + (code??"") + "|" + message);
        }

        /// <summary>
        /// Esta función se encarga de hacer el proxy forward al BU01
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static void ProxyPass(HttpContext controller, string url)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                StringBuilder queryURL = new StringBuilder();
                queryURL.Append("?proxyPassed=true");

                try
                {
                    foreach (var key in controller.Request.Query)
                    {
                        queryURL.Append('&');
                        queryURL.Append(key.Key);
                        queryURL.Append('=');
                        queryURL.Append(key.Value);
                    }
                }
                catch
                {

                }
                string finalURL = url + queryURL.ToString();

                var webRequest = System.Net.WebRequest.Create(finalURL);

                if (webRequest != null)
                {
                    webRequest.Timeout = 3000;
                    webRequest.Method = controller.Request.Method;
                    webRequest.ContentType = controller.Request.ContentType;

                    try
                    {
                        foreach (var key in controller.Request.Headers)
                        {
                            webRequest.Headers.Add(key.Key, key.Value);
                        }
                    }
                    catch 
                    {
                    
                    }

                    try
                    {
                        foreach (var key in controller.Request.Form)
                        {
                            
                        }
                    }
                    catch
                    {

                    }

                    try
                    {
                        Stream dataStream = webRequest.GetRequestStream();
                        byte[] byteArray = Body(controller);
                        dataStream.Write(byteArray, 0, byteArray.Length);
                    }
                    catch
                    { 
                    
                    }


                    using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            sb.Append( sr.ReadToEnd());
                        }
                    }
                }


                controller.Response.BodyWriter.WriteAsync(
                        new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(sb.ToString()))
                    );

                //controller.Response.StatusCode = 200;

                return;
            }
            catch (Exception e)
            {
                //controller.Response.StatusCode = 500;

                controller.Response.BodyWriter.WriteAsync(
                        new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(e.Message))
                    );
            }
        }


        /// <summary>
        /// Retorna un mensaje con estado http dentro del request.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="body"></param>
        /// <param name="status"></param>
        private static void Message(HttpContext context, string body, int status)
        {
            context.Response.StatusCode = status;
            context.Response.BodyWriter.WriteAsync( new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(body)));
        }

        /// <summary>
        /// Esta función cuando detecta que una página del front end no es administrada por el Razor, o los routes creados en las anotaciones de las clases, las busca dentro del Manejador de peticiones. 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        public static void HelperRequest(HttpContext context, Func<Task> next)
        {
            string url = context.Request.Path ;
            string[] paths = url.Split("/");
            if (paths.Length > 3)
            {
                switch (paths[1].ToLower())
                {
                    case URL_BASE_PROXY :
                    {
                        if (Core.CoreEnvironment.TryGetRoute(paths[2], paths[3], out var urlFound))
                        {
                                Logger("DEBUG", null, "Front End ProxyPass [this]" + url  + " -> " + urlFound);
                                ProxyPass(context, urlFound);
                        }
                        else
                        {
                                Message(context, MESSAGE_ERR_01_01, 404);
                                Logger("ERR", "ERR_01_01", "Mapped Proxy");
                            }
                    }break;
                    
                    default:
                        Message(context, MESSAGE_ERR_01_01, 404);
                        Logger("ERR", "ERR_01_02", "Mapped Proxy");
                        break;
                }
            }
            else
            {
                Message(context, MESSAGE_ERR_01_01, 404);
                Logger("ERR", "ERR_01_03", "Mapped Proxy");
            }
        }


    }
}
