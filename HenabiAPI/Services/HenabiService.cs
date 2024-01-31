using HenabiAPI.Data;
using HenabiAPI.DTOs;
using HenabiAPI.Interfaces;
using HenabiAPI.Models;
using HenabiAPI.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HenabiAPI.Services
{
    public class HenabiService : IHenabiService
    {
        private HenabiDBContext _context { get; set; }
        private Random rand { get; set; }

        public HenabiService(HenabiDBContext context)
        {
            _context = context;
            rand = new Random();
        }

        public List<Card> GetAllCards()
        {
            return _context.Cards.ToList();
        }

        public Card GetCardById(int cardId)
        {
            return _context.Cards.SingleOrDefault(x => x.Id == cardId);
        }

        public Player CreatePlayer(Player playerToCreate)
        {
            if (string.IsNullOrWhiteSpace(playerToCreate.Username) || string.IsNullOrWhiteSpace(playerToCreate.Name))
            {
                throw new Exception("Please provide a valid name and username");
            }
            if (_context.Players.Any(x => x.Username.ToLower() == playerToCreate.Username.ToLower()))
            {
                throw new Exception($"The username {playerToCreate.Username} is already taken");
            }
            _context.Add(playerToCreate);
            _context.SaveChanges();
            return _context.Players.SingleOrDefault(x => x.Username.ToLower() == playerToCreate.Username.ToLower());
        }

        public Game CreateGame(ICollection<int> playerIds)
        {
            if (playerIds.Count < 2 || playerIds.Count > 5)
            {
                throw new Exception("Games must have between 2 and 5 players");
            };
            Game gameToCreate = new Game()
            {
                Fuses = 3,
                Hints = 8,
                Players = new List<Player>()
            };

            //Add players
            foreach (int playerId in playerIds)
            {
                Player playerToAdd = _context.Players.SingleOrDefault(x => x.Id == playerId);
                if (playerToAdd == null)
                {
                    throw new Exception("Pleayer could not be found");
                }
                gameToCreate.Players.Add(playerToAdd);

            }

            _context.Games.Add(gameToCreate);

            //Add cards to the deck
            List<GameCard> gameCards = new List<GameCard>();
            foreach (Card currentCard in _context.Cards)
            {
                gameCards.Add(
                    new GameCard()
                    {
                        Game = gameToCreate,
                        Card = currentCard,
                        Position = CardPosition.Undrawn
                    });
            }

            List<GameCard> undrawnCards = gameCards.Where(x => x.Position == CardPosition.Undrawn).ToList();

            //Distribute cards to players
            int numberOfCardsInHand;
            if (playerIds.Count < 4)
            {
                numberOfCardsInHand = 5;
            }
            else
            {
                numberOfCardsInHand = 4;
            }

            Random rand = new Random();
            foreach (Player player in gameToCreate.Players)
            {
                for (int idx = 0; idx < numberOfCardsInHand; idx++)
                {
                    GameCard drawnGameCard = undrawnCards[rand.Next(0, undrawnCards.Count)];
                    undrawnCards.Remove(drawnGameCard);
                    drawnGameCard.Position = CardPosition.InHand;
                    drawnGameCard.Player = player;
                }
            }

            _context.GameCards.AddRange(gameCards);
            _context.SaveChanges();
            return gameToCreate;
        }

        public List<Game> GetAllGames()
        {
            List<Game> games = _context.Games.Include(x => x.Players).Include(x => x.Cards).ThenInclude(x => x.Card).ToList();
            return games;
        }

        public void DeleteGame(int gameId)
        {
            List<GameCard> cardsToDelete = _context.GameCards.Where(x => x.Game.Id == gameId).ToList();
            _context.GameCards.RemoveRange(cardsToDelete);
            Game gameToDelete = _context.Games.Include(x => x.Players).SingleOrDefault(x => x.Id == gameId);
            //Players need to be removed before deleting to avoid FK restraint issue
            gameToDelete.Players.RemoveRange(0, gameToDelete.Players.Count);
            _context.Games.Remove(gameToDelete);
            _context.SaveChanges();
        }

        public List<CardDTO> GetHandById(int playerId, int gameId)
        {
            return _context.GameCards.Where(x => x.Player.Id == playerId && x.Game.Id == gameId).Select(x => x.Card.MapToDTO()).ToList();
        }

        public HintDTO GiveHint(HintDTO hintGiven)
        {
            Hint hintToSave = new Hint()
            {
                ColorHint = hintGiven.ColorHint,
                NumberHint = hintGiven.NumberHint,
                TimeGiven = DateTime.Now,
                Cards = new List<Card>()
            };

            Player receivingPlayer = _context.Players.SingleOrDefault(x => x.Id == hintGiven.RecievingPlayerId);
            Player givingPlayer = _context.Players.SingleOrDefault(x => x.Id == hintGiven.GivingPlayerId);

            if (receivingPlayer is null || givingPlayer is null)
            {
                throw new Exception("Player does not exist");
            }

            if (receivingPlayer == givingPlayer)
            {
                throw new Exception("Player may not give self hint");
            }

            hintToSave.ReceivingPlayer = receivingPlayer;
            hintToSave.GivingPlayer = givingPlayer;

            Game game = _context.Games.SingleOrDefault(x => x.Id == hintGiven.GameId);

            if (game is null)
            {
                throw new Exception("Game does not exist");
            }

            if (game.Hints <= 0)
            {
                throw new Exception("No hints are remaining");
            }

            game.Hints -= 1;

            List<Card> receivingPlayersHand = _context.GameCards.Include(x => x.Card)
                                                                    .Where(x => x.Game.Id == game.Id && 
                                                                                x.Position == CardPosition.InHand && 
                                                                                x.Player.Id == receivingPlayer.Id)
                                                                    .Select(x => x.Card)
                                                                    .ToList();

            if (hintGiven.ColorHint is not null)
            {
                foreach (Card currentCard in receivingPlayersHand)
                {
                    if (hintGiven.ColorHint == currentCard.Color)
                    {
                        hintToSave.Cards.Add(currentCard);
                    }
                }
            }

            if (hintGiven.NumberHint is not null)
            {
                foreach (Card currentCard in receivingPlayersHand)
                {
                    if (hintGiven.NumberHint == currentCard.Number)
                    {
                        hintToSave.Cards.Add(currentCard);
                    }
                }
            }

            if (hintToSave.Cards.Count == 0)
            {
                throw new Exception("A valid hint must point out at least 1 valid card");
            }

            _context.Hints.Add(hintToSave);
            _context.SaveChanges();

            return new HintDTO()
            {
                GivingPlayerId = hintToSave.GivingPlayer.Id,
                RecievingPlayerId = hintToSave.ReceivingPlayer.Id,
                ColorHint = hintToSave.ColorHint,
                NumberHint = hintToSave.NumberHint,
                CardIds = hintToSave.Cards.Select(x => x.Id).ToList(),
                GameId = hintToSave.Id
            };
        }

        public CardDTO DiscardCard(CardDTO cardToDiscard)
        {
            GameCard gameCardToDiscard = _context.GameCards.Include(x => x.Player)
                                                           .Include(x => x.Card)
                                                           .SingleOrDefault(x => x.Game.Id == cardToDiscard.GameId &&
                                                                                 x.Card.Id == cardToDiscard.Id);

            if (gameCardToDiscard.Position != CardPosition.InHand)
            {
                throw new Exception("Card cannot be discarded unless it is currently in a player's hand");
            }

            gameCardToDiscard.Position = CardPosition.Discarded;
            Player currentPlayer = gameCardToDiscard.Player;
            gameCardToDiscard.Player = null;

            Game currentGame = _context.Games.Find(cardToDiscard.GameId);

            if (currentGame.Hints < 8)
            {
                currentGame.Hints += 1;
            }

            int totalUndrawnCards = _context.GameCards.Where(x => x.Game.Id == currentGame.Id && x.Position == CardPosition.Undrawn).Count();

            if (totalUndrawnCards == 0)
            {
                _context.SaveChanges();
                return null;
            }

            GameCard drawnCard = _context.GameCards.Include(x => x.Card).Where(x => x.Game.Id == currentGame.Id && x.Position == CardPosition.Undrawn).ToList().ElementAt(rand.Next(0, totalUndrawnCards));

            drawnCard.Position = CardPosition.InHand;
            drawnCard.Player = currentPlayer;

            _context.SaveChanges();

            CardDTO cardToReturn = drawnCard.Card.MapToDTO();
            cardToReturn.GameId = gameCardToDiscard.Game.Id;

            return cardToReturn;
        }

        public CardDTO PlayCard(CardDTO cardToPlayDTO)
        {
            GameCard gameCardToPlay = _context.GameCards.Include(x => x.Player)
                                               .Include(x => x.Card)
                                               .SingleOrDefault(x => x.Game.Id == cardToPlayDTO.GameId &&
                                                                     x.Card.Id == cardToPlayDTO.Id);

            if (gameCardToPlay.Position != CardPosition.InHand)
            {
                throw new Exception("Card cannot be played unless it is currently in a player's hand");
            }

            Game currentGame = _context.Games.Find(cardToPlayDTO.GameId);
            Player currentPlayer = gameCardToPlay.Player;

            var lastCardPlayed = _context.GameCards.Include(x => x.Card)
                                                   .Where(x => x.Game.Id == cardToPlayDTO.GameId &&
                                                               x.Position == CardPosition.Played &&
                                                               x.Card.Color == gameCardToPlay.Card.Color);

            if ((lastCardPlayed.Count() == 0 && gameCardToPlay.Card.Number != 1) &&
                !lastCardPlayed.Any(x => x.Card.Number == gameCardToPlay.Card.Number - 1))
            {
                currentGame.Fuses -= 1;
                gameCardToPlay.Position = CardPosition.Discarded;
            }
            else
            {
                gameCardToPlay.Position = CardPosition.Played;
                if (gameCardToPlay.Card.Number == 5 && currentGame.Hints < 8)
                {
                    currentGame.Hints += 1;
                }
            }

            gameCardToPlay.Player = null;

            int totalUndrawnCards = _context.GameCards.Where(x => x.Game.Id == currentGame.Id && x.Position == CardPosition.Undrawn).Count();

            if (totalUndrawnCards == 0)
            {
                _context.SaveChanges();
                return null;
            }

            GameCard drawnCard = _context.GameCards.Include(x => x.Card).Where(x => x.Game.Id == currentGame.Id && x.Position == CardPosition.Undrawn).ToList().ElementAt(rand.Next(0, totalUndrawnCards));

            drawnCard.Position = CardPosition.InHand;
            drawnCard.Player = currentPlayer;

            _context.SaveChanges();

            CardDTO cardToReturn = drawnCard.Card.MapToDTO();
            cardToReturn.GameId = gameCardToPlay.Game.Id;

            return cardToReturn;
        }

        public List<CardDTO> GetPlayedCards(int gameId)
        {
            var playedGameCards = _context.GameCards.Include(x => x.Card)
                                                    .Where(x => x.Game.Id == gameId &&
                                                                x.Position == CardPosition.Played);

            List<GameCard> maxPlayedGameCards = new List<GameCard>();

            foreach (CardColor currentColor in Enum.GetValues(typeof(CardColor)))
            {
                var currentColorPlayed = playedGameCards.Where(x => x.Card.Color == currentColor);
                if (currentColorPlayed?.Count() == 0)
                {
                    continue;
                }

                int maxPlayed = currentColorPlayed.Max(x => x.Card.Number);
                maxPlayedGameCards.Add(currentColorPlayed.SingleOrDefault(x => x.Card.Number == maxPlayed));
            }

            List<CardDTO> cardsPlayed = new List<CardDTO>();

            foreach (GameCard currentGameCard in maxPlayedGameCards)
            {
                CardDTO currentCardDTO = currentGameCard.Card.MapToDTO();
                currentCardDTO.GameId = gameId;
                cardsPlayed.Add(currentCardDTO);
            }

            return cardsPlayed;
        }
    }
}
