using System.Web;
using System.Web.Optimization;

namespace DOL.Web
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            #region 样式
            bundles.Add(new StyleBundle("~/Content/Admin").Include(
                "~/Styles/css/amazeui.css",
                "~/Scripts/tipso/css/tipso.min.css",
                "~/Styles/admin.css",
                "~/Scripts/My97DatePicker/skin/WdatePicker.css"));

            #endregion

            #region 脚本

            bundles.Add(new ScriptBundle("~/Scripts/Admin").Include(
               "~/Scripts/jquery-2.2.3.min.js",
               "~/Scripts/jquery.form.js",
               "~/Scripts/amazeui.min.js",
               "~/Scripts/jquery-validation/js/jquery.validate.js",
               "~/Scripts/tipso/js/tipso.js",

               "~/Scripts/Nuoya/nuoya.core.js",
               "~/Scripts/Nuoya/nuoya.grid.js",
               "~/Scripts/Nuoya/nuoya.form.js",
               "~/Scripts/Nuoya/nuoya.other.js",

               "~/Scripts/My97DatePicker/WdatePicker.js",
               "~/Scripts/My97DatePicker/config.js",
               "~/Scripts/My97DatePicker/lang/zh-cn.js",

               "~/Scripts/file_upload_plug-in.js"
               ));

            #endregion
          
            BundleTable.EnableOptimizations = false;
        }
    }
}
