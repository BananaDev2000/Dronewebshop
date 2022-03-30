using Dronewebshop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Dronewebshop.Persistence;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;

namespace Dronewebshop.Controllers
{
    public class HomeController : Controller
    {
        //We maken een instance aan van de persistence code om deze doorheen de controller te gebruiken.
        PersistenceCode pc = new PersistenceCode();
        [Authorize]
        [HttpGet]
        //We laden het assortiment in.
        public IActionResult Index()
        {
            int? UsrId = HttpContext.Session.GetInt32("ID");
            if (UsrId is not null)
            {
                //Als de klant al ingelogd is laden we eht assortiment.
                ProductRepository productsRepository = new ProductRepository();
                productsRepository.Producten = pc.loadProducten();
                return View(productsRepository);
            }
            else
            {
                //Als de klant nog niet ingelogd is wordt hij naar de inlogpagina verwezen.
                return RedirectToAction("Login","Auth");
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult Toevoegen(int pid)
        {
            VMToevoegen vMToevoegen = new VMToevoegen();
            if (pid == 0)
            {
                //De view toevoegen wordt teruggestuurd als de klant als aantal een 0 of een negatief getal ingeeft. Je ziet iets verder hoe dit in werking gaat.
                int ArtNr = Convert.ToInt32(HttpContext.Session.GetInt32("ArtNr"));
                vMToevoegen.Product = pc.loadProduct(ArtNr);
                ViewBag.fout = "Foutief aantal.";
                return View(vMToevoegen);
            }
            else
            {
                //We laden het product in dat de klantgevraag heeft.
                vMToevoegen.Product = pc.loadProduct(pid);
                HttpContext.Session.SetInt32("ArtNr", pid);
                return View(vMToevoegen);
            }
        }
        [Authorize]
        [HttpPost]
        public IActionResult Toevoegen(VMToevoegen vMToevoegen)
        {
            //We kijken na of het getal groter als 0 is
            if ((vMToevoegen.aantal > 0))
            {
                if (ModelState.IsValid)
                {
                    //Als het aantal binnen de grenzen van de voorraad is zal de klant na het drukken op de knop bestellen naar het winkelmandje doorverwezen worden.
                    int ArtNr = Convert.ToInt32(HttpContext.Session.GetInt32("ArtNr"));
                    vMToevoegen.Product = pc.loadProduct(ArtNr);
                    vMToevoegen.Product.Voorraad = pc.haalVoorraad(ArtNr);
                    if (vMToevoegen.aantal <= vMToevoegen.Product.Voorraad)
                    {

                        WinkelmandItem winkelmandItem = new WinkelmandItem();
                        winkelmandItem.Aantal = vMToevoegen.aantal;
                        winkelmandItem.KlantID = HttpContext.Session.GetInt32("ID");
                        winkelmandItem.ArtNr = vMToevoegen.Product.ArtNr;
                        pc.voegToe(winkelmandItem);
                        return RedirectToAction("Winkelmand");
                    }
                    else
                    {
                        //Bij een te groot aantal wordt de view toevoegen terug geladen.
                        ViewBag.fout = "Te weining voorraad";
                        return View(vMToevoegen);
                    }

                }
                else
                {
                    //Als de model niet geldig is sturen we de view terug door.
                    return View(vMToevoegen);

                }
            }
            else
            {
                //Als het aantal kleiner als 0 is wordt de klant terugverwezen naar view toevoegen waar we hiervoor uilteg hebben gegeven.
                return RedirectToAction("Toevoegen");
            }

           
        }

        //Het winkelmandje wordt geladen.
        [Authorize]
        [HttpGet]
        public IActionResult Winkelmand()
        {
            if (ModelState.IsValid == true)
            {
                //We halen inofrmatie op van de klant en de producten die er besteld zijn.
                VMWinkelmand vMWinkelmand = new VMWinkelmand();
                int klantNr = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                vMWinkelmand.gebruiker = pc.haalGebruiker(klantNr);
                WinkelmandItemRepository winkelmandrepos = new WinkelmandItemRepository();
                vMWinkelmand.winkelmandItemRepos = winkelmandrepos;
                winkelmandrepos.winkelmandItems = pc.loadWinkelitems(klantNr);
                Totalen totalen = new Totalen();
                foreach (var winkelmandItem in winkelmandrepos.winkelmandItems)
                {
                    //We berekenen de totalen.
                    totalen.totaalExcl += Math.Round(winkelmandItem.Totaal, 2);
                    totalen.BTW += Math.Round((winkelmandItem.Totaal * 0.21), 2);
                }
                totalen.totaalIncl = Math.Round(totalen.totaalExcl + totalen.BTW, 2);
                vMWinkelmand.totalen = totalen;
                return View(vMWinkelmand);
            }

            ViewBag.fout = "Ongeldig aantal.";
            return View();

        }

        // ArtikelNummer en Aantal worden gebruikt om het item uit de winkelmand te verwijderen.
        [Authorize]
        [HttpGet]
        public IActionResult Verwijder(int ArtNr, int Aantal)
        {
            WinkelmandItem winkelmandItem = new WinkelmandItem();
            winkelmandItem.ArtNr = ArtNr;
            winkelmandItem.Aantal = Aantal;
            winkelmandItem.KlantID = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
            pc.Verwijder(winkelmandItem);
            return RedirectToAction("Winkelmand");
        }
        [Authorize]
        // De winkelmand wordt ingeladen.
        public IActionResult WinkelmandReturn()
        {
            return RedirectToAction("Winkelmand");
        }

        // Er wordt een oder gemaakt vanuit het winkelmandje.
        [Authorize]
        [HttpPost]
        public IActionResult Winkelmand(Order order)
        {
            int KlantNr = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
            order = pc.maakOrder(KlantNr);

            try
            {
                // Er wordt een connectie gemaakt met de mail service om een mail te versturen.
                SmtpClient smtpClient = new SmtpClient("smtp.office365.com");
                smtpClient.Port = 587;
                smtpClient.Credentials = new System.Net.NetworkCredential("webshopgip6itn@outlook.com", "");
                smtpClient.EnableSsl = true;
                
                // Er wordt een mail bericht opgesteld, met ontvanger, onderwerp en een bericht.
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("webshopgip6itn@outlook.com");
                mailMessage.To.Add(order.mail);
                mailMessage.Subject = "Bestelbevestiging";
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = " <h1>Droneshop.be - Bestelbevestiging</h1> <p>We hebben uw bestelling met bestelnummer<strong> " + order.OrderNr + " </strong>goed ontvangen.</p> <p>Na overschrijving van<strong> " + order.totaal + " </strong>op rekeningnummer <strong>BE12 3442 0694 2069</strong> worden de goederen verpakt en verstuurd.</p> <p>U ontving deze bevestiging ook via e-mail.</p> <p>Bedankt voor uw vertouwen!</p> <p>M. Royer en F. Timmermans</p>";

                // De mail wordt verstuurd.
                smtpClient.Send(mailMessage);
                    }
            catch (Exception) { }

            return RedirectToAction("Bevestiging", order);
        }

        // De order bevestigingspagina wordt geladen met alle informatie van het order.
        [Authorize]
        [HttpGet]
        public IActionResult Bevestiging(Order order)
        {
            return View(order);
        }


    }
}