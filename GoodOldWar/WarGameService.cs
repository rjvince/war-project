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
                Player1 = game.players[0],
                Player2 = game.players[1]
            };

            return dto;
        }

        public GameStateDTO GetGame(int gameId)
        {
            Game game = _context.GetGameEntireState(gameId);

            GameStateDTO dto = new GameStateDTO
            {
                GameId = game.Id,
                GameOver = false,
                LastPlay = new List<string>(),
                Player1 = game.players[0],
                Player2 = game.players[1]
            };

            return dto;
        }

        public List<Game> GetNRecentGames(int v)
        {
            return _context.Games.Include(g => g.players).OrderByDescending(g => g.Id).Take(5).ToList();
        }

        public GameStateDTO PlayNextHand(int gameId)
        {
            Game game = _context.GetGameEntireState(gameId);

            List<string> plays = new List<string>();
            List<PlayingCard> prize = new List<PlayingCard>();

            HandResult handResult = DecideHandResult(game.players[0], game.players[1], prize, plays);

            while (handResult == HandResult.War && !game.Over())
            {
                handResult = DecideHandResult(game.players[0], game.players[1], prize, plays);
            }

            if(game.Over())
            {
                plays.Add("Game Over");
                if (game.players[0].Deck.IsEmpty() && game.players[1].Deck.IsEmpty())
                {
                    plays.Add("Both players are out of cards?! It's... a DRAW!");
                }
                else
                {
                    plays.Add($"{ (game.players[0].Deck.IsEmpty() ? game.players[1].Name : game.players[0].Name) } wins!");
                }
            }

            _context.SaveChanges();

            GameStateDTO dto = new GameStateDTO
            {
                GameId = game.Id,
                GameOver = (game.players[0].Deck.Count() == 0 || game.players[1].Deck.Count() == 0),
                LastPlay = plays,
                Player1 = game.players[0],
                Player2 = game.players[1]
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
