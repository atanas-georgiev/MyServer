namespace MyServer.Web
{
    using System;
    using System.Globalization;
    using System.Reflection;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Net.Http.Headers;

    using MyServer.Data;
    using MyServer.Data.Common;
    using MyServer.Data.Models;
    using MyServer.Services.ImageGallery;
    using MyServer.Services.Mappings;
    using MyServer.Services.Users;
    using MyServer.Web.Helpers;
    using MyServer.Web.Migrations;

    using Newtonsoft.Json.Serialization;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder =
                new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            // if (env.IsDevelopment())

            // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
            builder.AddUserSecrets();

            // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
            builder.AddApplicationInsightsSettings(developerMode: true);

            builder.AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        public static IStringLocalizer<SharedResource> SharedLocalizer { get; private set; }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IServiceScopeFactory scopeFactory,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            UserManager<User> userManager,
            IStringLocalizer<SharedResource> sharedLocalizer)
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

            var helper = new PathHelper(env, userManager);
            loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            scopeFactory.SeedData();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();

                // app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles(
                new StaticFileOptions()
                    {
                        OnPrepareResponse = (context) =>
                            {
                                var headers = context.Context.Response.GetTypedHeaders();
                                headers.CacheControl = new CacheControlHeaderValue() { MaxAge = TimeSpan.FromDays(100) };
                            }
                    });

            app.UseIdentity();

            app.UseFacebookAuthentication(
                new FacebookOptions() { AppId = "521558431365642", AppSecret = "af05f969147e202f1e8c76c4cfd31a79" });

            app.UseGoogleAuthentication(
                new GoogleOptions()
                    {
                        ClientId = "18361776506-dphsr6a6eamnjcb5b144j5offcn3tndq.apps.googleusercontent.com",
                        ClientSecret = "gul1QEzDbz2-bq3tXi4r8hLI"
                    });

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
            app.UseSession();
            app.UseMvc(
                routes =>
                    {
                        // Areas support
                        routes.MapRoute(
                            name: "areaRoute",
                            template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                        routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
                    });

            // Configure Kendo UI
            app.UseKendo(env);

            var autoMapperConfig = new AutoMapperConfig();
            autoMapperConfig.Execute(Assembly.GetEntryAssembly());
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(this.Configuration);

            services.AddDbContext<MyServerDbContext>(
                options => options.UseSqlServer(this.Configuration.GetConnectionString("MyServerDb")));

            services.AddScoped<DbContext, MyServerDbContext>();
            services.Add(ServiceDescriptor.Scoped(typeof(IRepository<,>), typeof(Repository<,>)));
            services.Add(ServiceDescriptor.Scoped(typeof(IRepository<>), typeof(Repository<>)));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IAlbumService, AlbumService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<ILocationService, LocationService>();

            services.AddIdentity<User, IdentityRole>(
                o =>
                    {
                        o.Password.RequireDigit = false;
                        o.Password.RequireLowercase = false;
                        o.Password.RequireUppercase = false;
                        o.Password.RequireNonAlphanumeric = false;
                        o.Password.RequiredLength = 6;
                    }).AddEntityFrameworkStores<MyServerDbContext>().AddDefaultTokenProviders();

            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddMemoryCache();

            // services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc()
                .AddViewLocalization(x => x.ResourcesPath = "Resources")
                .AddDataAnnotationsLocalization()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            services.AddKendo();

            services.Configure<IISOptions>(options => { });
        }
    }
}