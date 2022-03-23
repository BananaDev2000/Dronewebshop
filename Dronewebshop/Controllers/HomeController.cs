using Dronewebshop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Dronewebshop.Persistence;

namespace Dronewebshop.Controllers
{
    public class HomeController : Controller
    {
        PersistenceCode pc = new PersistenceCode();
        public IActionResult Index()
        {
            HttpContext.Session.SetInt32("ID", 1);
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
                totalen.totaalExcl += winkelmandItem.Totaal;
                totalen.BTW += (winkelmandItem.Totaal * 0.21);
            }
            totalen.totaalIncl = totalen.totaalExcl + totalen.BTW;
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

    }
}