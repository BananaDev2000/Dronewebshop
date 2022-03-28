using System.ComponentModel.DataAnnotations;
namespace Dronewebshop.Models
{
    public class LoginCredentials
    {
        [Required(ErrorMessage = "Verplicht :D")]
        public string gebruikersnaam { get; set; }
        [Required(ErrorMessage = "Verplicht :D")]
        public string wachtwoord { get; set; }

    }
}
