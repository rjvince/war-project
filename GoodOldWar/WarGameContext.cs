using GoodOldWar.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodOldWar
{
    public class WarGameContext : DbContext
    {
        public WarGameContext(): base() { }
        public WarGameContext(DbContextOptions<WarGameContext> options)
        : base(options) { }
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Deck> Decks { get; set; }
        public DbSet<PlayingCard> PlayingCards { get; set; }

        public Game GetGameEntireState(int gameId)
        {
            Game game =
            Games.Include(g => g.players)
                    .ThenInclude((Player p) => p.Deck)
                        .ThenInclude((Deck d) => d.Cards)
            .Single(g => g.Id == gameId);

            foreach (Player player in game.players)
            {
                player.Deck.Cards = player.Deck.Cards.OrderBy(c => c.Sequence).ToList();
            }

            return game;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=wargame.db");
        }
    }
}
