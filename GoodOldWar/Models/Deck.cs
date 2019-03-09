using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodOldWar.Models
{
    public class Deck
    {
        public int Id { get; set; }

        public List<PlayingCard> Cards { get; set; }

        public int Count()
        {
            return Cards.Count();
        }

        public bool IsEmpty()
        {
            return Count() == 0;
        }

        public void Shuffle()
        {
            Cards.Shuffle(3);
        }

        public Tuple<Deck, Deck> Cut()
        {
            int newSize = Cards.Count / 2;

            Deck deck1 = new Deck { Cards = Cards.GetRange(0, newSize) };
            Deck deck2 = new Deck { Cards = Cards.GetRange(newSize, newSize) };
            Cards.Clear();

            return new Tuple<Deck, Deck>(deck1, deck2);
        }

        public void RefreshSequence()
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                Cards[i].Sequence = i;
            }
        }

        public PlayingCard DealTopCard()
        {
            return Cards.Top(); 
        }

        public void AddCardsToBottom(List<PlayingCard> cards)
        {
            Cards.AddRange(cards);
            RefreshSequence();
        }
    }
}
