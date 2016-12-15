
using DOL.Core;
using DOL.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        public ViewResult School()
        {
            return View();
        }


        public ViewResult Recyle()
        {
            return View();
        }

        public ActionResult History(string Id, int isAfter)
        {
            var history = WebService.Find_Log(Id);
            if (history != null)
                ViewBag.Data = isAfter == 1 ? history.AfterJson.Replace("&quot;", "") : history.BeforeJson.Replace("&quot;", "");
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

        public ActionResult UpdateTimeCode(string ID, ThemeTimeCode themeTwoTimeCode, ThemeTimeCode themeThreeTimeCode)
        {
            return JResult(WebService.Update_StudentTimeCode(ID, themeTwoTimeCode, themeThreeTimeCode));
        }
        public ActionResult BatchUpdateTimeCode(string ids, ThemeTimeCode themeTwoTimeCode, ThemeTimeCode themeThreeTimeCode)
        {
            return JResult(WebService.Update_BatchStudentTimeCode(ids, themeTwoTimeCode, themeThreeTimeCode));
        }

        public ActionResult UpdateCoach(string ID, string themeTwoCoachID, string themeThreeCoachID)
        {
            return JResult(WebService.Update_StudentCoach(ID, themeTwoCoachID, themeThreeCoachID));
        }
        public ActionResult UpdateCreater(string ID, string createrID)
        {
            return JResult(WebService.Update_StudentCreater(ID,createrID));
        }

        
        public ActionResult BatchUpdateCoach(string ids, string themeTwoCoachID, string themeThreeCoachID)
        {
            return JResult(WebService.Update_BatchStudentCoach(ids, themeTwoCoachID, themeThreeCoachID));
        }

        public ActionResult UpdateDriver(string ID, string makeDriverShopID, DateTime makeCardDate)
        {
            return JResult(WebService.Update_StudentDriver(ID, makeDriverShopID, makeCardDate));
        }

        public ActionResult BatchUpdateDriver(string ids, string makeDriverShopID, DateTime makeCardDate)
        {
            return JResult(WebService.Update_BatchStudentDriver(ids, makeDriverShopID, makeCardDate));
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
            int moneyIsFull,
            int isOnSchool,
            int orderBy,
            StudentCode state,
            DateTime? enteredTimeStart, DateTime? enteredTimeEnd,
            DateTime? makedTimeStart, DateTime? makeTimeEnd)
        {
            return JResult(WebService.Get_StudentPageList(pageIndex, pageSize, name, referenceId, no, mobile, enteredPointId, makeDriverShopId, state, moneyIsFull,isOnSchool, orderBy, enteredTimeStart, enteredTimeEnd, makedTimeStart, makeTimeEnd));
        }

        /// <summary>
        /// 导出获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult ExportPageList(int pageIndex,
            int pageSize,
            string name,
            string referenceId,
            string no,
            string mobile,
            string enteredPointId,
            string makeDriverShopId,
            StudentCode state,
            int orderBy,
            DateTime? enteredTimeStart, DateTime? enteredTimeEnd,
            DateTime? makedTimeStart, DateTime? makeTimeEnd,
            bool isAll = false)
        {

            var list = WebService.Export_StudentPageList(pageIndex, pageSize, name, referenceId, no, mobile, enteredPointId, makeDriverShopId, state,orderBy, enteredTimeStart, enteredTimeEnd, makedTimeStart, makeTimeEnd);
            string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            string filePath = Path.Combine(Server.MapPath("~/") + @"Export\" + fileName);
       
            NPOIHelper<StudentExportModel>.GetExcel(list, GetHT(), filePath);
            //Directory.Delete(filePath);
            return File(filePath, "application/vnd.ms-excel", fileName);
        }

        private Hashtable GetHT()
        {
            Hashtable hs = new Hashtable();
            hs["Name"] = "姓名";
            hs["IDCard"] = "身份证";
            hs["GenderCode"] = "性别";
            hs["ProvinceName"] = "省份";
            hs["CityName"] = "市";
            hs["Address"] = "地址";
            hs["Mobile"] = "手机号";
            hs["CertificateName"] = "证书";
            hs["EnteredPointName"] = "报名点";
            hs["ReferenceName"] = "推荐人";
            hs["WantDriverShopName"] = "意向驾校";
            hs["TrianName"] = "培训班别";
            hs["Money"] = "费用";
            hs["HadPayMoney"] = "已交费用";
            hs["MoneyIsFull"] = "是否缴清";
            hs["PayMethodName"] = "缴费方式";
            hs["Remark"] = "备注";
            hs["EnteredDate"] = "报名时间";
            hs["EnteredProvinceName"] = "报名省份";
            hs["EnteredCityName"] = "报名市";

            hs["MakeDriverShopName"] = "制卡驾校";
            hs["MakeCardDate"] = "制卡日期";
            hs["MakeCardCityName"] = "制卡地";
            hs["MakeCardRemark"] = "制卡备注";
            hs["ThemeOneDate"] = "科一时间";
            hs["ThemeOnePass"] = "科一是否通过";
            hs["ThemeTwoDate"] = "科二时间";
            hs["ThemeTwoPass"] = "科二是否通过";
            hs["ThemeTwoTimeCode"] = "科二学时状态";
            hs["ThemeTwoCoachName"] = "科二教练";
            hs["ThemeThreeDate"] = "科三时间";
            hs["ThemeThreePass"] = "科三是否通过";
            hs["ThemeThreeTimeCode"] = "科三学时状态";
            hs["ThemeThreeCoachName"] = "科三教练";
            hs["ThemeFourDate"] = "科四时间";
            hs["ThemeFourPass"] = "科四是否通过";
            hs["State"] = "学员状态";
            hs["NowTheme"] = "当前科目";
            hs["DropOutDate"] = "退学时间";
            hs["PayOrderTypeName"] = "支付渠道";
            hs["VoucherNO"] = "凭证";
            return hs;
        }

        public ActionResult ExportInto(string mark)
        {
            HttpPostedFileBase file = Request.Files[0];
            if (file != null)
            {
                string path = UploadHelper.Save(file, mark);
                string filePath = Path.Combine(Server.MapPath("~/") + path);
                var list = NPOIHelper<StudentExportModel>.FromExcel(GetHT(), filePath);
                //var msg = ;
                return JResult(WebService.ExportInto_Student(list));
            }
            else
                return JResult(null);
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
            string mobile,
            int orderBy)
        {
            return JResult(WebService.Get_StudentPageList(pageIndex, pageSize, name, null, no, mobile, null, null, StudentCode.DontMakeCard,-1,-1,orderBy, null, null, null, null, true));
        }

        /// <summary>
        /// 获取客情分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetSchoolPageList(
            int pageIndex,
            int pageSize,
            int orderBy,
            string name,
            string schoolID,
            string collegeID,
            string majorID,
            string age,
            string wantDriverShopID,
            string makeDriverShopID,
            string provinceCode,
            string cityCode
            )
        {
            return JResult(WebService.Get_StudentSchoolPageList(pageIndex, pageSize,orderBy, name,schoolID,collegeID,majorID,age, wantDriverShopID, makeDriverShopID, provinceCode,cityCode));
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
            string referenceID,
            string no,
            string trianID,
            string driverShopID,
            string themeTwoCoachID,
            string themeThreeCoachID,
            int orderBy,
            ThemeTimeCode themeTwoTimeCode,
            ThemeTimeCode themeThreeTimeCode,
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
            return JResult(WebService.Get_StudentMoreInfoPageList(pageIndex, pageSize, name,referenceID, no, trianID, driverShopID, themeTwoCoachID, themeThreeCoachID,orderBy, themeTwoTimeCode,themeThreeTimeCode, themeOnePass, themeTwoPass, themeThreePass, themeFourPass,
                themeOneTimeStart, themeOneTimeEnd, themeTwoTimeStart, themeTwoTimeEnd, themeThreeTimeStart, themeThreeTimeEnd, themeFourTimeStart, themeFourTimeEnd));
        }

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetSelectItemList(string id, bool isAll=false)
        {
            return JResult(WebService.Get_SelectItemList(id, isAll));
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
            ModelState.Remove("CreaterID");
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
        public ActionResult WantDrop(PayOrder model, string remark)
        {
            return JResult(WebService.WantDrop_Student(model, remark));
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
            ModelState.Remove("CreaterID");
            
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
            return JResult(WebService.Get_LogByStudentId(pageIndex, pageSize, StudentID));
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