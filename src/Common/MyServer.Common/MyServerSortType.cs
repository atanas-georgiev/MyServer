namespace MyServer.Common
{
    using System.ComponentModel.DataAnnotations;

    public enum MyServerSortType
    {
        [Display(Name = "SortDateAddedDesc", ResourceType = typeof(MyServer.Common.Resources.Common))]
        SortDateAddedDesc,

        [Display(Name = "SortDateAddedAsc", ResourceType = typeof(MyServer.Common.Resources.Common))]
        SortDateAddedAsc,

        [Display(Name = "SortImagesDateDesc", ResourceType = typeof(MyServer.Common.Resources.Common))]
        SortImagesDateDesc,

        [Display(Name = "SortImagesDateAsc", ResourceType = typeof(MyServer.Common.Resources.Common))]
        SortImagesDateAsc,

        [Display(Name = "SortImagesCountDesc", ResourceType = typeof(MyServer.Common.Resources.Common))]
        SortImagesCountDesc,

        [Display(Name = "SortImagesCountAsc", ResourceType = typeof(MyServer.Common.Resources.Common))]
        SortImagesCountAsc
    }
}
