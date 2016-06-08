namespace MyServer.Web.Main
{
    using System.Data.Entity;
    using System.Reflection;
    using System.Web.Mvc;

    using Autofac;
    using Autofac.Integration.Mvc;

    using MyServer.Data;
    using MyServer.Data.Common;
    using MyServer.Web.Main;

    //using ImageGallery.Data;
    //using ImageGallery.Data.Common;
    //using ImageGallery.Services.Album;

    public static class AutofacConfig
    {
        public static void RegisterAutofac()
        {
            var builder = new ContainerBuilder();

            // Register your MVC controllers.
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // OPTIONAL: Register model binders that require DI.
            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinderProvider();

            // OPTIONAL: Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // OPTIONAL: Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            // OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();

            // Register services
            RegisterServices(builder);

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.Register(x => new MyServerDbContext()).As<DbContext>().InstancePerRequest();

            //builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(IAlbumService)))
            //    .AsImplementedInterfaces()
            //    .InstancePerRequest();

            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerRequest();
            builder.RegisterGeneric(typeof(Repository<,>)).As(typeof(IRepository<,>)).InstancePerRequest();

            // builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            // .AssignableTo<BaseController>()
            // .PropertiesAutowired()
            // .InstancePerRequest();
        }
    }
}