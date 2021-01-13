using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PWEB_TP.Startup))]
namespace PWEB_TP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
