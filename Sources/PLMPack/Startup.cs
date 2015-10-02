using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PLMPack.Startup))]
namespace PLMPack
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
