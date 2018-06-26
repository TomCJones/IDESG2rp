using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IDESG2rp.Models;

namespace IDESG2rp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "IDESG Relying Party best practice description.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Contacts for this Site.";
            return View();
        }

        public IActionResult FAQ()
        {
            ViewData["Message"] = "Frequently Asked Questions.";
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
