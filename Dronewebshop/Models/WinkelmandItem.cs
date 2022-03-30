using System.ComponentModel.DataAnnotations;
namespace Dronewebshop.Models
{
    public class WinkelmandItem
    {
        public int ArtNr { get;set;}
        public int? KlantID { get; set;} 
        [Range(1,100)]
        public int Aantal { get; set;}  
        public string Naam { get; set;}
        public double Prijs { get; set;}
        public string FotoNaam { get; set; }
        public double Totaal { get; set; }

    }
}
