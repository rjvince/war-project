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
    public class HomeController : Controller
    {
        private readonly WarGameService _service;

        public HomeController(WarGameService srv)
        {
            _service = srv;
        }

        [HttpPost("/game")]
        public GameStateDTO Game(string player1name, string player2name = "CPU")
        {
            GameStateDTO game =_service.CreateNewGame(player1name, player2name);
            game.Link = $"{Request.Path.Value}/{game.GameId}/next";
            
            return game;
        }

        [HttpGet("/game/reload")]
        public GameStateDTO GameReload(int id)
        {
            GameStateDTO game = _service.GetGame(id);
            game.Link = $"/game/{game.GameId}/next";

            return game;
        }

        [HttpGet("/game/{id}")]
        public GameStateDTO Game(int id)
        {
            GameStateDTO game = _service.GetGame(id);
            game.Link = $"{Request.Path.Value}/next";

            return game;
        }

        [HttpGet("/game/{id}/next")]
        public GameStateDTO PlayNext(int id)
        {
            GameStateDTO game = _service.PlayNextHand(id);
            if (!game.GameOver)
            {
                game.Link = $"{Request.Path.Value}";
            }
            else
            {
                game.Link = "/";
            }
            return game;
        }
        
        public IActionResult Index()
        {
            List<Game> games = _service.GetNRecentGames(5);

            ViewData["GameList"] = games;

            return View();
        }

        public IActionResult Stats()
        {
            ViewData["StatList"] = _service.GetStats();
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "War for two players";

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
