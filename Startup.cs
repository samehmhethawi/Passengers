using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Passengers.Startup))]
namespace Passengers
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}