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

            bundles.Add(new StyleBundle("~/Content/custom").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/justifiedGallery").Include("~/Content/JustifiedGallery/justifiedGallery.min.css"));

        }

        private static void RegisterScriptBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            bundles.Add(
                new ScriptBundle("~/bundles/kendo").Include(
                    "~/Scripts/kendo/kendo.web.min.js",
                    "~/Scripts/kendo/kendo.aspnetmvc.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/kendo/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/kendo/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryajax").Include("~/Scripts/kendo/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.validate*"));

            bundles.Add(
                new ScriptBundle("~/bundles/geocomplete").Include("~/Scripts/GeoComplete/jquery.geocomplete.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/justifiedGallery").Include("~/Scripts/JustifiedGallery/jquery.justifiedGallery.min.js"));
}
    }
}