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
        PersistenceCode pc = new PersistenceCode();
        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            int? UsrId = HttpContext.Session.GetInt32("ID");
            if (UsrId is not null)
            {
                ProductRepository productsRepository = new ProductRepository();
                productsRepository.Producten = pc.loadProducten();
                return View(productsRepository);
            }
            else
            {
                return RedirectToAction("Login","Auth");
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult Toevoegen(int pid)
        {
            VMToevoegen vMToevoegen = new VMToevoegen();
            vMToevoegen.Product = pc.loadProduct(pid);
            HttpContext.Session.SetInt32("ArtNr", pid);
            return View(vMToevoegen);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Toevoegen(VMToevoegen vMToevoegen)
        {

            if (ModelState.IsValid)
            {

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
                    ViewBag.fout = "Te weining voorraad";
                    return View(vMToevoegen);
                }

            }
            else
            {

                return View(vMToevoegen);

            }
           
        }

        [Authorize]
        [HttpGet]
        public IActionResult Winkelmand()
        {
            VMWinkelmand vMWinkelmand = new VMWinkelmand();
            int klantNr = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
            vMWinkelmand.gebruiker = pc.haalGebruiker(klantNr);
            WinkelmandItemRepository winkelmandrepos = new WinkelmandItemRepository();
            vMWinkelmand.winkelmandItemRepos = winkelmandrepos;
            winkelmandrepos.winkelmandItems = pc.loadWinkelitems(klantNr);
            Totalen totalen = new Totalen();
            foreach(var winkelmandItem in winkelmandrepos.winkelmandItems)
            {
                totalen.totaalExcl += Math.Round(winkelmandItem.Totaal,2);
                totalen.BTW += Math.Round((winkelmandItem.Totaal * 0.21),2);
            }
            totalen.totaalIncl = Math.Round(totalen.totaalExcl + totalen.BTW,2);
            vMWinkelmand.totalen = totalen;
            return View(vMWinkelmand);

        }
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
        public IActionResult WinkelmandReturn()
        {
            return RedirectToAction("Winkelmand");
        }
        [Authorize]
        [HttpPost]
        public IActionResult Winkelmand(Order order)
        {
            int KlantNr = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
            order = pc.maakOrder(KlantNr);

            try
            {
                //Mailserver
                SmtpClient smtpClient = new SmtpClient("smtp.office365.com");
                smtpClient.Port = 587;
                smtpClient.Credentials = new System.Net.NetworkCredential("haspo6itn2022@outlook.com", "GIPwebsite");
                smtpClient.EnableSsl = true;
                
                //MailBericht
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("haspo6itn2022@outlook.com");
                mailMessage.To.Add(order.mail);
                mailMessage.Subject = "Bestelbevestiging";
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = " <h1>Droneshop.be - Bestelbevestiging</h1> <p>We hebben uw bestelling met bestelnummer<strong> " + order.OrderNr + "</strong>goed ontvangen.</p> <p>Na overschrijving van<strong> " + order.totaal + " </strong>op rekeningnummer <strong>BE12 3442 0694 2069</strong> worden de goederen verpakt en verstuurd.</p> <p>U ontving deze bevestiging ook via e-mail.</p> <p>Bedankt voor uw vertouwen!</p> <p>M. Royer en F. Timmermans</p>";


                smtpClient.Send(mailMessage);
                    }
            catch (Exception) { }

            return RedirectToAction("Bevestiging", order);
        }
        [Authorize]
        [HttpGet]
        public IActionResult Bevestiging(Order order)
        {
            return View(order);
        }


    }
}