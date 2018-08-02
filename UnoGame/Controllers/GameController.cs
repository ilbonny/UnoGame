using System.Net;
using System.Net.Http;
using System.Web.Http;
using UnoGame.Models;
using UnoGame.Services;

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

        [Route("start")]
        [HttpGet]
        public HttpResponseMessage CreateDesk()
        {
            var game = _gameService.Start();
            return Request.CreateResponse(HttpStatusCode.OK, game);
        }

        [Route("playerturn")]
        [HttpPost]
        public HttpResponseMessage PlayerTurnExecute(PlayerTurn playerTurn)
        {
            var game = _gameService.PlayerTurnExecute(playerTurn);
            return Request.CreateResponse(HttpStatusCode.OK, game);
        }

        [Route("drawDeck")]
        [HttpGet]
        public HttpResponseMessage DrawDeck()
        {
            var game = _gameService.DrawDeck();
            return Request.CreateResponse(HttpStatusCode.OK, game);
        }
    }
}
