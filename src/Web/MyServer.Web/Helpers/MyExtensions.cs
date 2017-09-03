namespace MyServer.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public static class MyExtensions
    {
        public static string ToDescription(this Enum value)
        {
            var attributes =
                (DescriptionAttribute[])
                value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static IEnumerable<SelectListItem> ToSelectList(this Enum enumValue)
        {
            var result = from Enum e in Enum.GetValues(enumValue.GetType())
                select
                    new SelectListItem
                    {
                        Selected = e.Equals(enumValue),
                        Text = e.ToString(),
                        Value = Array.IndexOf(Enum.GetValues(e.GetType()), e).ToString()
                    };
            return result;
        }
    }
}