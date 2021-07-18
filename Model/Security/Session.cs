
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travel.Model.Base;

namespace Travel.Model.Security
{
    /// <summary>
    /// Dado que la implementación se secure state-less, esta implementación persiste la autenticación en una base de datos de memoria que se implementa em el projecto de Autenticación.
    /// </summary>
    [TravelEntity(nameof(Session), "Sesión 'Persistente'")]
    public sealed record Session : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Token { set; get; }
        public int UserId { set; get; }

    }
}
