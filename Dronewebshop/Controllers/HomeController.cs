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
            ProductRepository productsRepository = new ProductRepository();
            productsRepository.Producten = pc.loadProducten();
            return View(productsRepository);
        }

    }
}