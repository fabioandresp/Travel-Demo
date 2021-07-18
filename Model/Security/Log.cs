
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travel.Model.Base;

namespace Travel.Model.Security
{
    /// <summary>
    /// Entidad de logs, para registrar eventos en la base de datos.
    /// </summary>
    [TravelEntity(nameof(Log), "Log")]
    public sealed record Log 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string GUID { set; get; }
        
        public string Message { set; get; }


    }
}
