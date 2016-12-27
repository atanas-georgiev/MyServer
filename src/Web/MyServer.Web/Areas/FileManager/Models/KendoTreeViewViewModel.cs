using System.Collections.Generic;

namespace MyServer.Web.Areas.FileManager.Models
{
    public class KendoTreeViewViewModel
    {
        public string id { get; set; }
        public string Name { get; set; }
        public bool hasChildren { get; set; }

        public IEnumerable<KendoTreeViewViewModel> Children { get; set; }
    }
}
