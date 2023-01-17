﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplicationCoreLogin.Models;

namespace WebApplicationCoreLogin.Controllers
{
    [Authorize]//isem erişime izin ver yani login olduysam
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous] 
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous] 
        public IActionResult AccessDenied()
        {
            return View();
        }
          [AllowAnonymous] 
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous] 
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}