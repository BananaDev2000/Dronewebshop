namespace Dronewebshop.Models
{
    public class Product
    {
        public int ArtNr { get; set; }
        public string Naam {get;set;}
        public double Prijs { get; set; }
        public IFormFile Foto {get;set;}
        public int Voorraad {get;set;}

    }
}
