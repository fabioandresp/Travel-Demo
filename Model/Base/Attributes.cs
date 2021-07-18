using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travel.Model.Base
{

    /// <summary>
    /// Esta clase representa una relación n:m es el manejador , es complementario al ORM para admnistrar la DB, sirve para tener en runtime un mapping de toda las entidades, pero no se implementa el scan de los ensamblados, objeticvo: Alta escalabilidad, unificación de cambios en entidades. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RelationShipAttribute : TableAttribute
    {
        public RelationShipAttribute(params string[] properties) : base(properties[0])
        { 
            // Para propósitos de documentación y autogeneración de código.
        }
    }

    /// <summary>
    /// Esta entidad representa una Entidad del proyecto Travel
    /// </summary>
    [AttributeUsage(AttributeTargets.Class , Inherited = false)]
    public class TravelEntityAttribute : TableAttribute 
    {
        public TravelEntityAttribute(string entity, string alias) : base(entity)
        {
            // Para generar SQL expecíficos.
        }
    }


    /// <summary>
    /// Clase usada para documentar las propiedades de las entidades, a menudo se implementar para especificar Llaves, Índices y forzar la ejecución de sentencias.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class TravelDescriptionAttribute : Attribute
    {
        public TravelDescriptionAttribute(string description) 
        {
            // Para propósitos de documentación y autogeneración de código.
        }
    }

    /// <summary>
    /// Modelado de las llaves únicas.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UniqueKeyAttribute : Attribute
    {
        public UniqueKeyAttribute(params string[] properties)
        {
            // Para propósitos de documentación y autogeneración de código.
        }
    }

    /// <summary>
    /// Clase usada para documentar las propiedades de las entidades, a menudo se implementar para especificar Llaves, Índices y forzar la ejecución de sentencias.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class TravelForeignAttribute : Attribute
    {
        public TravelForeignAttribute(string entity, string property)
        {
            // Para propósitos de documentación y autogeneración de código.
        }
    }

    /// <summary>
    /// Clase usada para compilar el modelo, es decir, crear manualmente índices, particiones.
    /// </summary>
    public static class TravelModelBuilder
    {
        public static bool CompileUniqueConstraint()
        {
            /*
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                      .HasName("IX_User")
                      .IsUnique();

                entity.HasAlternateKey(u => u.Email);

                entity.HasIndex(e => e.Email)
                      .HasName("IX_Email")
                      .IsUnique();
            });/**/
            return true;
        }
    
    }
}
