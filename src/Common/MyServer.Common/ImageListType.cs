namespace MyServer.Common
{
    using System.ComponentModel.DataAnnotations;

    public enum ImageListType
    {
        [Display(Name = "Date", ResourceType = typeof(MyServer.Common.Resources.Common))]
        Date,

        [Display(Name = "Location", ResourceType = typeof(MyServer.Common.Resources.Common))]
        Location,

        [Display(Name = "Album", ResourceType = typeof(MyServer.Common.Resources.Common))]
        Album
    }
}
