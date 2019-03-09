using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoodOldWar.Models;
using Microsoft.EntityFrameworkCore;

namespace GoodOldWar.Controllers
{
    public class WarController : Controller
    {
        private readonly WarGameService _service;

        public WarController(WarGameService srv)
        {
            _service = srv;
        }

        [HttpPost("/game")]
        public GameStateDTO Game(string player1name, string player2name)
        {
            return _service.CreateNewGame(player1name, player2name);           
        }      
        
        [HttpGet("/game/{id}/next")]
        public GameStateDTO PlayNext(int id)
        {
            return _service.PlayNextHand(id);
        }
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
