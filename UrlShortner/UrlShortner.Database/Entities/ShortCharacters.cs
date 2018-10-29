using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortner.Database.Entities
{
    [Table("ShortCharacters")]
    public class ShortCharacters
    {
        [Key]
        public int Id { get; set; }
        public string Characters { get; set; }
    }
}
