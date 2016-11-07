
using DOL.Core;
using DOL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOL.Web.Controllers
{
    [LoginFilter]
    public class StudentController : BaseController
    {

        public ViewResult Index()
        {
            return View();
        }


        public ViewResult Add()
        {
            return View();
        }

        public ViewResult Update()
        {
            return View();
        }


        public ViewResult Confirm()
        {
            return View();
        }

        public ViewResult MoreInfo()
        {
            return View();
        }

        public ViewResult Recyle()
        {
            return View();
        }

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Find(string ID)
        {
            return JResult(WebService.Find_Student(ID));
        }


        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetPageList(int pageIndex,
            int pageSize,
            string name,
            string referenceId,
            string no,
            string mobile,
            string enteredPointId,
            string makeDriverShopId,
            StudentCode state,
            DateTime? enteredTimeStart, DateTime? enteredTimeEnd,
            DateTime? makedTimeStart, DateTime? makeTimeEnd)
        {
            return JResult(WebService.Get_StudentPageList(pageIndex, pageSize, name, referenceId, no, mobile, enteredPointId, makeDriverShopId,state, enteredTimeStart, enteredTimeEnd, makedTimeStart, makeTimeEnd));
        }

        /// <summary>
        /// 获取删除的分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetDeletePageList(int pageIndex,
            int pageSize,
            string name,
            string no,
            string mobile)
        {
            return JResult(WebService.Get_StudentPageList(pageIndex, pageSize, name, null, no, mobile, null, null, StudentCode.DontMakeCard, null, null, null, null, true));
        }



        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetMoreInfoPageList(
            int pageIndex,
            int pageSize,
            string name,
            string no,
            string trianID,
            string driverShopID,
            string themeTwoCoachID,
            string themeThreeCoachID,
            YesOrNoCode? themeOnePass,
            YesOrNoCode? themeTwoPass,
            YesOrNoCode? themeThreePass,
            YesOrNoCode? themeFourPass,
            DateTime? themeOneTimeStart, DateTime? themeOneTimeEnd,
            DateTime? themeTwoTimeStart, DateTime? themeTwoTimeEnd,
            DateTime? themeThreeTimeStart, DateTime? themeThreeTimeEnd,
            DateTime? themeFourTimeStart, DateTime? themeFourTimeEnd
            )
        {
            return JResult(WebService.Get_StudentMoreInfoPageList(pageIndex, pageSize, name, no, trianID, driverShopID, themeTwoCoachID, themeThreeCoachID, themeOnePass, themeTwoPass, themeThreePass, themeFourPass,
                themeOneTimeStart, themeOneTimeEnd, themeTwoTimeStart, themeTwoTimeEnd, themeThreeTimeStart, themeThreeTimeEnd, themeFourTimeStart, themeFourTimeEnd));
        }

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetSelectItemList()
        {
            return JResult(WebService.Get_SelectItemList());
        }


        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetPaySelectItemList()
        {
            return JResult(WebService.Get_DataDictorySelectItem(GroupCode.PayType));
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Update(Student model)
        {
            ModelState.Remove("UpdaterID");
            ModelState.Remove("UpdatedTime");
            ModelState.Remove("CreatedTime");
            if (ModelState.IsValid)
            {
                return JResult(WebService.Update_Student(model));
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
        public ActionResult Delete(string ids)
        {
            return JResult(WebService.Delete_Student(ids));
        }


        /// <summary>
        /// 申请退学
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult WantDrop(string id,string remark,decimal money)
        {
            return JResult(WebService.WantDrop_Student(id,remark, money));
        }
        

        /// <summary>
        /// 报名
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddEntered(Student model)
        {
            ModelState.Remove("ID");
            ModelState.Remove("UpdaterID");
            ModelState.Remove("UpdatedTime");
            ModelState.Remove("CreatedTime");
            if (ModelState.IsValid)
            {
                return JResult(WebService.Add_Student(model));
            }
            else
            {
                return ParamsErrorJResult(ModelState);
            }
        }

        public ActionResult GetLogList(int pageIndex,
            int pageSize, string StudentID)
        {
            return JResult(WebService.Get_LogByStudentId(pageIndex, pageSize,StudentID));
        }

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult FindLog(string ID)
        {
            return JResult(WebService.Find_Log(ID));
        }

    }
}