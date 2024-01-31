using HenabiAPI.DTOs;
using HenabiAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HenabiAPI.Interfaces
{
    public interface IHenabiService
    {
        public List<Card> GetAllCards();
        public Card GetCardById(int cardId);
        public Player CreatePlayer(Player playerToCreate);
        public Game CreateGame(ICollection<int> playerIds);
        public List<Game> GetAllGames();
        public void DeleteGame(int gameId);
        public List<CardDTO> GetHandById(int playerId, int gameId);
        public HintDTO GiveHint(HintDTO hintGiven);
        public CardDTO DiscardCard(CardDTO cardToDiscard);
        public CardDTO PlayCard(CardDTO cardToPlay);
        public List<CardDTO> GetPlayedCards(int gameId);
    }
}
