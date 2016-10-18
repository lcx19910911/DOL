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
    /// 密室
    /// </summary>
    [LoginFilter]
    public class PayController : BaseController
    {
        // GET: 
        public ActionResult Index()
        {
            return View();
        }
    }
}