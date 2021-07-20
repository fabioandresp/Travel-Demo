using Microsoft.AspNetCore.Mvc;
using Travel.Model;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;

namespace Travel.Controllers
{

    /// <summary>
    /// Controlador de servicios de la Línea de Neogocio Travel, Autor.
    /// </summary>
    [ApiVersion("1")]
    [Route(AuthorPrefix)]
    [ApiController]
    [EnableCors(Core.CoreEnvironment.AllowAllHeaders)]
    public sealed class AuthorController : TravelController
    {

        /// <summary>
        /// Error #01 Al generar la Lista.
        /// </summary>
        public const string ERR_AUTH_001 = nameof(ERR_AUTH_001);

        /// <summary>
        /// Error #02 Al generar valores Dummy.
        /// </summary>
        public const string ERR_AUTH_002 = nameof(ERR_AUTH_002);

        /// <summary>
        /// Error al truncar la tabla.
        /// </summary>
        public const string ERR_AUTH_003 = nameof(ERR_AUTH_003);

        /// <summary>
        /// Este es el prefijo del serviciono se podrá cambiar en tiempo de ejecución.
        /// </summary>
        public const string AuthorPrefix = "Autor";

        /// <summary>
        /// Listado de nombres
        /// </summary>
        public static readonly string[] Names = new string[]
        {
                            "Carlos",
                            "María",
                            "Gabriel",
                            "Gabrielle",
                            "Alberto",
                            "Rosse",
                            "Rosa",
                            "Margarita",
                            "Sergio",
                            "Ana",
                            "Miguel",
                            "Mical",
                            "Daniel",
                            "Rivka",
                            "Pedro",
                            "Lea",
                            "Isaac",
                            "Rachel",
                            "Dan",
                            "Israel",
                            "Jacob",
                            "Fabio",
        };

        /// <summary>
        /// Listado e Apellidos.
        /// </summary>
        public static readonly string[] LastNames = new string[]
            {
                            "Cohen",
                            "Gaule",
                            "Franco",
                            "Der Rose",
                            "Martínez",
                            "Sánchez",
                            "Reyes",
                            "Mizrahi",
                            "Palmieri",
                            "Villa",
                            "Rodríguez",
                            "De las Casas",
                            "O'Neil",
                            "Machuca",
                            "Paez",
                            "Quinchanegua",
                            "Pérez",
                };



        /// <summary>
        /// Esta función la lista el API del Controlador Author.
        /// </summary>
        public const string ActionNameIndex = "";
        [HttpPost, HttpGet]
        [Route(ActionNameIndex)]
        public ActionResult Index()
        {
            if (!SecureSession<TravelController>("API Description")) return Unauthorized();

            return Load( Path.Combine("Views","ApiAuthor.html") , 200 );
        }


        /// <summary>
        /// 1. Listar esta funcionalidad muestra todos los autores.
        /// </summary>
        public const string ActionNameList = "Listar";
        [HttpPost, HttpGet]
        [Route(ActionNameList)]
        [ActionName(ActionNameList)]
        public ActionResult List(string pattern)
        {

            if (!SecureSession<TravelController>(ActionNameList)) return Unauthorized();

            try
            {
                List<Author> authors = null;

                Int32.TryParse(pattern, out int numberKey);

                using (var context = new Travel.Model.Context.TDBContext())
                {
                    authors =
                        context.Author.Where(x =>
                        (pattern==null) ? 
                        true :
                            (x.Id == numberKey
                                ||
                                x.Name.ToLower().Contains(pattern.ToLower())
                                ||
                            x.LastName.ToLower().Contains(pattern.ToLower())
                            )
                    ).ToList<Author>();

                }
                return ToString(authors);
            }
            catch (Exception e)
            {
                return HandleException(e, ERR_AUTH_001);
            }
        }


        /// <summary>
        /// 2. Esta función permitirá Mostrar un resultado usando el contenido smartKey para cada llave.
        /// </summary>
        public const string ActionNameDisplay = "Mostrar";
        [HttpPost, HttpGet]
        [Route(ActionNameDisplay)]
        [ActionName(ActionNameDisplay)]
        public ActionResult Display(int id)
        {
            if (!SecureSession<TravelController>(ActionNameDisplay)) return Unauthorized();

            try
            {
                List<Author> authors = new();
                using (var context = new Travel.Model.Context.TDBContext())
                {
                    var author = 
                        context.
                        Author.
                        Where(x => x.Id == id )
                        .FirstOrDefault<Author>();

                        if (author != null)
                        {
                            authors.Add(author);
                        }
                }
                return ToString(authors);
            }
            catch (Exception e)
            {
                return HandleException(e, ERR_AUTH_001);
            }
        }


        /// <summary>
        /// 3.1 Esta función permite crear una nueva instancia de un Autor.
        /// </summary>
        public const string ActionNameCreate = "Crear";
        [HttpPost, HttpGet]
        [Route(ActionNameCreate)]
        [ActionName(ActionNameCreate)]
        public ActionResult Create()
        {
            if (!SecureSession<TravelController>(ActionNameCreate)) return Unauthorized();

            var author = Body<Author>();
            try
            {
                using (var context = new Travel.Model.Context.TDBContext())
                {
                    var newAuthor = author with
                    {
                        Id = context.NextIdentity<Author>()
                    };

                    context.Author.Add(newAuthor);

                    context.SaveChanges();

                    return
                        ToString(new Dictionary<string, object>()
                            {
                            {"success","true" },
                            {"Created", newAuthor }
                            }
                        );
                }
            }
            catch (Exception e)
            {
                return HandleException(e, ERR_AUTH_001);
            }
        }


