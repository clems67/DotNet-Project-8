using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shared
{
    [Table("Consultant")]
    public partial class Consultant
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string FName { get; set; }

        [StringLength(100)]
        public string LName { get; set; }

        [StringLength(50)]
        public string Speciality { get; set; }
    }
}
