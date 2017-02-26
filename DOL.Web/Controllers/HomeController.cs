using DOL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DOL.Web.Controllers
{
    /// <summary>
    /// 首页
    /// </summary>
    [LoginFilter]
    public class HomeController : BaseController
    {
        // GET: 
        public ActionResult Index()
        {
            return View();
        }

        public void LoadCache()
        {
            WebService.Cache_Get_CoachList();
            WebService.Cache_Get_StudentList();
            WebService.Cache_Get_DataDictionary();
            WebService.Cache_Get_ReferenceList();
            WebService.Cache_Get_DriverShopList();
            WebService.Cache_Get_EnteredPointList();
        }
    }
}