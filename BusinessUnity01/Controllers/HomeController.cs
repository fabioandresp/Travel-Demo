using Microsoft.AspNetCore.Mvc;
using Travel.Controllers.Base;
using Travel.Model;
using System.IO;
using Microsoft.AspNetCore.Cors;

namespace Travel.Controllers
{
    [EnableCors(Core.CoreEnvironment.AllowAllHeaders)]
    public sealed class HomeController : BaseController
    {

        [HttpPost, HttpGet]
        [Route(nameof(Dummy))]
        public ActionResult Dummy()
        {

            using( var context = new Travel.Model.Context.TDBContext() )
            {
                var authors =
                    context.Author.Add(new Author()
                    {
                        Id = context.NextIdentity<Author>(),
                        Name = "Nombre",
                        LastName = "Apellido"
                    }); ;
            }
            return Content("");
            
        }


        [HttpPost, HttpGet]
        [Route("")]
        public ActionResult Index()
        {
            return Load(Path.Combine("Views","Index.html"), 200);
        }

    }
}
