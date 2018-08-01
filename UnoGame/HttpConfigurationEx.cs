using System.Web.Http;
using DryIoc;
using DryIoc.WebApi;
using UnoGame.Services;

namespace UnoGame
{
    public static class HttpConfigurationEx
    {
        public static HttpConfiguration ConfigureRouting(this HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            return config;
        }

        public static HttpConfiguration ConfigureDependencyInjection(this HttpConfiguration config)
        {
            var container = new Container();

            container.Register<IGameService, GameService>(Reuse.Singleton);
            container.Register<ICardDeskService, CardDeskService>(Reuse.Singleton);
            container.Register<IPlayerService, PlayerService>(Reuse.Singleton);

            container.WithWebApi(config);
            return config;
        }
    }

}