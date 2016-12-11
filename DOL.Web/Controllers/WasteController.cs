
using DOL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOL.Web.Controllers
{
    [LoginFilter]
    public class WasteController : BaseController
    {

        public ViewResult Index()
        {
            return View();
        }
        public ViewResult Report()
        {
            return View(WebService.Get_CoachSelectItem(""));
        }

        public JsonResult GetReport(string coachId, string carId, DateTime? searchTime)
        {
            return JResult(WebService.Get_WasteReport(coachId, carId, searchTime));
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public JsonResult GetPageList(int pageIndex, int pageSize, WasteCode code, string oilId, string carId, string userId,string license)
        {
            return JResult(WebService.Get_WastePageList(pageIndex, pageSize, code, oilId, carId, userId, license));
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult Add(Waste entity)
        {
            ModelState.Remove("ID");
            ModelState.Remove("UpdaterID");
            ModelState.Remove("UpdatedTime");
            ModelState.Remove("CreatedTime");
            ModelState.Remove("CreatedUserID");
            if (ModelState.IsValid)
            {
                var result = WebService.Add_Waste(entity);
                return JResult(result);
            }
            else
            {
                return ParamsErrorJResult(ModelState);
            }
        }


        
    }
}