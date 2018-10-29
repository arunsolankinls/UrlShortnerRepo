using Microsoft.Owin;
using Owin;

//[assembly: OwinStartupAttribute(typeof(UrlShortnerClient.Startup))]
[assembly: OwinStartupAttribute("UrlShortnerClient", typeof(UrlShortnerClient.Startup))]
namespace UrlShortnerClient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
