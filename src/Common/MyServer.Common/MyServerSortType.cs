namespace MyServer.Common
{
    using System.ComponentModel.DataAnnotations;

    public enum MyServerSortType
    {
        [Display(Name = "SortLatestDate", ResourceType = typeof(MyServer.Common.Resources.Common))]
        LatestDate,

        [Display(Name = "SortLatestAdded", ResourceType = typeof(MyServer.Common.Resources.Common))]
        LatestAdded
    }
}