        /// <summary>
        /// 3.2 Esta función permite actualizar un registro de tipo Autor.
        /// </summary>
        public const string ActionNameUpdate = "Actualizar";
        [HttpPost, HttpGet]
        [Route(ActionNameUpdate)]
        [ActionName(ActionNameUpdate)]
        public ActionResult Update()
        {
            if (!SecureSession<TravelController>(ActionNameUpdate)) return Unauthorized();

            var author = Body<Author>();

            if (author == null)
            {
                ToString(new Dictionary<string, object>()
                            {
                                {"success","false" },
                                {"error","error" },
                                {"reason", "Check body content" }
                            }
                        );
            }
            else
            { 
            
            }

            try
            {
                using (var context = new Travel.Model.Context.TDBContext())
                {
                    
                    var foundRecord =
                        context.Set<Author>()
                        .Where(x => x.Id == author.Id)
                        .AsNoTracking()
                        .FirstOrDefault();
                        

                    if (foundRecord == null)
                    {
                        ToString(new Dictionary<string, object>()
                            {
                                {"success","false" },
                                {"error","error" },
                                {"reason", "Id was not found." }
                            }
                        );
                    }
                    else
                    {
                        Author newRecord = foundRecord with
                            {
                                Name = author.Name ?? foundRecord.LastName,
                                LastName = author.LastName ?? foundRecord.LastName
                            };

                        try
                        {
                            context.Update<Author>(newRecord);
                        }
                        catch (Exception update)
                        {
                            Core.CoreEnvironment.Logger("ERR", "AuthorCOntroller.Update", update.Message, null);
                        }
                        

                        context.SaveChanges();
                    }

                    return
                        ToString(new Dictionary<string, object>()
                            {
                                {"success","ok" },
                                {"Updated", foundRecord}
                            }
                        );
                }
            }
            catch (Exception e)
            {
                return HandleException(e, ERR_AUTH_001);
            }
        }


        /// <summary>
        /// 4. Esta función permitirá Eliminar un Autor existente.
        /// </summary>
        public const string ActionNameDelete = "Eliminar";
        [HttpPost, HttpGet, HttpDelete]
        [Route(ActionNameDelete)]
        [ActionName(ActionNameDelete)]
        public ActionResult Delete(int id)
        {
            if (!SecureSession<TravelController>(ActionNameDelete)) return Unauthorized();

            try
            {
                List<Author> authors = new();
                using (var context = new Travel.Model.Context.TDBContext())
                {
                    authors =
                        context.
                        Author
                        .Where(x => x.Id == id)
                        .ToList();

                    if (authors != null)
                    {
                        foreach (var auth in authors)
                        {
                            //Implementar acción de auditoría.
                            //Audit(DELETE,author);
                            context.Remove(auth);
                            context.SaveChanges();
                        }
                        return ToString(authors);
                    }
                    else
                    {
                        return Content(EmptyArray);
                    }
                }
            }
            catch (Exception e)
            {
                Core.CoreEnvironment.Logger("ERR", nameof(AuthorController) + "." + nameof(Delete) + " | ", e.Message, null);
                return Content(EmptyArray);
            }
        }


        /// <summary>
        /// 5.1 Esta función genera un conjunto de registro inciales.
        /// </summary>
        public const string ActionNameCreateDummy = "Generar";
        [HttpPost, HttpGet]
        [Route(ActionNameCreateDummy)]
        [ActionName(ActionNameCreateDummy)]
        public ActionResult CreateDummy()
        {

            if (!SecureSession<TravelController>(ActionNameCreateDummy)) return Unauthorized();

            List<Author> authors = new();

            try
            {
                using (var context = new Travel.Model.Context.TDBContext())
                {
                int i = 1;
                while (i++ <= 20)
                {
                        var author = new Author()
                        {
                            Id = context.NextIdentity<Author>(),
                            Name = Names[random.Next(0, Names.Length - 1)],
                            LastName = LastNames[random.Next(0, LastNames.Length - 1)],
                        };
                        context.Author.Add(author);
                        context.SaveChanges();
                        authors.Add(author);
                }
                }
                return ToString(authors);
            }
            catch (Exception e)
            {
                return HandleException(e, ERR_AUTH_002);
            }
        }


        /// <summary>
        /// 5.2 Esta función genera un conjunto de registro inciales.
        /// </summary>
        public const string ActionNameTruncate = "Limpiar";
        [HttpPost, HttpGet]
        [Route(ActionNameTruncate)]
        [ActionName(ActionNameTruncate)]
        public ActionResult Truncate()
        {
            if (!SecureSession<TravelController>(ActionNameTruncate)) return Unauthorized();

            try
            {
                using (var context = new Travel.Model.Context.TDBContext())
                {
                    using(var transaction = context.Database.BeginTransaction())
                    {
                        context.Author.RemoveRange(context.Author);
                        context.SaveChanges();
                        transaction.Commit();
                    }
                }
                
                return 
                    ToString(
                        new Dictionary<string, object>()
                        {
                            {"success","true" }
                        }
                );
            }
            catch (Exception e)
            {
                return HandleException(e, ERR_AUTH_003);
            }
        }

    }
}
