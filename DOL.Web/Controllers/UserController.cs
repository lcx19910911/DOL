using DOL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DOL.Web.Controllers
{
    /// <summary>
    /// 用户管理
    /// </summary>
    [LoginFilter]
    public class UserController : BaseController
    {
        // GET: 
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Admin()
        {
            return View();

        }
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetPageList(int pageIndex, int pageSize, string name, string mobile, DateTime? startTimeStart, DateTime? endTimeEnd)
        {
            return JResult(WebService.Get_UserPageList(pageIndex, pageSize, name, mobile, startTimeStart, endTimeEnd));
        }


        /// <summary>
        /// 增加
        /// </summary>
        /// <returns></returns>
        public ActionResult Add(DOL.Model.User model)
        {
            ModelState.Remove("ID");
            ModelState.Remove("UpdaterID");
            ModelState.Remove("UpdatedTime");
            ModelState.Remove("CreatedTime");
            ModelState.Remove("UserId");
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                var result = WebService.Add_User(model);
                return JResult(result);
            }
            else
            {
                return ParamsErrorJResult(ModelState);
            }
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <returns></returns>
        public ActionResult Update(DOL.Model.User model)
        {
            ModelState.Remove("UpdaterID");
            ModelState.Remove("UpdatedTime");
            ModelState.Remove("CreatedTime");
            ModelState.Remove("NewPassword");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                var result = WebService.Update_User(model);
                return JResult(result);
            }
            else
            {
                return ParamsErrorJResult(ModelState);
            }
        }

        /// <summary>
        /// 操作权限
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="OperateFlag"></param>
        /// <returns></returns>

        public ActionResult UpdateOperate(string ID, long OperateFlag)
        {
            return JResult(WebService.Update_UserOperate(ID, OperateFlag));
        }

        /// <summary>
        /// 报名点权限
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="enteredPointIDStr"></param>
        /// <returns></returns>
        public ActionResult UpdateEnteredPointIDStr(string ID, string enteredPointIDStr)
        {
            return JResult(WebService.Update_UserEnteredPointIDStr(ID, enteredPointIDStr));
        }
        

        



        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public ActionResult Delete(string ids)
        {
            return JResult(WebService.Delete_User(ids));
        }

        /// <summary>
        /// 查找
        /// </summary>
        /// <returns></returns>
        public ActionResult Find(string id)
        {
            return JResult(WebService.Find_User(id));
        }

        /// <summary>
        /// 启用
        /// </summary>
        /// <returns></returns>
        public ActionResult Enable(string ids)
        {
            return JResult(WebService.Enable_User(ids));
        }

        /// <summary>
        /// 禁用
        /// </summary>
        /// <returns></returns>
        public ActionResult Disable(string ids)
        {
            return JResult(WebService.Disable_User(ids));
        }
    }
}