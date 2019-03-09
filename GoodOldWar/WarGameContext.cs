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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=wargame.db");
        }
    }
}
