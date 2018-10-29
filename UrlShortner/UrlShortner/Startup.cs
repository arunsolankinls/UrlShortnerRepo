using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute("UrlShortner", typeof(UrlShortner.Startup))]
//[assembly: OwinStartupAttribute(typeof(UrlShortner.Startup))]
namespace UrlShortner
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
