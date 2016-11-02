using DOL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOL.Web.Controllers
{
    [LoginFilter]
    public class EnteredPointController : BaseController
    {

        public ViewResult Index()
        {
            return View();
        }

        /// <summary>
        /// 报名点统计
        /// </summary>
        /// <param name="searchTime"></param>
        /// <returns></returns>
        public ViewResult Report(DateTime? searchTime)
        {
            return View(WebService.Get_EnteredPointReport(searchTime));
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult Add(EnteredPoint entity)
        {
            ModelState.Remove("ID");
            ModelState.Remove("UpdaterID");
            ModelState.Remove("UpdatedTime");
            ModelState.Remove("CreatedTime");
            if (ModelState.IsValid)
            {
                var result = WebService.Add_EnteredPoint(entity);
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
        public JsonResult Update(EnteredPoint entity)
        {
            ModelState.Remove("UpdaterID");
            ModelState.Remove("UpdatedTime");
            ModelState.Remove("CreatedTime");
            if (ModelState.IsValid)
            {
                var result = WebService.Update_EnteredPoint(entity);
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
        public ActionResult GetPageList(int pageIndex, int pageSize, string name, string provinceCode, string cityCode, string districtCode, string no)
        {
            return JResult(WebService.Get_EnteredPointPageList(pageIndex, pageSize, name, provinceCode, cityCode, districtCode, no));
        }

       
        /// <summary>
        /// 获取下拉框 flag
        /// </summary>
        /// <returns></returns>
        public ActionResult GetZTreeFlagChildren()
        {
            return JResult(WebService.Get_EnteredPointZTreeStr());
        }

        /// <summary>
        /// 获取下拉框 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSelectItem(string cityCode)
        {
            return JResult(WebService.Get_EnteredPointSelectItem(cityCode));
        }

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Find(string id)
        {
            return JResult(WebService.Find_EnteredPoint(id));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult Delete(string ids)
        {
            return JResult(WebService.Delete_EnteredPoint(ids));
        }
    }
}