
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travel.Model.Base;
//First-CODE Implementation.
namespace Travel.Model.Security
{
    /// <summary>
    /// Esta clase corresponse a Usuarios.
    /// </summary>
    [TravelEntity(nameof(User), "Usuario")]
    public sealed record User : BaseModel 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        public string UserName { set; get; }
        public string EMail { set; get; }
        public string Password { set; get; }
        public string Role { set; get; }
    }
}
