using HenabiAPI.DTOs;
using HenabiAPI.Interfaces;
using HenabiAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HenabiAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HenabiController : ControllerBase
    {
        private IHenabiService henabiService { get; set; }
        public HenabiController(IHenabiService henabiService)
        {
            this.henabiService = henabiService;
        }

        [HttpGet]
        public List<Card> GetAllCards()
        {
            return henabiService.GetAllCards();
        }

        [HttpGet]
        [Route("{id}")]
        public Card GetCardById(int id)
        {
            Card test = henabiService.GetCardById(id);
            return test;
        }

        [HttpPost]
        [Route("player/new")]
        public Player CreateNewPlayer([FromBody] Player playerToCreate)
        {
            return henabiService.CreatePlayer(playerToCreate);
        }

        [HttpPost]
        [Route("game/new")]
        public Game BeginNewGame(ICollection<int> playerIds)
        {
            return henabiService.CreateGame(playerIds);
        }

        [HttpGet]
        [Route("games")]
        public List<Game> GetAllGames()
        {
            return henabiService.GetAllGames();
        }

        [HttpDelete]
        [Route("game/delete/{id}")]
        public void DeleteGameById(int id)
        {
            henabiService.DeleteGame(id);
        }

        [HttpGet]
        [Route("hand/{playerId}/{gameId}")]
        public List<CardDTO> GetCurrentHandById(int playerId, int gameId)
        {
            return henabiService.GetHandById(playerId, gameId);
        }

        [HttpPost]
        [Route("hint")]
        public HintDTO GiveHint(HintDTO hintGiven)
        {
            return henabiService.GiveHint(hintGiven);
        }

        [HttpPut]
        [Route("discard")]
        public CardDTO DiscardCard(CardDTO cardToDiscard)
        {
            return henabiService.DiscardCard(cardToDiscard);
        }

        [HttpPut]
        [Route("play")]
        public CardDTO PlayCard(CardDTO cardToPlay)
        {
            return henabiService.PlayCard(cardToPlay);
        }

        [HttpGet]
        [Route("played/{gameId}")]
        public List<CardDTO> playedCards(int gameId)
        {
            return henabiService.GetPlayedCards(gameId);
        }
    }
}
