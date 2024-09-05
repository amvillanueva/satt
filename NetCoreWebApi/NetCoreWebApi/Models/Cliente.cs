using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NetCoreWebApi.Models
{
    public class Cliente
    {
        [Key]
        public int id { get; set; } 
        public string nombres { get; set; } = String.Empty;
        public string apellidos { get; set; } = String.Empty;
        public DateTime fechaNac { get; set; }

        [NotMapped]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int edad { get; set; }
    }
}
