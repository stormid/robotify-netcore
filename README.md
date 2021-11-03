# ![Robotify.AspNetCore](https://raw.githubusercontent.com/stormid/robotify.aspnetcore/master/docs/img/robot.png) Robotify.AspNetCore

Robotify - robots.txt middleware for .NET core (.netstandard2.0)

## Installation

Install package via NuGet:

```powershell
Install-Package Robotify.AspNetCore
```

## Configuration

Add Robotify to the middleware pipeline like any other:

Add to the `ConfigureServices` method to ensure the middleware and configuration are added to the services collection

```c#
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.AddRobotify();

            services.AddMvc();
        }
```

To configure what robots directives you want to appear in your robots file you can create and assign a `IRobotifyRobotGroupProvider`.  Multiple providers can be created and registered in the standard DI container, each provider will then be iterated when generating the contents of the robots file.

A `IRobotifyRobotGroupProvider` contains a single method that returns a list of `RobotGroup`, below is an example of the `RobotifyDisallowAllRobotGroupProvider`:

```c#
    public class YourOwnProvider : IRobotifyRobotGroupProvider
    {
        public IEnumerable<RobotGroup> Get()
        {
            yield return new RobotGroup() 
			{			
				UserAgent = "something",
				Disallow = new[] { "/" }
			}
        }
    }
```

To register a provider, use the `.AddRobotGroupProvider<TProvider>()` extension method when adding robotify:

```c#
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRobotify(c => c
                .AddDisallowAllRobotGroupProvider()
                .AddRobotGroupsFromAppSettings()
                .AddRobotGroupProvider<YourOwnProvider>()
            );
        }
```


Then use the middleware as part of the request pipeline, I would suggest adding it at least after the `.UseStaticFiles()` middleware, then if you have a static file robots.txt this will be used instead.

```c#
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseStaticFiles();
            app.UseRobotify();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
```

Robotify can be configured within the `.UseRobotify()` method or via json configuration:

```json
{
  "Robotify": {
    "Enabled": true,
    "SitemapUrl": "https://www.example.com/sitemap.xml",
    "CrawlDelay": 10,
    "Groups": [
      {
        "UserAgent": "*",
        "Disallow": "/"
      },
      {
        "UserAgent": "Googlebot",
        "Disallow": ["/admin"]
      },
      {
        "UserAgent": "AnotherBot",
        "Allow": ["/search"]
      }
    ]
  }
}
```

Json configuration and code configuration will be merged at boot time.  If no groups are specified a default deny all ill be added.
