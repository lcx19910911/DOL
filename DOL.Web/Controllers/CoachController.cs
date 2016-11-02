using DOL.Model;
using DOL.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOL.Web.Controllers
{
    /// <summary>
    /// 教练
    /// </summary>
    [LoginFilter]
    public class CoachController : BaseController
    {

        public ViewResult Index()
        {
            return View();
        }

        public ViewResult MyStudent()
        {
            return View();
        }


        /// <summary>
        /// 教练培训信息
        /// </summary>
        /// <param name="searchTime"></param>
        /// <param name="driverShopId"></param>
        /// <param name="coachId"></param>
        /// <returns></returns>
        public ViewResult Train(DateTime? searchTime, string driverShopId, string coachId)
        {
            CoacheInfoModel model = new CoacheInfoModel();

            model.DriverShopList = WebService.Get_DriverShopSelectItem("");

            if (model.DriverShopList != null && model.DriverShopList.Count > 0)
            {
                driverShopId = string.IsNullOrEmpty(driverShopId) ? model.DriverShopList[0].Value : driverShopId;
                model.CoachList = WebService.Get_CoachSelectItem(model.DriverShopList[0].Value);
                if (model.CoachList != null && model.CoachList.Count > 0)
                {
                    coachId = string.IsNullOrEmpty(coachId) ? model.CoachList[0].Value : coachId;
                    model.CoachReportModel = WebService.Get_CoachSalary(searchTime, coachId);
                }
            }
            return View(model);
        }

        public ViewResult Exams(DateTime? searchTime)
        {
            return View(WebService.Get_CoachSalary(searchTime, Client.LoginUser.CoachID));
        }

        public ViewResult Wages(DateTime? searchTime)
        {
            return View(WebService.Get_CoachSalary(searchTime, Client.LoginUser.CoachID));
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult Add(Coach entity)
        {
            ModelState.Remove("ID");
            ModelState.Remove("UpdaterID");
            ModelState.Remove("UpdatedTime");
            ModelState.Remove("CreatedTime");       
            if (ModelState.IsValid)
            {
                var result = WebService.Add_Coach(entity);
                return JResult(result);
            }
            else
            {
                return ParamsErrorJResult(ModelState);
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult Update(Coach entity)
        {
            ModelState.Remove("UpdaterID");
            ModelState.Remove("UpdatedTime");
            ModelState.Remove("CreatedTime");
            if (ModelState.IsValid)
            {
                var result = WebService.Update_Coach(entity);
                return JResult(result);
            }
            else
            {
                return ParamsErrorJResult(ModelState);
            }
        }

        
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPageList(int pageIndex, int pageSize, string name,  string no)
        {
            return JResult(WebService.Get_CoachPageList(pageIndex, pageSize, name, no));
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMyStudentPageList(int pageIndex, int pageSize, string name, string no,
            ThemeTimeCode? themeTwoCode,
            ThemeTimeCode? themeThreeCode)
        {
            return JResult(WebService.Get_MyStudenPageList(pageIndex, pageSize, name, no, themeTwoCode, themeThreeCode));
        }


        /// <summary>
        /// 获取下拉框 
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectItem(string id)
        {
            return JResult(WebService.Get_CoachSelectItem(id));
        }

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Find(string id)
        {
            return JResult(WebService.Find_Coach(id));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult Delete(string ids)
        {
            return JResult(WebService.Delete_Coach(ids));
        }
        /// <summary>
        /// 我的工资
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult GetSalary(DateTime? time, string id)
        {
            return JResult(WebService.Get_CoachSalary(time,id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSelectItem(string driverShopId)
        {
            return JResult(WebService.Get_CoachSelectItem(driverShopId));
        }
        
    }
}