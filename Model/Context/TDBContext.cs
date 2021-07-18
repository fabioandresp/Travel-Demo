
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using Travel.Model.Base;
using Travel.Model.Security;

namespace Travel.Model.Context
{
    /// <summary>
    /// Este contexto de datos hace el mapeo relacional de la base de datos.
    /// </summary>
    public class TDBContext : DbContext
    {

        //Usa las variables más concurridas como readonly para ayudar al GC.
        private static readonly string EFSConnection;
        private const string METHOD_PLAIN = nameof(METHOD_PLAIN);

        private Session session;

        //Esta colección almacena el listado de instancia de los autores.
        public DbSet<Author> Author { get; set; }
        
        //Esta colección almacena el listado de instancia de las editorial.
        public DbSet<Issuer> Issuer { get; set; }

        //Esta colección almacena el listado de instancia de los libros.
        public DbSet<Book> Book { get; set; }


        //Esta colección almacena las instancias de los usuarios del sistema.
        public DbSet<User> User { get; set; }

        //Esta colección almacena las sesiones de los usuarios.
        public DbSet<Session> Session { get; set; }

        public static string ConnectionString { get => EFSConnection; }
        static TDBContext()
        {
            //Desencriptar la conección
            EFSConnection = Travel.Core.Utilities.TravelUtilities.Decode(Travel.Core.CoreEnvironment.EncryptedConnection , METHOD_PLAIN);
        }


        public TDBContext() : base()
        {
            Database.Migrate();
        }

        private DbContextOptionsBuilder optionsBuilder;
        protected override void OnConfiguring(DbContextOptionsBuilder optBuilder) 
        {
            optionsBuilder = optBuilder;
            optionsBuilder.UseSqlServer(EFSConnection);
        }

        public static DbContextOptionsBuilder FactoryBuilder(DbContextOptionsBuilder builder)
        {
            return builder.UseSqlServer(EFSConnection,
                b => b.MigrationsAssembly("Travel.Model")
                );
        }

#warning FP: Posible error de tipo ACID, en la Implementación del ORM. Se sugiere usar GUID en vez de Identity para aplicaciones altamente concurrentes.
        /// <summary>
        /// <b>WARN</b>Esta función retorna el siguiente ID Libre no es save-thread.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int NextIdentity<T>() where T : BaseModel
        {
            switch (typeof(T).Name)
            {
                case nameof(Author):
                        return Author?
                            .OrderByDescending(x => x.Id)?
                            .FirstOrDefault()?.Id + 1 ?? 1;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Este método recompila las entidades, en este punto no existe EDMX para EF Core, por lo que se implementa este verificador.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Ejecutar el modelador del padre.
            base.OnModelCreating(modelBuilder);

            //Aquí se pueden agregar ensamblados externos en tiempo de ejecución.
            string[] assemblies = new string[]
                {
                    "Travel.Model"
                };

            foreach (string assembly in assemblies)
            {
                try
                {
                    Assembly assm = Assembly.Load(assembly);
                    //Este módulo escanea todas las clases/registros del ensamblado, que hereden de BaseModel.
                    foreach (var entity in 
                        assm.GetTypes().Where( x => x.BaseType.Equals(typeof(BaseModel)) )
                        )
                    {
                        modelBuilder.Entity( entity.FullName , 
                            b =>
                        {
                            //Por Cada propiedad de la clases genere el modelo.
                            try
                            {
                                //Recrea todos los atributos.
                                foreach (var property in entity.GetProperties())
                                { 
                                    b.Property(property.PropertyType , property.Name )
                                    .ValueGeneratedOnAdd()

                                    //Se usa esto para no establecer un Identity en el caso de SQLServer.
                                    .ValueGeneratedNever();
                                }

                                //b.HasKey(nameof(Base.BaseModel.Id));

                                b.ToTable(entity.Name);

                            }
                            catch (Exception RebuildModel)
                            {
                                Core.CoreEnvironment.Logger("ERR", "TDBContext.OnModelCreating", RebuildModel.Message, null);
                            }
                            finally
                            {

                            }
                        });
                    }

                    

                }
                catch
                {

                }
                finally
                { 
                
                }
            }

            

        }

    }
}
