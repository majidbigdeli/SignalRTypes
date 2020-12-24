using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Septa.AspNetCore.SignalRTypes
{
    public class SignalrTypeMiddleware
    {
        readonly RequestDelegate _next;
        private readonly SignalrTypeOptions options;


        public SignalrTypeMiddleware(RequestDelegate next,
            SignalrTypeOptions options)
        {
            _next = next;
            this.options = options;

        }

        public async Task Invoke(HttpContext httpContext)
        {

            var httpMethod = httpContext.Request.Method;
            var path = httpContext.Request.Path.Value;
            if (httpMethod == "GET" && Regex.IsMatch(path, $"^/?{Regex.Escape(options.RoutePath)}/?$", RegexOptions.IgnoreCase))
            {
                //var indexUrl = httpContext.Request.GetEncodedUrl().TrimEnd('/') + "/index.html";

                //  RespondWithRedirect(httpContext.Response, indexUrl);

                var settings = new SignalrTypeGeneratorSettings();
                var generator = new SignalrTypeGenerator(settings);

                var document = await generator.GenerateForHubsAsync(options.HubServices);

                var json = document.ToJson();

                httpContext.Response.StatusCode = 200;
                await httpContext.Response.WriteAsync(json, Encoding.UTF8);

                return;
            }

            await _next(httpContext);

            //await context.Response.WriteAsync(_version);

            //we're all done, so don't invoke next middleware
        }

    }

}
