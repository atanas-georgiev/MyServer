namespace MyServer.Common
{
    using System.ComponentModel.DataAnnotations;

    public enum ImageListType
    {
        [Display(Name = "Date", ResourceType = typeof(Resources.Common))]
        Date,

        [Display(Name = "Location", ResourceType = typeof(Resources.Common))]
        Location,

        [Display(Name = "Album", ResourceType = typeof(Resources.Common))]
        Album
    }
}