using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Robotify.AspNetCore
{
    public class RobotifyMiddleware : IMiddleware
    {
        private readonly IRobotifyContentWriter writer;

        public RobotifyMiddleware(IRobotifyContentWriter writer)
        {
            this.writer = writer;
        }
        
        //public async Task Invoke(HttpContext httpContext, IRobotifyContentWriter writer)
        //{
        //    var content = writer.Write();
        //    httpContext.Response.ContentType = "text/plain";
        //    await httpContext.Response.WriteAsync(content);
        //}

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var content = writer.Write();
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(content);
        }
    }
}