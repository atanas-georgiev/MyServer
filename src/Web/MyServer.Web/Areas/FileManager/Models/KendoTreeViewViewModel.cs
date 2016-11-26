using System.Collections.Generic;

namespace MyServer.Web.Areas.FileManager.Models
{
    public class KendoTreeViewViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool HasChildren { get; set; }

        public IEnumerable<KendoTreeViewViewModel> Children { get; set; }
    }
}
