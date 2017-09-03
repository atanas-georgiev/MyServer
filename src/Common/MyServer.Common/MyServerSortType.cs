namespace MyServer.Common
{
    using System.ComponentModel.DataAnnotations;

    public enum MyServerSortType
    {
        [Display(Name = "SortDateAddedDesc", ResourceType = typeof(Resources.Common))]
        SortDateAddedDesc,

        [Display(Name = "SortDateAddedAsc", ResourceType = typeof(Resources.Common))]
        SortDateAddedAsc,

        [Display(Name = "SortImagesDateDesc", ResourceType = typeof(Resources.Common))]
        SortImagesDateDesc,

        [Display(Name = "SortImagesDateAsc", ResourceType = typeof(Resources.Common))]
        SortImagesDateAsc,

        [Display(Name = "SortImagesCountDesc", ResourceType = typeof(Resources.Common))]
        SortImagesCountDesc,

        [Display(Name = "SortImagesCountAsc", ResourceType = typeof(Resources.Common))]
        SortImagesCountAsc
    }
}