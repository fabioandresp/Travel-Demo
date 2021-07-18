using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travel.Model.Base;
using Travel.Model.Context;

namespace Travel.Model
{

    /// <summary>
    /// Libro, es cada elemento de la biblioteca, este contiene la información detallada de un libro
    /// </summary>
    [TravelEntity(nameof(Book),"Libro")]
    public sealed record Book : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ISBN { set; get; }

        /// <summary>
        /// Los libros pertenecen a una única editorial. Esta propiedad es un enlace
        /// </summary>
        [TravelDescription("Editorial_ID")]
        [TravelForeign(nameof(Issuer),nameof(Model.Issuer.Id))]
        public int Issuer_FK { set; get; }

        
        [TravelDescription("Título")]
        [StringLength(45, MinimumLength = 1)]
        public string Title { set; get; }

        
        [TravelDescription("Sinopsis")]
        [StringLength(45, MinimumLength = 1)]
        public string Synapsis { set; get; }

        
        [TravelDescription("N_Paginas")]
        [StringLength(45, MinimumLength = 1)]
        public string N_Pages { set; get; }


        //public Issuer Issuer() { return null; }


        #region Delegates
        /// <summary>
        /// Esta función delegada debe ser implementada en el ModelBuilder para acceder fluidamente mediante Linq a la instancia relacionada en esta llave.
        /// No es de obligatoria implementación, pero´permitirá cambios de éxtensión sobre código cerrado (Open/Closed principle)
        /// </summary>
        internal static GetEntity<Issuer, Book, TDBContext> GetIssuer = null;

        #endregion

    }
}
