using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodOldWar.Models
{
    public class PlayingCard
    {
        public int Id { get; set; }
        public int Rank { get; set; }
        public Suite Suite { get; set; }
        public int Sequence { get; set; }

        public PlayingCard(int rank, Suite suite)
        {
            this.Rank = rank;
            this.Suite = suite;
        }

        
        override public string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RankName(Rank)).Append(" of ").Append(Suite);

            return builder.ToString();
        }

        private string RankName(int rank)
        {
            string name;

            switch(rank)
            {
                case 11:
                    name = "Jack";
                    break;
                case 12:
                    name = "Queen";
                    break;
                case 13:
                    name = "King";
                    break;
                case 14:
                    name = "Ace";
                    break;
                default:
                    name = rank.ToString();
                    break;
            }

            return name;
        }
    }

    public enum Suite
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }
}

