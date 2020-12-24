using Microsoft.AspNetCore.Builder;
using System;

namespace Microsoft.AspNetCore.SignalRTypes
{
    public static class SignalrTypeExtensions
    {
        public static IApplicationBuilder UseSignalrType(this IApplicationBuilder app, Action<SignalrTypeOptions> setupAction)
        {
            var options = new SignalrTypeOptions();
            setupAction(options);
            return app.UseMiddleware<SignalrTypeMiddleware>(options);
        }
    }

}
