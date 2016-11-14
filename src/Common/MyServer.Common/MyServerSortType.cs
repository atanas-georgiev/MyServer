namespace MyServer.Common
{
    using System.ComponentModel.DataAnnotations;

    public enum MyServerSortType
    {
        [Display(Name = "SortDateAddedAsc", ResourceType = typeof(MyServer.Common.Resources.Common))]
        SortDateAddedAsc,

        [Display(Name = "SortDateAddedDesc", ResourceType = typeof(MyServer.Common.Resources.Common))]
        SortDateAddedDesc,

        [Display(Name = "SortImagesDateAsc", ResourceType = typeof(MyServer.Common.Resources.Common))]
        SortImagesDateAsc,

        [Display(Name = "SortImagesDateDesc", ResourceType = typeof(MyServer.Common.Resources.Common))]
        SortImagesDateDesc,

        [Display(Name = "SortImagesCountAsc", ResourceType = typeof(MyServer.Common.Resources.Common))]
        SortImagesCountAsc,

        [Display(Name = "SortImagesCountDesc", ResourceType = typeof(MyServer.Common.Resources.Common))]
        SortImagesCountDesc
    }
}
