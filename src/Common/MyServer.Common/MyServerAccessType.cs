namespace MyServer.Common
{
    using System.ComponentModel.DataAnnotations;

    public enum MyServerAccessType
    {
        [Display(Name = "Public", ResourceType = typeof(Resources.Common))]
        Public,

        [Display(Name = "Registrated", ResourceType = typeof(Resources.Common))]
        Registrated,

        [Display(Name = "Private", ResourceType = typeof(Resources.Common))]
        Private
    }
}