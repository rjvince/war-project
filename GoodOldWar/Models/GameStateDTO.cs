using System.Collections.Generic;

namespace GoodOldWar.Models
{
    public class GameStateDTO
    {
        public int GameId { get; internal set; }
        public bool GameOver { get; internal set; }
        public List<string> LastPlay { get; internal set; }
        public int Player1Score { get; internal set; }
        public int Player2Score { get; internal set; }
    }
    
}