using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travel.Model.Base;

namespace Travel.Model
{
    /// <summary>
    /// Esta clase crea una instancia de un autor, actor que crea un Libro. 
    /// </summary>
    [TravelEntity(nameof(Author),"Autor")]
    public record Author : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; init; }


        [TravelDescription("Nombres")]
        [StringLength(45)]        
        public string Name { get; init; }


        [TravelDescription("Apellidos")]
        [StringLength(45)]
        public string LastName { get; init; }
    }
}
