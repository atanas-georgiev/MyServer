namespace MyServer
{
    using Data;
    using Data.Common;
    using Data.Models;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json.Serialization;
    using Services.ImageGallery;
    using Services.Mappings;
    using Services.Users;
    using System.Globalization;
    using System.Reflection;
    using Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Localization;
    using Microsoft.AspNetCore.Identity;
    using MyServer.Data.Migrations;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddDbContext<MyServerDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MyServerDb")));

            services.AddScoped<DbContext, MyServerDbContext>();
            services.Add(ServiceDescriptor.Scoped(typeof(IRepository<,>), typeof(Repository<,>)));
            services.Add(ServiceDescriptor.Scoped(typeof(IRepository<>), typeof(Repository<>)));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IAlbumService, AlbumService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<ILocationService, LocationService>();

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<MyServerDbContext>()
                .AddDefaultTokenProviders();

            services.AddDistributedMemoryCache();
            services.AddSession();

            //services.AddLocalization(options => options.ResourcesPath = "Resources");

            services
                .AddMvc()
                    .AddViewLocalization(x => x.ResourcesPath = "Resources")
                    .AddDataAnnotationsLocalization()
                    .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            services.AddKendo();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceScopeFactory scopeFactory, IHostingEnvironment env, ILoggerFactory loggerFactory, UserManager<User> userManager)
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("bg-BG")
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });

            var helper = new PathHelper(env, userManager);
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
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
                //app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseFacebookAuthentication(new FacebookOptions()
            {                
                AppId = "521558431365642",
                AppSecret = "af05f969147e202f1e8c76c4cfd31a79"
            });

            app.UseGoogleAuthentication(new GoogleOptions()
            {
                ClientId = "18361776506-dphsr6a6eamnjcb5b144j5offcn3tndq.apps.googleusercontent.com",
                ClientSecret = "PdTUR7GxKo40AV93WlvpcMQ7"
            });

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
            app.UseSession();
            app.UseMvc(routes =>
            {
                // Areas support
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Configure Kendo UI
            app.UseKendo(env);

            var autoMapperConfig = new AutoMapperConfig();
            autoMapperConfig.Execute(Assembly.GetEntryAssembly());
        }
    }
}
