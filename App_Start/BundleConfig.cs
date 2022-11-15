using System.Web;
using System.Web.Optimization;

namespace Passengers
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));
            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*",
            //            "~/Scripts/messages_ar.js"));
            //bundles.Add(new ScriptBundle("~/bundles/jqueryval_unobtrusive").Include(
            //            "~/Scripts/jquery.unobtrusive-ajax.min.js",
            //            "~/Scripts/jquery.validate.min.js",
            //            "~/Scripts/jquery.validate.unobtrusive.min.js"));
            //bundles.Add(new ScriptBundle("~/bundles/jquery_ui").Include(
            //            "~/Scripts/jquery-ui-{version}.js",
            //            "~/Scripts/datepicker-ar.js"));

            //bundles.Add(new ScriptBundle("~/bundles/javascript").Include(
            //            "~/Scripts/dt/jquery.dataTables.min.js",
            //            "~/Scripts/dt/dataTables.bootstrap.min.js",
            //            "~/Scripts/dt/CustomDatatable.js",

            //            "~/Scripts/other/jquery.preloader.js",
            //            "~/Scripts/other/toastr.min.js",
            //            "~/Scripts/other/select2.min.js",

            //            "~/Scripts/moment.min.js",
            //            "~/Scripts/moment-with-langs.js",
               //         "~/Scripts/bootstrapdtp/bootstrap-datetimepicker.js"
            //            ));

            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          //"~/Scripts/bootstrap.js",
            //          "~/Scripts/bootstrap.rtl.min.js",
            //          "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/font-awesome.min.css",
            //          "~/Content/bootstrap.css",
            //          "~/Content/bootstrap.rtl.min.css",
            //          "~/Content/hover.css",
            //          "~/Content/dt/dataTables.bootstrap.min.css",
            //          "~/Content/other/preloader.css",
            //          "~/Content/other/toastr.min.css",
            //          "~/Content/other/select2.min.css",
            //          "~/Content/themes/base/all.css",
              //       "~/Content/boostrapdtp/bootstrap-datetimepicker.css",
            //          "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/v2/css")
                    .Include("~/Content/v2/bootstrap/css/bootstrap.4.1.3.min.css")
                    .Include("~/Content/v2/font-awesome/css/font-awesome.min.css")
                    .Include("~/Content/v2/animate/animate.min.css")
                    .Include("~/Content/v2/ionicons/css/ionicons.min.css")
                    .Include("~/Content/v2/toastr.min.css")
                    .Include("~/Content/dt/dataTables.bootstrap4.min.css")
                    .Include("~/Content/bootstrap.rtl.min.css")
                    .Include("~/Content/other/preloader.css")
                    .Include("~/Content/other/select2.min.css")
                    .Include("~/Content/v2/style.css")
                    .Include("~/Content/v2/Site.Custom.css")
                );

            bundles.Add(new ScriptBundle("~/bundles/v2/jquery")
                    //.Include("~/Scripts/v2/jquery/jquery.min.js")
                    //.Include("~/Scripts/jquery-confirm.min.js")
                );
            
            bundles.Add(new ScriptBundle("~/bundles/jqueryval")
                    .Include("~/Scripts/jquery.validate.min.js")
                    .Include("~/Scripts/jquery.validate.unobtrusive.bootstrap.min.js")
                    .Include("~/Scripts/jquery.validate.unobtrusive.min.js")
                    .Include("~/Scripts/messages_ar.js"));

            bundles.Add(new ScriptBundle("~/bundles/v2/modernizr")
                    .Include("~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/v2/jqueryval_unobtrusive")
                .Include("~/Scripts/jquery.unobtrusive-ajax.min.js")
                .Include("~/Scripts/jquery.validate.min.js")
                .Include("~/Scripts/jquery.validate.unobtrusive.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/v2/js")
                    //.Include("~/Scripts/v2/jquery/jquery.min.js")
                    .Include("~/Scripts/v2/jquery/jquery-migrate.min.js")
                    .Include("~/Scripts/v2/bootstrap/js/bootstrap.bundle.min.js")
                    .Include("~/Scripts/v2/superfish/hoverIntent.js")
                    .Include("~/Scripts/v2/superfish/superfish.min.js")
                    .Include("~/Scripts/v2/wow/wow.min.js")
                    .Include("~/Scripts/v2/toastr.min.js")
                    .Include("~/Scripts/v2/counterup/jquery.waypoints.min.js")
                    .Include("~/Scripts/v2/counterup/jquery.counterup.js")
                    .Include("~/Scripts/v2/contactform/contactform.js")
                    .Include("~/Scripts/other/select2.min.js")
                    .Include("~/Scripts/v2/main.js")
                    .Include("~/Scripts/v2/jquery.min.js")
                );

            bundles.Add(new ScriptBundle("~/bundles/v2/dataTables").Include(
                        "~/Scripts/dt/jquery.dataTables.min.js",
                        "~/Scripts/dt/dataTables.bootstrap4.min.js",
                        "~/Scripts/dt/CustomDatatable.js",
                        "~/Scripts/other/jquery.preloader.js",
                        "~/Scripts/moment.min.js",
                        "~/Scripts/moment-with-langs.js"
                        ));
        }
    }
}
