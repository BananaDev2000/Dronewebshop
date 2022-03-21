using System.ComponentModel.DataAnnotations;
namespace Dronewebshop.Models
{
    public class VMToevoegen
    {
        public Product Product { get; set; }
        [Required(ErrorMessage ="Verplicht veld")]
        public int aantal { get; set; }

    }
}
