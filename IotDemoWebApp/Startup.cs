using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IotDemoWebApp.Startup))]
namespace IotDemoWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
