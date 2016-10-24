using DOL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOL.Web.Controllers
{
    /// <summary>
    /// 驾校
    /// </summary>
    [LoginFilter]
    public class DriverShopController : BaseController
    {

        public ViewResult Index()
        {
            return View();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult Add(DriverShop entity)
        {
            ModelState.Remove("ID");
            ModelState.Remove("UpdatedTime");
            ModelState.Remove("CreatedTime");
            if (ModelState.IsValid)
            {
                var result = WebService.Add_DriverShop(entity);
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
        public JsonResult Update(DriverShop entity)
        {
            ModelState.Remove("UpdatedTime");
            ModelState.Remove("CreatedTime");
            if (ModelState.IsValid)
            {
                var result = WebService.Update_DriverShop(entity);
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
            return JResult(WebService.Get_DriverShopPageList(pageIndex, pageSize, name, provinceCode, cityCode, districtCode, no));
        }

       
        /// <summary>
        /// 获取下拉框 flag
        /// </summary>
        /// <returns></returns>
        public ActionResult GetZTreeFlagChildren()
        {
            return JResult(WebService.Get_DriverShopZTreeStr());
        }

        /// <summary>
        /// 获取下拉框 
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectItem(string ID)
        {
            return JResult(WebService.Get_DriverShopSelectItem(ID));
        }

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Find(string id)
        {
            return JResult(WebService.Find_DriverShop(id));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult Delete(string ids)
        {
            return JResult(WebService.Delete_DriverShop(ids));
        }
    }
}