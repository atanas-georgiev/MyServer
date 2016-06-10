namespace MyServer.Web.Main
{
    using System.Web.Optimization;

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();

            RegisterScriptBundles(bundles);
            RegisterContentBundles(bundles);

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
        }

        private static void RegisterContentBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/bootstrap.min.css"));

            bundles.Add(
                new StyleBundle("~/Content/kendo").Include(
                    "~/Content/kendo/kendo.common.min.css",
                    "~/Content/kendo/kendo.common-bootstrap.min.css",
                    "~/Content/kendo/kendo.bootstrap.min.css"));

            //bundles.Add(
            //    new StyleBundle(
            //        "~/content/toastr",
            //        "http://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/css/toastr.min.css").Include(
            //            "~/Content/toastr.min.css"));

            bundles.Add(new StyleBundle("~/Content/custom").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/photoswipe").Include("~/Content/Photoswipe/photoswipe.css", "~/Content/Photoswipe/default-skin/default-skin.css"));
        }

        private static void RegisterScriptBundles(BundleCollection bundles)
        {
            bundles.Add(
                new ScriptBundle("~/bundles/kendo").Include(
                    "~/Scripts/kendo/kendo.web.min.js",
                    "~/Scripts/kendo/kendo.aspnetmvc.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/kendo/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/kendo/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryajax").Include("~/Scripts/kendo/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.validate*"));

            //bundles.Add(new ScriptBundle("~/bundles/geocomplete").Include("~/Scripts/Geocomplete/*.js"));

            //bundles.Add(new ScriptBundle("~/bundles/montage").Include("~/Scripts/Montage/*.js"));

            //bundles.Add(
            //    new ScriptBundle(
            //        "~/bundles/toastr",
            //        "http://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/js/toastr.min.js").Include(
            //            "~/Scripts/toastr.min.js"));

            //bundles.Add(
            //    new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.js", "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/photoswipe").Include("~/Scripts/Photoswipe/*.js"));
        }
    }
}