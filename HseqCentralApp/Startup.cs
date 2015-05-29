using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HseqCentralApp.Startup))]
namespace HseqCentralApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
