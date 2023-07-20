using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RMS.Web.Startup))]
namespace RMS.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
