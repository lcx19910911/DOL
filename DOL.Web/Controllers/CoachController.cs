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
        /// <summary>
        /// 科二学员
        /// </summary>
        /// <returns></returns>
        public ViewResult ThemeTwoStudent()
        {
            return View();
        }
        /// <summary>
        /// 科三学员
        /// </summary>
        /// <returns></returns>
        public ViewResult ThemeThreeStudent()
        {
            return View();
        }

        /// <summary>
        /// 油卡
        /// </summary>
        /// <returns></returns>
        public ViewResult OilCard()
        {
            return View();
        }
        /// <summary>
        /// 教练工资（管理员）
        /// </summary>
        /// <returns></returns>
        public ViewResult Info(DateTime? searchTime,string coachID)
        {
            return View(WebService.Get_CoachSalary(searchTime, coachID));
        }

        /// <summary>
        /// 我的车辆
        /// </summary>
        /// <returns></returns>
        public ViewResult Car()
        {
            return View();
        }
        /// <summary>
        /// 我的耗损
        /// </summary>
        /// <returns></returns>
        public ViewResult Waste()
        {
            return View(WebService.Get_CarSelectItem(Client.LoginUser.CoachID));
        }

        public JsonResult GetWaste(string carId, DateTime? searchTime)
        {
            return JResult(WebService.Get_WasteReport(Client.LoginUser.CoachID, carId, searchTime));
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
                else
                {
                    model.CoachList = new List<Core.SelectItem>();
                    model.CoachReportModel = new CoachReportModel();
                }
            }
            else
            {
                model.CoachList = new List<Core.SelectItem>();
                model.CoachReportModel = new CoachReportModel();
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
        public ActionResult GetThemeTwoPageList(int pageIndex, int pageSize, string name, string no,
            YesOrNoCode? code)
        {
            return JResult(WebService.Get_MyStudenPageList(pageIndex, pageSize, name, no, code, true));
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetThemeThreePageList(int pageIndex, int pageSize, string name, string no,
            YesOrNoCode? code )
        {
            return JResult(WebService.Get_MyStudenPageList(pageIndex, pageSize, name, no, code, false));
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="key"> 搜索项</param>
        /// <param name="value">搜索项</param>
        /// <returns></returns>
        public ActionResult GetCarPageList(int pageIndex, int pageSize, string brandName, string model, string modelCode, string engineNumber, string license)
        {
            return JResult(WebService.Get_CarPageList(pageIndex, pageSize, brandName, model, modelCode, engineNumber, license, Client.LoginUser.CoachID));
        }
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="key"> 搜索项</param>
        /// <param name="value">搜索项</param>
        /// <returns></returns>
        public ActionResult GetOilCardPageList(int pageIndex, int pageSize, string companyName, string no)
        {
            return JResult(WebService.Get_OilCardPageList(pageIndex, pageSize, companyName, no, Client.LoginUser.CoachID));
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
        public ActionResult Confirm(string id,int code)
        {
            return JResult(WebService.Confirm_Coach(id, code));
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