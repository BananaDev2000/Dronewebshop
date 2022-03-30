using System.ComponentModel.DataAnnotations;
namespace Dronewebshop.Models
{
    public class LoginCredentials
    {
        [Required(ErrorMessage = "Verplicht veld.")]
        public string gebruikersnaam { get; set; }
        [Required(ErrorMessage = "Verplicht veld.")]
        public string wachtwoord { get; set; }

    }
}
