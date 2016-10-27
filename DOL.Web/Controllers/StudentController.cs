
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
            DateTime? enteredTimeStart, DateTime? enteredTimeEnd,
            DateTime? makedTimeStart, DateTime? makeTimeEnd)
        {
            return JResult(WebService.Get_StudentPageList(pageIndex, pageSize, name, referenceId, no, mobile, enteredPointId, makeDriverShopId, enteredTimeStart, enteredTimeEnd, makedTimeStart, makeTimeEnd));
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
    }
}