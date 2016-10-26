
using DOL.Core;
using DOL.Model;
using DOL.Service;
using DOL.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOL.Web
{
    /// <summary>
    /// 过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class LoginFilterAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller as BaseController;

            
            var controllerName = filterContext.RouteData.Values["Controller"].ToString();
            string requestUrl = filterContext.HttpContext.Request.Url.ToString();


            var user = CookieHelper.GetCurrentUser();
            if (user == null)
            {
                RedirectResult redirectResult = new RedirectResult("/Accout/Login?redirecturl=" + requestUrl);
                filterContext.Result = redirectResult;
            }
            else
            {
                if (user.MenuFlag != -1)
                {
                    int enumKey = EnumHelper.GetEnumKey(typeof(MenuFlag), controllerName);
                    if ((user.MenuFlag & enumKey) == 0)
                    {
                        RedirectResult redirectResult = new RedirectResult("/Base/Forbidden");
                        filterContext.Result = redirectResult;
                    }
                }
                if (user.OperateFlag != -1)
                {
                    var url = filterContext.HttpContext.Request.RawUrl;
                    var operateFlag = user.OperateFlag.HasValue ? user.OperateFlag.Value : 0;
                    if (!new WebService(new WebClient(filterContext.HttpContext)).IsHaveAuthority(operateFlag, url))
                    {
                        var result=new WebResult<bool>{ Code = ErrorCode.sys_user_role_error, Result = false };

                        JsonResult jsonResult = new JsonResult();
                        jsonResult.Data = result;
                        jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                        filterContext.Result = jsonResult;
                    }
                }
            }
        }
    }
}