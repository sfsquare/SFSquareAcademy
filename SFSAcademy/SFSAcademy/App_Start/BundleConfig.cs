using System.Web;
using System.Web.Optimization;

namespace SFSAcademy.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Content/javascripts/jquery/jquery.min.js")
                .Include("~/Content/javascripts/jquery/jquery-ui.min.js")
                .Include("~/Content/javascripts/jquery/jquery-1.9.1.js")
                .Include("~/Content/javascripts/jquery/jquery-ui.js")
                .Include("~/Content/javascripts/jquery/jquery.hotkeys.js")
            );

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Content/javascripts/jquery/jquery.validate.min.js",
                        "~/Content/javascripts/jquery/jquery.validate.unobtrusive.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryunobtrusive").Include(
                        "~/Content/javascripts/jquery/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Content/javascripts/scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/nicetitle").Include(
                      "~/Content/javascripts/scripts/nicetitle.js",
                      "~/Content/javascripts/droplicious.css"));

            bundles.Add(new ScriptBundle("~/bundles/fckeditor").Include(
                      "~/Content/javascripts/fckeditor/fckeditor.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquerytime").Include(
                        "~/Content/javascripts/jquery/jquery.timepicker.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/timecss").Include(
                      "~/Content/stylesheets/_styles/jquery.timepicker.css"));

        }
    }
}