using System.Diagnostics;
using Bombardiro_Cardealo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Bombardiro_Cardealo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;
        private static List<Car> Cars = new List<Car>();
        private static List<Car> WishList = new List<Car>();
        private static HashSet<int> RemovedCarIds = new HashSet<int>();

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Browse()
        {
            var visibleCars = Cars.Where(c => !RemovedCarIds.Contains(c.Id)).ToList();
            ViewBag.Wishlist = WishList; 
            ViewBag.WishlistCount = WishList.Count; 
            return View(visibleCars);
        }

        [HttpGet]
        public IActionResult AddCar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCar(Car car, IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                var fileName = Path.GetFileName(image.FileName);
                var imagesPath = Path.Combine(_env.WebRootPath, "images");

                if (!Directory.Exists(imagesPath))
                {
                    Directory.CreateDirectory(imagesPath);
                }

                var filePath = Path.Combine(imagesPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }

                car.ImageFileName = fileName;
            }

            car.Id = Cars.Count + 1;
            Cars.Add(car);
            return RedirectToAction("Browse");
        }

        [HttpPost]
        public IActionResult AddToWishlist(int id)
        {
            var car = Cars.FirstOrDefault(c => c.Id == id);
            if (car != null && !WishList.Any(c => c.Id == id))
            {
                WishList.Add(car);
            }
            return RedirectToAction("Browse");
        }

        [HttpPost]
        public IActionResult RemoveCar(int id)
        {
            RemovedCarIds.Add(id);
            return RedirectToAction("Browse");
        }

        public IActionResult Wishlist()
        {
            return View(WishList);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public IActionResult Buy(int id)
        {
            var car = WishList.FirstOrDefault(c => c.Id == id);
            if (car != null)
            {
                WishList.Remove(car);
                TempData["Message"] = "Thanks for Buying in Bombardiro Cardealo";
            }
            return RedirectToAction("Wishlist");
        }

        [HttpPost]
        public IActionResult RemoveFromWishlist(int id)
        {
            var car = WishList.FirstOrDefault(c => c.Id == id);
            if (car != null)
            {
                WishList.Remove(car);
            }
            return RedirectToAction("Wishlist");
        }
        [HttpPost]
        public IActionResult RemoveFromBrowse(int id)
        {
            var car = Cars.FirstOrDefault(c => c.Id == id);
            if (car != null)
                Cars.Remove(car);
            return RedirectToAction("Browse");
        }
    }
}
