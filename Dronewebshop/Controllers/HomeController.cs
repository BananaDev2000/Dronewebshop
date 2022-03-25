using Dronewebshop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Dronewebshop.Persistence;
using System.Net;
using System.Net.Mail;

namespace Dronewebshop.Controllers
{
    public class HomeController : Controller
    {
        PersistenceCode pc = new PersistenceCode();
        public IActionResult Index()
        {
            HttpContext.Session.SetInt32("ID", 2);
            int? id = HttpContext.Session.GetInt32("ID");
            if (id is not null)
            {
                ProductRepository productsRepository = new ProductRepository();
                productsRepository.Producten = pc.loadProducten();
                return View(productsRepository);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public IActionResult Toevoegen(int pid)
        {
            VMToevoegen vMToevoegen = new VMToevoegen();
            vMToevoegen.Product = pc.loadProduct(pid);
            HttpContext.Session.SetInt32("ArtNr", pid);
            return View(vMToevoegen);
        }

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

        public IActionResult Verwijder(int ArtNr, int Aantal)
        {
            WinkelmandItem winkelmandItem = new WinkelmandItem();
            winkelmandItem.ArtNr = ArtNr;
            winkelmandItem.Aantal = Aantal;
            winkelmandItem.KlantID = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
            pc.Verwijder(winkelmandItem);
            return RedirectToAction("Winkelmand");
        }

        public IActionResult WinkelmandReturn()
        {
            return RedirectToAction("Winkelmand");
        }

        [HttpPost]
        public IActionResult Winkelmand(Order order)
        {
            int KlantNr = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
            order = pc.maakOrder(KlantNr);

            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("drones@nyrztest.xyz");
                message.To.Add(new MailAddress("egl.banaan@gmail.com"));
                message.Subject = "Test";
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = "    <h1>Droneshop.be - Bestelbevestiging</h1> < p > We hebben uw bestelling met bestelnummer<strong> @Model.OrderNr </ strong > goed ontvangen.</ p >< p > Na overschrijving van<strong> @Model.totaal</ strong > op rekeningnummer<strong> BE12 3442 0694 2069 </ strong > worden de goederen verpakt en verstuurd.</ p >< p > U ontving deze bevestiging ook via e-mail.</ p > < p > Bedankt voor uw vertouwen! </ p >< p > M.Royer en F. Timmermans </ p > ";
                smtp.Port = 465;
                smtp.Host = "mail.zxcs.nl"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("drones@nyrztest.xyz", "drones");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception) { }

            return RedirectToAction("Bevestiging", order);
        }

        public IActionResult Bevestiging(Order order)
        {
            return View(order);
        }

    }
}