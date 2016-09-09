﻿namespace MyServer.Services.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using AutoMapper;

    public class AutoMapperConfig
    {
        public static MapperConfiguration Configuration { get; private set; }

        public void Execute(Assembly assembly)
        {
            Configuration = new MapperConfiguration(
                cfg =>
                    {
                        var types = assembly.GetExportedTypes();
                        LoadStandardMappings(types, cfg);
                        //LoadReverseMappings(types, cfg);
                        LoadCustomMappings(types, cfg);
                    });
        }

        private static void LoadCustomMappings(IEnumerable<Type> types, IMapperConfigurationExpression mapperConfiguration)
        {
            //var maps = (from t in types
            //            from i in t.GetInterfaces()
            //            where typeof(IHaveCustomMappings).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface
            //            select (IHaveCustomMappings) Activator.CreateInstance(t)).ToArray();

            //foreach (var map in maps)
            //{
            //    map.CreateMappings(mapperConfiguration);
            //}

            var maps = (from t in types
                        from i in t.GetInterfaces()
                        where typeof(IHaveCustomMappings).IsAssignableFrom(t)
                        select (IHaveCustomMappings)Activator.CreateInstance(t)).ToArray();

            foreach (var map in maps)
            {
                map.CreateMappings(mapperConfiguration);
            }
        }

        //private static void LoadReverseMappings(IEnumerable<Type> types, MapperConfiguration mapperConfiguration)
        //{
        //    var maps = (from t in types
        //                from i in t.GetInterfaces()
        //                where
        //                    i.GetGenericTypeDefinition() == typeof(IMapTo<>)
        //                select new { Destination = i.GetGenericArguments()[0], Source = t }).ToArray();

        //    foreach (var map in maps)
        //    {
        //        mapperConfiguration.CreateMap(map.Source, map.Destination);
        //    }
        //}

        private static void LoadStandardMappings(IEnumerable<Type> types, IMapperConfigurationExpression mapperConfiguration)
        {
            var maps = (from t in types
                        from i in t.GetInterfaces()
                        where
                            i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)
                        select new { Source = i.GetGenericArguments()[0], Destination = t }).ToArray();

            foreach (var map in maps)
            {
                mapperConfiguration.CreateMap(map.Source, map.Destination);
            }
        }
    }
}