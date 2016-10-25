
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

      
        ///// <summary>
        ///// 获取分页列表
        ///// </summary>
        ///// <param name="pageIndex">页码</param>
        ///// <param name="pageSize">分页大小</param>
        ///// <param name="name">名称 - 搜索项</param>
        ///// <param name="no">编号 - 搜索项</param>
        ///// <returns></returns>
        //public ActionResult GetPageList(int pageIndex, int pageSize, string name, string no)
        //{
        //    return JResult(WebService.Get_StudentPageList(pageIndex, pageSize, name, no));
        //}
        

        ///// <summary>
        ///// 查找实体
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public ActionResult Find(string id)
        //{
        //    return JResult(WebService.Find_Student(id));
        //}

        ///// <summary>
        ///// 删除
        ///// </summary>
        ///// <param name="ids"></param>
        ///// <returns></returns>
        //public ActionResult Delete(string ids)
        //{
        //    return JResult(WebService.Delete_Student(ids));
        //}
    }
}