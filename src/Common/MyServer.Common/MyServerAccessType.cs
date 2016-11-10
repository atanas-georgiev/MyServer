namespace MyServer.Common
{
    using System.ComponentModel.DataAnnotations;

    public enum MyServerAccessType
    {
        [Display(Name = "Public", ResourceType = typeof(MyServer.Common.Resources.Common))]
        Public,

        [Display(Name = "Registrated", ResourceType = typeof(MyServer.Common.Resources.Common))]
        Registrated,

        [Display(Name = "Private", ResourceType = typeof(MyServer.Common.Resources.Common))]
        Private
    }
}