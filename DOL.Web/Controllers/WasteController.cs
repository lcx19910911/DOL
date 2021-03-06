﻿
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
            return View(WebService.Get_CoachSelectItem("",true));
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
        public JsonResult GetPageList(int pageIndex, int pageSize, WasteCode code,string thingId, string oilId, string carId, string userId,string license)
        {
            return JResult(WebService.Get_WastePageList(pageIndex, pageSize, code, thingId, oilId, carId, userId, license));
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

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult AddBachOil(List<Waste> model, string oilId)
        {
            var result = WebService.Add_BatachOildWaste(model, oilId);
            return JResult(result);

        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult Delete(string ids)
        {
            return JResult(WebService.Delete_Waste(ids));
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult Update(Waste entity)
        {
            ModelState.Remove("UpdaterID");
            ModelState.Remove("UpdatedTime");
            ModelState.Remove("CreatedTime");
            ModelState.Remove("CreatedUserID");
            if (ModelState.IsValid)
            {
                var result = WebService.Update_Waste(entity);
                return JResult(result);
            }
            else
            {
                return ParamsErrorJResult(ModelState);
            }
        }
        
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult GetOilSelectItem()
        {
            return JResult(WebService.GetOilSelectItem());
        }

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Find(string ID)
        {
            return JResult(WebService.Find_Waste(ID));
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult GetRepairSelectItem()
        {
            return JResult(WebService.GetRepairSelectItem());
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult AddBachRepair(List<Waste> model)
        {
            var result = WebService.Add_BatachRepair(model);
            return JResult(result);

        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult GetItemSelectItem()
        {
            return JResult(WebService.Get_DataDictorySelectItem(GroupCode.Thing));
        }
        
    }
}