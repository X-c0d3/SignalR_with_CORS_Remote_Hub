using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Cors;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Ninject;
using Ninject.Activation;
using Ninject.Web.Common;
using Owin;
using SignalR_webapi;
using SignalR_webapi.App_Start;
using SignalR_webapi.Hubs;
using SignalR_webapi.Services;

[assembly: OwinStartupAttribute(typeof(Startup))]
namespace SignalR_webapi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            RegisterSignalR(app);
        }

        public static void RegisterSignalR(IAppBuilder app)
        {
            var kernel = new StandardKernel();
            var resolver = new NinjectSignalRDependencyResolver(kernel);

            NinjectWebCommon.RegisterServices(kernel);

            kernel.Bind(typeof(IHubConnectionContext<dynamic>)).ToMethod(context => resolver.Resolve<IConnectionManager>()
             .GetHubContext<MyHub>().Clients).WhenInjectedInto<MyServices>();


            var httpConfig = new HttpConfiguration();
            //change this configuration as you want.
            var cors = new EnableCorsAttribute("http://localhost:54336", "*", "*");
            httpConfig.EnableCors(cors);

            app.Map("/signalr", map =>
            {

                var corsOption = new CorsOptions
                {
                    PolicyProvider = new CorsPolicyProvider
                    {
                        PolicyResolver = context =>
                        {
                            var policy = new CorsPolicy { AllowAnyHeader = true, AllowAnyMethod = true, SupportsCredentials = true };
                            // Only allow CORS requests from the trusted domains.
                            cors.Origins.ToList().ForEach(o => policy.Origins.Add(o));
                            return Task.FromResult(policy);
                        }
                    }
                };

                //map.UseCors(CorsOptions.AllowAll); //If you don't care about security 
                map.UseCors(corsOption);

                var config = new HubConfiguration
                {
                    EnableDetailedErrors = true,
                    EnableJavaScriptProxies = true,
                    EnableJSONP = true,
                    Resolver = resolver
                };
                map.RunSignalR(config);
            });

            //app.UseWebApi(httpConfig);
        }
    }
}