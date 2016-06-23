namespace MyServer.Web.Api
{
    using System.Data.Entity;
    using System.Reflection;
    using System.Web.Http;
    using System.Web.Mvc;

    using Autofac;
    using Autofac.Integration.Mvc;
    using Autofac.Integration.WebApi;

    using MyServer.Data;
    using MyServer.Data.Common;
    using MyServer.Services.ImageGallery;
    using MyServer.Services.Users;

    public static class AutofacConfig
    {
        public static void RegisterAutofac()
        {
            var builder = new ContainerBuilder();

            // Register your WebApi controllers.
            builder.RegisterApiControllers(typeof(WebApiApplication).Assembly);

            // OPTIONAL: Register model binders that require DI.
            builder.RegisterWebApiModelBinders(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiModelBinderProvider();

            // OPTIONAL: Register web abstractions like HttpContextBase.
        //    builder.RegisterModule<AutofacWebTypesModule>();

            // OPTIONAL: Enable property injection in view pages.
       //     builder.RegisterSource(new ViewRegistrationSource());

            // OPTIONAL: Enable property injection into action filters.
        //    builder.RegisterWebApiFilterProvider();

            // Register services
            RegisterServices(builder);

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver =
                 new AutofacWebApiDependencyResolver(container);
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.Register(x => new MyServerDbContext()).As<DbContext>().InstancePerRequest();

            builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(IAlbumService)))
                .AsImplementedInterfaces()
                .InstancePerRequest();

            builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(IUserService)))
                .AsImplementedInterfaces()
                .InstancePerRequest();


            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerRequest();
            builder.RegisterGeneric(typeof(Repository<,>)).As(typeof(IRepository<,>)).InstancePerRequest();

            // builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            // .AssignableTo<BaseController>()
            // .PropertiesAutowired()
            // .InstancePerRequest();
        }
    }
}