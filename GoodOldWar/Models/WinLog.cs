using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodOldWar.Models
{
    public class WinLog
    {
        public WinLog() { }
        public WinLog(string name, int cards)
        {
            this.PlayerName = name;
            this.CardsWon = cards;
        }
        public int Id { get; set; }
        public string PlayerName { get; set; }
        public int CardsWon { get; set; }
    }
}
