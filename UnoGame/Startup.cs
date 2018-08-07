using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace UnoGame
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration()
                .ConfigureRouting()
                .ConfigureDependencyInjection();

            var physicalFileSystem = new PhysicalFileSystem("./www");

            var fileServerOptions = new FileServerOptions
            {
                EnableDefaultFiles = true,
                RequestPath = PathString.Empty,
                FileSystem = physicalFileSystem,
                DefaultFilesOptions = { DefaultFileNames = new[] { "home.html" } },
                StaticFileOptions =
                {
                    ServeUnknownFileTypes = true,
                    FileSystem = physicalFileSystem
                }
            };

            appBuilder.UseFileServer(fileServerOptions);
            appBuilder.UseWebApi(config);
            appBuilder.MapSignalR();
        }
    }
}
