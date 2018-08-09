using System.Reflection;
using System.Web.Http;
using DryIoc;
using DryIoc.SignalR;
using DryIoc.WebApi;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using UnoGame.Core.Services;

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
            container.Register<IRuleService, RuleService>(Reuse.Singleton);
            container.Register<IUserService, UserService>(Reuse.Singleton);
            container.Register<IAutomaticPlayerService, AutomaticPlayerService>(Reuse.Transient);

            var hubAssemblies = new[] { Assembly.GetAssembly(typeof(GameHub)) };
            container.WithSignalR(hubAssemblies);
            container.WithWebApi(config);
            return config;
        }
    }

}