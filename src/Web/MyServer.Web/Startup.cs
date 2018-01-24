namespace MyServer.Web
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;

    using Hangfire;
    using Hangfire.MemoryStorage;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Net.Http.Headers;

    using MyServer.Data;
    using MyServer.Data.Common;
    using MyServer.Data.Models;
    using MyServer.Services.Content;
    using MyServer.Services.ImageGallery;
    using MyServer.Services.Mappings;
    using MyServer.Services.SmartHome;
    using MyServer.Services.SmartHome.Config;
    using MyServer.Services.Users;
    using MyServer.ViewComponents.Common.Components.Social.Controllers;
    using MyServer.ViewComponents.ImageGallery.Components.LatestAddedAlbums.Controllers;
    using MyServer.Web.Helpers;
    using MyServer.Web.Migrations;

    using Newtonsoft.Json.Serialization;

    public class Startup
    {
        public static IServiceScopeFactory scopeFactory = null;

        public static string WwwPath = string.Empty;

        public static string WwwRootPath = string.Empty;

        private readonly IHostingEnvironment hostingEnvironment;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            this.Configuration = configuration;
            this.hostingEnvironment = env;
        }

        public static IStringLocalizer<SharedResource> SharedLocalizer { get; private set; }

        public IConfiguration Configuration { get; }

        public void Configure(
            IApplicationBuilder app,
            IServiceScopeFactory scopeFactoryLocal,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            UserManager<User> userManager,
            IStringLocalizer<SharedResource> sharedLocalizer,
            IHomeTemparatures homeTemparatures)
        {
            var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("bg-BG") };
            SharedLocalizer = sharedLocalizer;

            app.UseRequestLocalization(
                new RequestLocalizationOptions
                    {
                        DefaultRequestCulture = new RequestCulture("en-US"),

                        // Formatting numbers, dates, etc.
                        SupportedCultures = supportedCultures,

                        // UI strings that we have localized.
                        SupportedUICultures = supportedCultures
                    });

            var pathHelper = new PathHelper(env, userManager);
            scopeFactory = scopeFactoryLocal;

            scopeFactory.SeedData();

            app.UseStaticFiles(
                new StaticFileOptions()
                    {
                        OnPrepareResponse = (context) =>
                            {
                                var headers = context.Context.Response.GetTypedHeaders();
                                headers.CacheControl =
                                    new CacheControlHeaderValue()
                                        {
                                            MaxAge = TimeSpan.FromDays(
                                                100)
                                        };
                            }
                    });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error?statusCode=500");
                app.UseStatusCodePagesWithReExecute("/Error?statusCode={0}");
            }

            app.UseAuthentication();
            app.UseHangfireDashboard();
            app.UseHangfireServer();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
            app.UseSession();
            app.UseMvc(
                routes =>
                    {
                        // Areas support
                        routes.MapRoute(name: "areaRoute", template: "{area:exists}/{controller}/{action=Index}/{id?}");

                        routes.MapRoute(name: "default", template: "{controller}/{action=Index}/{id?}");
                    });

            // Configure Kendo UI
            app.UseKendo(env);

            var autoMapperConfig = new AutoMapperConfig();
            autoMapperConfig.Execute(
                new List<Assembly>
                    {
                        Assembly.GetEntryAssembly(),
                        typeof(LatestAddedAlbumsViewComponent).GetTypeInfo().Assembly,
                        typeof(SocialViewComponent).GetTypeInfo().Assembly
                    });

            RecurringJob.AddOrUpdate(() => homeTemparatures.Update(), Cron.MinuteInterval(5));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MyServerDbContext>(
                options => options.UseSqlServer(this.Configuration.GetConnectionString("MyServerDb")));

            services.AddOptions();
            services.AddScoped<DbContext, MyServerDbContext>();
            services.Add(ServiceDescriptor.Scoped(typeof(IRepository<,>), typeof(Repository<,>)));
            services.Add(ServiceDescriptor.Scoped(typeof(IRepository<>), typeof(Repository<>)));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IAlbumService, AlbumService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IContentService, ContentService>();

            services.AddHangfire(c => c.UseMemoryStorage());

            var appSettings = this.Configuration.GetSection("SmartHome:Temperatures");
            services.Configure<List<TemperatureConfig>>(appSettings);

            services.AddSingleton<IHomeTemparatures, HomeTemparatures>();

            services.AddIdentity<User, IdentityRole>(
                o =>
                    {
                        o.Password.RequireDigit = false;
                        o.Password.RequireLowercase = false;
                        o.Password.RequireUppercase = false;
                        o.Password.RequireNonAlphanumeric = false;
                        o.Password.RequiredLength = 6;
                    }).AddEntityFrameworkStores<MyServerDbContext>().AddDefaultTokenProviders();

            services.AddAuthentication().AddFacebook(
                facebookOptions =>
                    {
                        facebookOptions.AppId = "521558431365642";
                        facebookOptions.AppSecret = "af05f969147e202f1e8c76c4cfd31a79";
                    });

            services.AddAuthentication().AddGoogle(
                googleOptions =>
                    {
                        googleOptions.ClientId =
                            "18361776506-dphsr6a6eamnjcb5b144j5offcn3tndq.apps.googleusercontent.com";
                        googleOptions.ClientSecret = "gul1QEzDbz2-bq3tXi4r8hLI";
                    });

            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddMemoryCache();

            services.AddCloudscribeNavigation(this.Configuration.GetSection("NavigationOptions"));

            services.Configure<RazorViewEngineOptions>(
                o => { o.ViewLocationExpanders.Add(new ViewLocationExpander()); });

            if (this.hostingEnvironment.IsDevelopment())
            {
                services.AddMvc()
                    .AddJsonOptions(
                        options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
                    .AddRazorPagesOptions(
                        options =>
                            {
                                options.Conventions.AuthorizeFolder("/Account/Manage");
                                options.Conventions.AuthorizePage("/Account/Logout");
                            }).AddViewLocalization(x => x.ResourcesPath = "Resources");
            }
            else
            {
                services.AddMvc(options => options.Filters.Add(typeof(RequireHttpsAttribute)))
                    .AddJsonOptions(
                        options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
                    .AddRazorPagesOptions(
                        options =>
                            {
                                options.Conventions.AuthorizeFolder("/Account/Manage");
                                options.Conventions.AuthorizePage("/Account/Logout");
                            }).AddViewLocalization(x => x.ResourcesPath = "Resources");
            }

            services.AddKendo();

            services.Configure<IISOptions>(options => { });

            var embeddedFileProviders = new List<EmbeddedFileProvider>
                                            {
                                                new EmbeddedFileProvider(
                                                    typeof(LatestAddedAlbumsViewComponent)
                                                        .GetTypeInfo().Assembly,
                                                    "MyServer.ViewComponents.ImageGallery"),
                                                new EmbeddedFileProvider(
                                                    typeof(SocialViewComponent).GetTypeInfo()
                                                        .Assembly,
                                                    "MyServer.ViewComponents.Common")
                                            };

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.Configure<RazorViewEngineOptions>(
                options =>
                    {
                        foreach (var embeddedFileProvider in embeddedFileProviders)
                        {
                            options.FileProviders.Add(embeddedFileProvider);
                        }
                    });
        }
    }
}