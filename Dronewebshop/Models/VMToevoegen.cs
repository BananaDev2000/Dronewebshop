using System.ComponentModel.DataAnnotations;
namespace Dronewebshop.Models
{
    public class VMToevoegen
    {
        public Product Product { get; set; }
   //   [Range(1, Product.Voorraad, ErrorMessage = "Oeps")]
        public int aantal { get; set; }

    }
}
