using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PassionProject1N01659860.Startup))]
namespace PassionProject1N01659860
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
