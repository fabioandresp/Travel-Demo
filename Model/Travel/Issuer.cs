using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travel.Model.Base;

namespace Travel.Model
{
    /// <summary>
    /// Editor del Libro, es la empresa que publica los libros físicamente.
    /// Una Editorial puede tener más de un Libro. ( 0:N )
    /// </summary>
    [TravelEntity(nameof(Issuer),"Editorial")]
    public sealed record Issuer : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }


        [TravelDescription("Nombre")]
        [StringLength(45, MinimumLength = 1)]
        public string Name { get; set; }


        [TravelDescription("Sede")]
        [StringLength(45, MinimumLength = 1)]
        public string Facility { get; set; }
    }
}
