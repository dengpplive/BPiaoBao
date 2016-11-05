using System.Web;
using System.Web.Optimization;

namespace BPiaoBao.Web.SupplierManager
{
    public class BundleConfig
    {
        // 有关 Bundling 的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.9.0.js",
                        "~/Scripts/json2.js"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                        "~/Scripts/knockout-{version}.js",
                        "~/Scripts/knockout.mapping-latest.js"));

            bundles.Add(new ScriptBundle("~/bundles/easyui").Include(
                        "~/easyui-1.3.6/jquery.easyui.js",
                        "~/easyui-1.3.6/locale/easyui-lang-zh_CN.js"));

            bundles.Add(new ScriptBundle("~/bundles/upload").Include(
                        "~/Scripts/jquery.uploadify.js"));

            bundles.Add(new ScriptBundle("~/bundles/layout").Include(
                        "~/Scripts/jquery.layout-latest.js"));

            bundles.Add(new ScriptBundle("~/bundles/common").Include(
                        "~/Scripts/extend.js",
                        "~/Scripts/ko.custombinding.js",

                        "~/Scripts/commonViewModel.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryplus").Include( 
                        "~/easyui/plugins/jquery.loadmask.js",
                        "~/easyui/plugins/jquery.watermark.js"));
             

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/css/style.css",
                        "~/Content/css/loadmask.css"));

            bundles.Add(new StyleBundle("~/Content/login").Include(
                        "~/Content/css/login.css"));

            bundles.Add(new StyleBundle("~/Content/Upload").Include(
                        "~/Content/uploadify/uploadify.css"));

            bundles.Add(new StyleBundle("~/easyui/css").Include(
                        "~/easyui-1.3.6/themes/metro-blue/easyui.css",
                        "~/easyui-1.3.6/themes/icon.css"));
        }
    }
}