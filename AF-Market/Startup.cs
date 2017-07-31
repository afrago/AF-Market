using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AF_Market.Startup))]
namespace AF_Market
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
