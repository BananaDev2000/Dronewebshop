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
            return View(vMToevoegen);
        }

        [HttpPost]
        public IActionResult Toevoegen(VMToevoegen vMToevoegen)
        {
            vMToevoegen.Product.Voorraad = pc.haalVoorraad(vMToevoegen.Product.ArtNr);
            if (vMToevoegen.aantal <= vMToevoegen.Product.Voorraad)
            {
                return RedirectToAction("Winkelmand");
            }
            else
            {
                ViewBag.fout = "Te weining voorraad";
                return View(vMToevoegen);
            }
        }

    }
}