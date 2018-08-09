using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UnoGame.Core.Models;
using UnoGame.Core.Services;

namespace UnoGame.Controllers
{
    [RoutePrefix("api/game")]
    public class GameController : ApiController
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [Route("playerturn")]
        [HttpPost]
        public HttpResponseMessage PlayerTurnExecute(PlayerTurn playerTurn)
        {
            _gameService.PlayerTurnExecute(playerTurn);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("drawDeck")]
        [HttpGet]
        public HttpResponseMessage DrawDeck(Guid gameId)
        {
            _gameService.DrawDeck(gameId);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

    }
}
