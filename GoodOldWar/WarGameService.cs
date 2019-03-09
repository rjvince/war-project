using GoodOldWar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace GoodOldWar
{
    public class WarGameService
    {
        private readonly WarGameContext _context;
        public WarGameService(WarGameContext ctx)
        {
            _context = ctx;
        }

        public GameStateDTO CreateNewGame(string player1name, string player2name)
        {
            Game game = new Game();

            Player player1 = new Player { Name = player1name };
            Player player2 = new Player { Name = player2name };

            Deck cards = CreateDeck();
            cards.Shuffle();
            Tuple<Deck, Deck> decks = cards.Cut();
            player1.Deck = decks.Item1;
            player2.Deck = decks.Item2;
            player1.Deck.RefreshSequence();
            player2.Deck.RefreshSequence();

            game.players = new List<Player> { player1, player2 };

            _context.Add(game);
            _context.SaveChanges();

            GameStateDTO dto = new GameStateDTO
            {
                GameId = game.Id,
                GameOver = false,
                LastPlay = new List<string>(),
                Player1Score = player1.Deck.Count(),
                Player2Score = player2.Deck.Count()
            };

            return dto;
        }

        public GameStateDTO PlayNextHand(int gameId)
        {
            Game currentGame = _context.Games.Include(g => g.players).Single(g => g.Id == gameId);
            
            Player player1 = _context.Players.Include(p => p.Deck).Single(p => p.Id == currentGame.players[0].Id);
            Player player2 = _context.Players.Include(p => p.Deck).Single(p => p.Id == currentGame.players[1].Id);

            Deck deck1 = _context.Decks.Include(d => d.Cards).Single(d => d.Id == player1.Deck.Id);
            Deck deck2 = _context.Decks.Include(d => d.Cards).Single(d => d.Id == player2.Deck.Id);

            deck1.Cards = deck1.Cards.OrderBy(card => card.Sequence).ToList();
            deck2.Cards = deck2.Cards.OrderBy(card => card.Sequence).ToList();

            List<string> plays = new List<string>();
            List<PlayingCard> prize = new List<PlayingCard>();

            HandResult handResult = DecideHandResult(player1, player2, prize, plays);

            while (handResult == HandResult.War && !currentGame.Over())
            {
                handResult = DecideHandResult(player1, player2, prize, plays);
            }

            if(currentGame.Over())
            {
                plays.Add("Game Over");
                plays.Add($"{ (deck1.IsEmpty() ? player1.Name : player2.Name) } is out of cards.");
            }

            _context.SaveChanges();

            GameStateDTO dto = new GameStateDTO
            {
                GameId = currentGame.Id,
                GameOver = (deck1.Count() == 0 || deck2.Count() == 0),
                LastPlay = plays,
                Player1Score = deck1.Count(),
                Player2Score = deck2.Count()
            };

            return dto;
        }

        private HandResult DecideHandResult(Player player1, Player player2, List<PlayingCard> prize, List<string> plays)
        {
            Deck deck1 = player1.Deck;
            Deck deck2 = player2.Deck;

            PlayingCard deck1top = deck1.DealTopCard();
            PlayingCard deck2top = deck2.DealTopCard();

            plays.Add($"{player1.Name} played {deck1top}");
            plays.Add($"{player2.Name} played { deck2top}");
            prize.Add(deck1top);
            prize.Add(deck2top);

            int comparison = deck1top.Rank - deck2top.Rank;
            if (comparison > 0)
            {
                prize.Shuffle(1);
                deck1.AddCardsToBottom(prize);
                deck1.RefreshSequence();
                deck2.RefreshSequence();
                plays.Add($"{player1.Name} won {prize.Count()} cards");
                return HandResult.Player1Win;
            }
            else if (comparison < 0)
            {
                prize.Shuffle(1);
                deck2.AddCardsToBottom(prize);
                deck1.RefreshSequence();
                deck2.RefreshSequence();
                plays.Add($"{player2.Name} won {prize.Count()} cards");
                return HandResult.Player2Win;
            }
            else
            {
                //War! Two unseen cards are dealt into the prize pool
                prize.Add(deck1.DealTopCard());
                prize.Add(deck1.DealTopCard());
                plays.Add($"-- WAR!");
                return HandResult.War;
            }
        }

        private Deck CreateDeck()
        {
            List<PlayingCard> cards = new List<PlayingCard>();

            var suites = Enum.GetValues(typeof(Suite));
            foreach (Suite suite in suites)
            {
                for (int i = 2; i <= 14; i++)
                {
                    cards.Add(new PlayingCard(i, suite));
                }
            }

            return new Deck { Cards = cards };
        }
    }

    public enum HandResult
    {
        Player1Win,
        Player2Win,
        War
    }
}
