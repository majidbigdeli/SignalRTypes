using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Septa.AspNetCore.SignalRTypes
{
    public class SignalrTypeMiddleware
    {
        readonly RequestDelegate _next;
        private readonly SignalrTypeOptions options;

        private readonly JsonSerializerSettings _jsonSerializerSettings;


        public SignalrTypeMiddleware(RequestDelegate next,
            IOptions<NewtonsoftJsonHubProtocolOptions> jsonOptions,
            SignalrTypeOptions options)
        {
            _next = next;
            _jsonSerializerSettings = jsonOptions.Value.PayloadSerializerSettings;
            this.options = options;
        }

        public async Task Invoke(HttpContext httpContext, ISignalRTypesBuilder signalRTypesBuilder)
        {

            var httpMethod = httpContext.Request.Method;

            var path = httpContext.Request.Path.Value;
            if (httpMethod == "GET" && Regex.IsMatch(path, $"^/?{Regex.Escape(options.RoutePath)}/?$", RegexOptions.IgnoreCase))
            {

                var settings = new SignalrTypeGeneratorSettings(_jsonSerializerSettings);
                var generator = new SignalrTypeGenerator(settings);

                var document = await generator.GenerateForHubsAsync(options.Hubs, signalRTypesBuilder);

                var json = document.ToJson();

                httpContext.Response.StatusCode = StatusCodes.Status200OK;
                httpContext.Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                await httpContext.Response.WriteAsync(json, Encoding.UTF8);

                return;
            }

            await _next(httpContext);

            //await context.Response.WriteAsync(_version);

            //we're all done, so don't invoke next middleware
        }

    }

}
