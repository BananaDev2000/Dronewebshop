using Dronewebshop.Models;
namespace Dronewebshop.Models
{
    public class VMWinkelmand
    {

        public WinkelmandItemRepository winkelmandItemRepos { get; set; }
        public Totalen totalen { get; set; }
        public Gebruiker gebruiker { get; set; }
        public ProductRepository productRepo { get; set; }


    }
}
