namespace MyServer.Services.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using AutoMapper;

    public class AutoMapperConfig
    {
        public static MapperConfiguration Configuration { get; private set; }

        public void Execute(IEnumerable<Assembly> assemblies)
        {
            List<Type> typesAssemblies = new List<Type>();

            foreach (var assembly in assemblies)
            {
                typesAssemblies.AddRange(assembly.GetExportedTypes());
            }

            Configuration = new MapperConfiguration(
                cfg =>
                    {
                        var types = typesAssemblies.ToArray();
                        LoadStandardMappings(types, cfg);
                        LoadCustomMappings(types, cfg);
                    });
        }

        private static void LoadCustomMappings(
            IEnumerable<Type> types,
            IMapperConfigurationExpression mapperConfiguration)
        {
            var maps = (from t in types
                        where typeof(IHaveCustomMappings).IsAssignableFrom(t)
                        select (IHaveCustomMappings)Activator.CreateInstance(t)).ToArray();

            foreach (var map in maps)
            {
                map.CreateMappings(mapperConfiguration);
            }
        }

        private static void LoadStandardMappings(
            IEnumerable<Type> types,
            IMapperConfigurationExpression mapperConfiguration)
        {
            var maps = (from t in types
                        from i in t.GetInterfaces()
                        where i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)
                        select new { Source = i.GetGenericArguments()[0], Destination = t }).ToArray();

            foreach (var map in maps)
            {
                mapperConfiguration.CreateMap(map.Source, map.Destination);
            }
        }
    }
}