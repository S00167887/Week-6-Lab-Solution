using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Week6.MVC.Startup))]
namespace Week6.MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
