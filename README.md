# ![Robotify.AspNetCore](https://raw.githubusercontent.com/stormid/robotify.aspnetcore/master/docs/img/robot.png) Robotify.AspNetCore

Robotify - robots.txt middleware for .NET core

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

            services.AddRobotify(Configuration);

            services.AddMvc();
        }
```

Then use the middleware as part of the request pipeline, I would suggest adding it at least after the `.UseStaticFiles()` middleware, then if you have a static file robots.txt this will be used instead.

```c#
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseRobotify();
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
        "Disallow": "/admin"
      },
      {
        "UserAgent": "AnotherBot",
        "Allow": "/search"
      }
    ]
  }
}
```

Json configuration and code configuration will be merged at boot time.  If no groups are specified a default deny all ill be added.