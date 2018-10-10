using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Robotify.AspNetCore
{
    public class RobotifyMiddleware
    {
        public RobotifyMiddleware(RequestDelegate next)
        {
            
        }
        
        public async Task Invoke(HttpContext httpContext, IRobotifyContentWriter writer)
        {
            var content = writer.Write();
            httpContext.Response.ContentType = "text/plain";
            await httpContext.Response.WriteAsync(content);
        }
    }
}