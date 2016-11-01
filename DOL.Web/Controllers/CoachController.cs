﻿using DOL.Model;
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


        public ViewResult Train(DateTime? searchTime, string coachId)
        {
            if (string.IsNullOrEmpty(coachId))
            {
                var driverShopList = WebService.Get_DriverShopSelectItem("");
                
                if (driverShopList != null && driverShopList.Count > 0)
                {
                    var driverShopId = driverShopList[0].Value;
                    var coachList = WebService.Get_CoachSelectItem(driverShopId);
                    if (coachList != null && coachList.Count > 0)
                    {
                        return View(WebService.Get_CoachSalary(searchTime, coachList[0].Value));
                    }
                }
            }
           
            return View(WebService.Get_CoachSalary(searchTime, coachId));
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
        
    }
}