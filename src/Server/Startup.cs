using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Peachpie.Web;

namespace Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            if(env.IsStaging() || env.IsProduction())
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseSession();

            // Use re-write to redirect source to wwwroot folder for php script files
            // UsePhp cannot translate the base path to a sub folder of the project file.
            var rewriteOptions = new RewriteOptions()
                //.AddRewrite(@"^.*?/[^/.]*(?:/([#?].*))?$", "wwwroot/$0", skipRemainingRules: true)  // Directory
                //.AddRewrite(@"^(?:[^\?]+)(?:\.php)(?:.*)", "wwwroot/$0", skipRemainingRules: true); // IsFile = .php
                .Add(new PhpRewriteRule(".php", "wwwroot", skipRemainingRules: true));
                // .Add((context) => {
                //     var request = context.HttpContext.Request;

                //     context.Result = RuleResult.SkipRemainingRules;
                //     request.Path = request.Path;
                //     request.QueryString = new QueryString("");
                // });
                
            app.UseRewriter(rewriteOptions);

            // app.Run(context => context.Response.WriteAsync(
            //     $"Rewritten or Redirected Url: " +
            //     $"{context.Request.Path + context.Request.QueryString}"));

            var phpOptions = new PhpRequestOptions(scriptAssemblyName: "Website");
            phpOptions.RootPath = Path.Combine(AppContext.BaseDirectory);
             app.UsePhp(phpOptions);
             app.UseDefaultFiles();

            Console.WriteLine($"Content Root Path: {Path.Combine(AppContext.BaseDirectory, "wwwroot")}");
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(AppContext.BaseDirectory, "wwwroot")),
                    //ContentTypeProvider = MimeTypes.GetKnownFileTypes()
                    ContentTypeProvider = new FileExtensionContentTypeProvider()
            });
            app.UseCookiePolicy();
        }
    }
}
