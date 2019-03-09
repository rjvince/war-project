using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodOldWar.Models
{
    public class Game
    {
        public int Id { get; set; }
        public List<Player> players { get; set; }
        public bool Over() => players.Exists(p => p.Deck.IsEmpty());
    }
}
