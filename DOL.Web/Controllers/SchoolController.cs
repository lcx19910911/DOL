
using DOL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOL.Web.Controllers
{
    [LoginFilter]
    public class SchoolController : BaseController
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
        public JsonResult Add(DataDictionary entity)
        {
            ModelState.Remove("ID");
            ModelState.Remove("Key");
            if (ModelState.IsValid)
            {
                var result = WebService.Add_DataDictionary(entity);
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
        public JsonResult Update(DataDictionary entity)
        {
            ModelState.Remove("Key");
            if (ModelState.IsValid)
            {
                var result = WebService.Update_DataDictionary(entity);
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
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="key"> 搜索项</param>
        /// <param name="value">搜索项</param>
        /// <returns></returns>
        public ActionResult GetPageList(int pageIndex, int pageSize, string key, string value,string parentkey,GroupCode code)
        {
            return JResult(WebService.Get_GroupPageList(pageIndex, pageSize, parentkey, code, key, value));
        }


        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Find(string ID)
        {
            return JResult(WebService.Find_DataDictionary(ID));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult Delete(string ID)
        {
            return JResult(WebService.Delete_DataDictionary(ID));
        }


        /// <summary>
        /// 获取地区下拉框
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSelectItem(string value,GroupCode code=GroupCode.School)
        {
            return JResult(WebService.Get_SchoolList(value,code));
        }

    }
}