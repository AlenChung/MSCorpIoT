using System.Linq;
using System.Web;
using System.Web.Http;

namespace MSCorp.FirstResponse.WebApiDemo
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
