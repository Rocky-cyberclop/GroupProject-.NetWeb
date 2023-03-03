using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GroupProject.Startup))]
namespace GroupProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

        }
    }
}
