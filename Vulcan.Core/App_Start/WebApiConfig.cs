using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.Application;
using Vulcan.Core.Auth;
using Vulcan.Core.Auth.AzureAuthProvider;
using Vulcan.Core.DataAccess.Formatters;

namespace Vulcan.Core
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config
                .EnableSwagger(c => c.SingleApiVersion("docs", "A title for your API"))
                .EnableSwaggerUi(c => c.InjectJavaScript(typeof(WebApiConfig).Assembly, "Vulcan.Core.Auth.SwashBuckleExtensions.onComplete.js"));

            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());

            GlobalConfiguration.Configuration.Formatters.Add(new DynamicXmlFormatter());
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);


            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            AuthenticationProviderManager.Register(new AzureAuthentication(ConfigurationManager.AppSettings["AzureClientId"], ConfigurationManager.AppSettings["AzureClientSecret"], null));
        }
    }
}