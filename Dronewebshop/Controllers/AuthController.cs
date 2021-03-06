using Microsoft.AspNetCore.Mvc;
using Dronewebshop.Models;
using Dronewebshop.Persistence;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;

namespace Dronewebshop.Controllers
{
    public class AuthController : Controller
    {

        PersistenceCode pc = new PersistenceCode();
        // Login pagina wordt geladen.
        public IActionResult Login()
        {
            return View();
        }

        // Er wordt gekeken of gebruikersnaam en wachtwoord overeenkomen in de databank.
        // Als dit zo is dan wordt de gebruik ingelogd en wordt zijn ID bijgehouden.
        [HttpPost]
        public IActionResult Login(LoginCredentials LC)
        {
            if (ModelState.IsValid)
            {
                if (pc.checkCredentials(LC) != -1)
                {
                    int usrID = pc.checkCredentials(LC);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, LC.gebruikersnaam)
                    };
                    var userIdentity = new ClaimsIdentity(claims, "SecureLogin");
                    var userPrincipal = new ClaimsPrincipal(userIdentity);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties { IsPersistent = true, ExpiresUtc = System.DateTime.Today.AddDays(1) });
                    HttpContext.Session.SetInt32("ID",usrID);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.fout = "Ongeldige login poging";
                    return View();
                }
            }
            else
            {
                return View(LC);
            }
        }

        // De gebruiker wordt uitgelogd.
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }

    }
}
