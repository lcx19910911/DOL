
using DOL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOL.Web.Controllers
{
    [LoginFilter]
    public class ExpendController : BaseController
    {

        public ViewResult Index()
        {
            return View();
        }
        public ViewResult Report()
        {
            return View();
        }

        public JsonResult GetReport(DateTime? searchTime)
        {
            return JResult(WebService.Get_ExpendReport(searchTime));
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public JsonResult GetPageList(int pageIndex, int pageSize,string thingId,string no, DateTime? createTimeStart, DateTime? createTimeEnd)
        {
            return JResult(WebService.Get_ExpendPageList(pageIndex, pageSize, thingId, no, createTimeStart, createTimeEnd));
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult AddBach(List<Expend> model)
        {
            var result = WebService.Add_BatachExpend(model);
            return JResult(result);

        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult Delete(string ids)
        {
            return JResult(WebService.Delete_Expend(ids));
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
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult GetExpendSelectItem()
        {
            return JResult(WebService.GetExpendSelectItem());
        }

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Find(string id)
        {
            return JResult(WebService.Find_Expend(id));
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult Update(Expend entity)
        {
            ModelState.Remove("UpdaterID");
            ModelState.Remove("UpdatedTime");
            ModelState.Remove("CreatedTime");
            if (ModelState.IsValid)
            {
                var result = WebService.Update_Expend(entity);
                return JResult(result);
            }
            else
            {
                return ParamsErrorJResult(ModelState);
            }
        }

    }
}