﻿using DOL.Core;
using DOL.Model;
using DOL.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Service
{
    public partial class WebService
    {
        string studentKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "Student");

        /// <summary>
        /// 站内通知全局缓存
        /// </summary>
        /// <returns></returns>
        public List<Student> Cache_Get_StudentList()
        {

            return CacheHelper.Get<List<Student>>(studentKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<Student> list = db.Student.OrderByDescending(x => x.CreatedTime).ThenBy(x => x.ID).ToList();
                    return list;
                }
            });
        }

        /// <summary>
        /// 缓存 dic
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Student> Cache_Get_StudentList_Dic()
        {
            return Cache_Get_StudentList().ToDictionary(x => x.ID);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<Student>> Get_StudentPageList(
            int pageIndex,
            int pageSize,
            string name,
            string referenceId,
            string no,
            string mobile,
            string enteredPointId,
            string makeDriverShopId,
            string wantDriverShopId,
            StudentCode state,
            int moneyIsFull,
            int isOnSchool,
            int orderBy,
            DateTime? enteredTimeStart, DateTime? enteredTimeEnd,
            DateTime? makedTimeStart, DateTime? makeTimeEnd,
            bool isDelete = false
            )
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = entities.Student.AsQueryable().Where(x=>x.State!=StudentCode.HadDropOut).AsNoTracking();
                if (!isDelete)
                {
                    query = query.Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0);
                }
                else
                {
                    query = query.Where(x => (x.Flag & (long)GlobalFlag.Removed) != 0);
                }
                if (!Client.LoginUser.IsAdmin)
                {
                    
                    query = query.Where(x => Client.LoginUser.EnteredPointIDStr.Contains(x.EnteredPointID));
                    if (Client.LoginUser.IsStoreAdmin==YesOrNoCode.No)
                    {
                        query = query.Where(x => x.CreaterID.Equals(Client.LoginUser.ID));
                    }
                }
                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }
                if (no.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.IDCard.Contains(no));
                }
                if (mobile.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Mobile.Contains(mobile));
                }
                if (isOnSchool == -1)
                {
                    if ((int)state != -1 && !isDelete)
                    {
                        query = query.Where(x => x.State==state);
                    }
                }
                else
                {
                    if (isOnSchool == 1)
                    {
                        if ((int)state != -1 && !isDelete)
                        {
                            query = query.Where(x => x.State == state);
                        }
                        else
                        {
                            query = query.Where(x => x.State != StudentCode.WantDropOut && x.State != StudentCode.HadDropOut && x.State != StudentCode.Graduate);
                        }
                    }
                    else
                    {
                        query = query.Where(x => x.State == StudentCode.WantDropOut || x.State == StudentCode.HadDropOut || x.State == StudentCode.Graduate);
                    }
                }
                if (moneyIsFull != -1)
                {
                    var code = moneyIsFull == 1 ? YesOrNoCode.Yes : YesOrNoCode.No;
                    query = query.Where(x => x.MoneyIsFull == code);
                }
                if (referenceId.IsNotNullOrEmpty() && referenceId != "-1")
                {
                    query = query.Where(x => x.ReferenceID.Equals(referenceId));
                }

                if (enteredPointId.IsNotNullOrEmpty() && enteredPointId != "-1")
                {
                    query = query.Where(x => x.EnteredPointID.Equals(enteredPointId));
                }
                if (makeDriverShopId.IsNotNullOrEmpty() && makeDriverShopId != "-1")
                {
                    query = query.Where(x =>!string.IsNullOrEmpty(x.MakeDriverShopID) && x.MakeDriverShopID.Equals(makeDriverShopId));
                }
                if (wantDriverShopId.IsNotNullOrEmpty() && wantDriverShopId != "-1")
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.WantDriverShopID) && x.WantDriverShopID.Equals(wantDriverShopId));
                }
                if (enteredTimeStart != null)
                {
                    query = query.Where(x => x.EnteredDate >= enteredTimeStart);
                }
                if (enteredTimeEnd != null)
                {
                    enteredTimeEnd = enteredTimeEnd.Value.AddDays(1);
                    query = query.Where(x => x.EnteredDate < enteredTimeEnd);
                }

                if (makedTimeStart != null)
                {
                    query = query.Where(x => x.MakeCardDate >= makedTimeStart);
                }
                if (makeTimeEnd != null)
                {
                    makeTimeEnd = makeTimeEnd.Value.AddDays(1);
                    query = query.Where(x => x.MakeCardDate < makeTimeEnd);
                }
                var count = query.Count();
                if (orderBy == 0)
                {
                    query = query.OrderByDescending(x => x.EnteredDate).ThenByDescending(x => x.CreatedTime);
                }
                else if (orderBy == 1)
                {
                    query = query.OrderByDescending(x => x.CreatedTime);
                }
                else if (orderBy == 2)
                {
                    query = query.OrderByDescending(x => x.MakeCardDate).ThenByDescending(x => x.CreatedTime);
                }
                else if (orderBy == 3)
                {
                    query = query.OrderByDescending(x => x.ThemeOneDate).ThenByDescending(x => x.CreatedTime);
                }

                var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
               

                return ResultPageList(GetReturnList(list,entities), pageIndex, pageSize, count);
            }
        }

        public List<Student> GetReturnList(List<Student> list, DbRepository entities)
        {
            var referenceDic = Cache_Get_ReferenceList_Dic();
            var driverShopDic = Cache_Get_DriverShopList_Dic();
            // var areaDic = Cache_Get_DataDictionary()[GroupCode.Area];
            var enteredPointDic = Cache_Get_EnteredPoint_Dic();
            var cerDic = Cache_Get_DataDictionary()[GroupCode.Certificate];
            //var payMethodDic = Cache_Get_DataDictionary()[GroupCode.PayMethod];
            var trianDic = Cache_Get_DataDictionary()[GroupCode.Train];
            var userDic = Cache_Get_UserDic();
            var coachDic = Cache_Get_CoachList_Dic();
            var studentIdList = list.Select(x => x.ID).ToList();
            var payOrderList = Cache_Get_PayOrderList().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0);
            var examDic = Cache_Get_ExamList().Where(x => studentIdList.Contains(x.StudentID)).GroupBy(x => x.StudentID).ToDictionary(x => x.Key);
            list.ForEach(x =>
            {
                //报名地
                // if (!string.IsNullOrEmpty(x.EnteredCityCode) && areaDic.ContainsKey(x.EnteredCityCode))
                //   x.EnteredCityName = areaDic[x.EnteredCityCode]?.Value;

                //证书
                if (!string.IsNullOrEmpty(x.EnteredPointID) && enteredPointDic.ContainsKey(x.EnteredPointID))
                    x.EnteredPointName = enteredPointDic[x.EnteredPointID]?.Name;
                var parOrder = payOrderList.Where(y => y.StudentID.Equals(x.ID) && y.IsConfirm == YesOrNoCode.No).FirstOrDefault();
                if (parOrder != null)
                {
                    x.DoConfirmMoney = parOrder.PayMoney;
                }
                //制卡地
                //if (!string.IsNullOrEmpty(x.MakeCardCityCode) && areaDic.ContainsKey(x.MakeCardCityCode))
                // x.MakeCardCityName = areaDic[x.MakeCardCityCode]?.Value;
                //培训方式
                if (!string.IsNullOrEmpty(x.TrianID) && trianDic.ContainsKey(x.TrianID))
                    x.TrianName = trianDic[x.TrianID]?.Value;
                //制卡驾校
                if (!string.IsNullOrEmpty(x.MakeDriverShopID) && driverShopDic.ContainsKey(x.MakeDriverShopID))
                    x.MakeDriverShopName = driverShopDic[x.MakeDriverShopID]?.Name;

                //推荐人
                if (!string.IsNullOrEmpty(x.ReferenceID) && referenceDic.ContainsKey(x.ReferenceID))
                    x.ReferenceName = referenceDic[x.ReferenceID]?.Name;

                //科二教练
                if (!string.IsNullOrEmpty(x.ThemeTwoCoachID) && coachDic.ContainsKey(x.ThemeTwoCoachID))
                    x.ThemeTwoCoachName = coachDic[x.ThemeTwoCoachID]?.Name;

                //支付方式
                //if (!string.IsNullOrEmpty(x.PayMethodID) && payMethodDic.ContainsKey(x.PayMethodID))
                // x.PayMethodName = payMethodDic[x.PayMethodID]?.Value;

                //省
                //if (!string.IsNullOrEmpty(x.ProvinceCode) && areaDic.ContainsKey(x.ProvinceCode))
                // x.ProvinceName = areaDic[x.ProvinceCode]?.Value;
                //省
                // if (!string.IsNullOrEmpty(x.CityCode) && areaDic.ContainsKey(x.CityCode))
                // x.CityName = areaDic[x.CityCode]?.Value;

                //证书
                if (!string.IsNullOrEmpty(x.CertificateID) && cerDic.ContainsKey(x.CertificateID))
                    x.CertificateName = cerDic[x.CertificateID]?.Value;

                //修改人
                if (!string.IsNullOrEmpty(x.CreaterID) && userDic.ContainsKey(x.CreaterID))
                    x.CreaterName = userDic[x.CreaterID]?.Name;

                if (examDic.ContainsKey(x.ID))
                    x.ExamCount = examDic[x.ID].Where(y => y.Code == x.NowTheme).Count() + 1;
                else
                    x.ExamCount = 1;

                if (x.NowTheme == ThemeCode.Two || x.NowTheme == ThemeCode.Three)
                {
                    var otherNowTheme = x.NowTheme == ThemeCode.Two ? ThemeCode.Three : ThemeCode.Two;
                    x.OtherExamCount = examDic[x.ID].Where(y => y.Code == otherNowTheme).Count() + 1;
                }
            });
            return list;
        }

        /// <summary>
        ///导出 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public List<StudentExportModel> Export_StudentPageList(
            int pageIndex,
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
            bool isAll = false
            )
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = entities.Student.AsQueryable().Where(x => x.State != StudentCode.HadDropOut).AsNoTracking();
                if (!isAll)
                {

                    query = query.Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0);
                    if (name.IsNotNullOrEmpty())
                    {
                        query = query.Where(x => x.Name.Contains(name));
                    }
                    if (!Client.LoginUser.IsAdmin)
                    {
                        query = query.Where(x => Client.LoginUser.EnteredPointIDStr.Contains(x.EnteredPointID));
                    }
                    if (no.IsNotNullOrEmpty())
                    {
                        query = query.Where(x => x.IDCard.Contains(no));
                    }
                    if (mobile.IsNotNullOrEmpty())
                    {
                        query = query.Where(x => x.Mobile.Contains(mobile));
                    }
                    if ((int)state != -1)
                    {
                        query = query.Where(x => x.State.Equals(state));
                    }
                    if (referenceId.IsNotNullOrEmpty() && referenceId != "-1")
                    {
                        query = query.Where(x => x.ReferenceID.Equals(referenceId));
                    }

                    if (enteredPointId.IsNotNullOrEmpty() && enteredPointId != "-1")
                    {
                        query = query.Where(x => x.EnteredPointID.Equals(enteredPointId));
                    }
                    if (makeDriverShopId.IsNotNullOrEmpty() && makeDriverShopId != "-1")
                    {
                        query = query.Where(x => x.MakeDriverShopID.Equals(makeDriverShopId));
                    }
                    if (enteredTimeStart != null)
                    {
                        query = query.Where(x => x.EnteredDate >= enteredTimeStart);
                    }
                    if (enteredTimeEnd != null)
                    {
                        enteredTimeEnd = enteredTimeEnd.Value.AddDays(1);
                        query = query.Where(x => x.EnteredDate < enteredTimeEnd);
                    }

                    if (makedTimeStart != null)
                    {
                        query = query.Where(x => x.MakeCardDate >= makedTimeStart);
                    }
                    if (makeTimeEnd != null)
                    {
                        makeTimeEnd = makeTimeEnd.Value.AddDays(1);
                        query = query.Where(x => x.MakeCardDate < makeTimeEnd);
                    }
                    if (orderBy == 0)
                    {
                        query = query.OrderByDescending(x => x.EnteredDate).ThenByDescending(x => x.CreatedTime);
                    }
                    else if (orderBy == 1)
                    {
                        query = query.OrderByDescending(x => x.CreatedTime);
                    }
                    else if (orderBy == 2)
                    {
                        query = query.OrderByDescending(x => x.MakeCardDate).ThenByDescending(x => x.CreatedTime);
                    }
                    else if (orderBy == 3)
                    {
                        query = query.OrderByDescending(x => x.ThemeOneDate).ThenByDescending(x => x.CreatedTime);
                    }
                    query = query.OrderByDescending(x => x.EnteredDate).Skip((pageIndex - 1) * pageSize).Take(pageSize);
                }

                var list = query.ToList();
                var enPointDic = Cache_Get_EnteredPoint_Dic();
                var referenceDic = Cache_Get_ReferenceList_Dic();
                var driverShopDic = Cache_Get_DriverShopList_Dic();
                var areaDic = Cache_Get_DataDictionary()[GroupCode.Area];
                var enteredPointDic = Cache_Get_EnteredPoint_Dic();
                var cerDic = Cache_Get_DataDictionary()[GroupCode.Certificate];
                var payMethodDic = Cache_Get_DataDictionary()[GroupCode.PayMethod];
                var trianDic = Cache_Get_DataDictionary()[GroupCode.Train];
                var coachDic = Cache_Get_CoachList_Dic();
                var userDic = Cache_Get_UserDic();
                var returnList = new List<StudentExportModel>();
                list.ForEach(x =>
                {
                    var model = new StudentExportModel();

                    model.Name = x.Name;
                    model.IDCard = x.IDCard;
                    model.GenderCode = EnumHelper.GetEnumDescription(x.GenderCode);
                    //(x.GenderCode == GenderCode.Female ? "女" : (x.GenderCode == GenderCode.Male ? "男" : "未知"));
                    model.Address = x.Address;
                    model.Mobile = x.Mobile;
                    model.Money = x.Money.ToString();
                    model.MoneyIsFull = EnumHelper.GetEnumDescription(x.MoneyIsFull);
                    model.HadPayMoney = x.HadPayMoney.ToString();
                    model.Remark = x.Remark;
                    model.EnteredDate = x.EnteredDate.ToString("yyyy-MM-dd");
                    model.MakeCardDate = x.MakeCardDate.HasValue ? x.MakeCardDate.Value.ToString("yyyy-MM-dd") : "";
                    model.MakeCardRemark = x.MakeCardRemark;

                    model.ThemeOneDate = x.ThemeOneDate.HasValue ? x.ThemeOneDate.Value.ToString("yyyy-MM-dd") : "";
                    model.ThemeOnePass = EnumHelper.GetEnumDescription(x.ThemeOnePass);


                    model.ThemeTwoDate = x.ThemeTwoDate.HasValue ? x.ThemeTwoDate.Value.ToString("yyyy-MM-dd") : "";
                    model.ThemeTwoPass = EnumHelper.GetEnumDescription(x.ThemeTwoPass);
                    model.ThemeTwoTimeCode = EnumHelper.GetEnumDescription(x.ThemeTwoTimeCode);

                    model.ThemeThreeDate = x.ThemeThreeDate.HasValue ? x.ThemeThreeDate.Value.ToString("yyyy-MM-dd") : "";
                    model.ThemeThreePass = EnumHelper.GetEnumDescription(x.ThemeThreePass);
                    model.ThemeThreeTimeCode = EnumHelper.GetEnumDescription(x.ThemeThreeTimeCode);

                    model.ThemeFourDate = x.ThemeFourDate.HasValue ? x.ThemeFourDate.Value.ToString("yyyy-MM-dd") : "";
                    model.ThemeFourPass = EnumHelper.GetEnumDescription(x.ThemeFourPass);

                    model.State = EnumHelper.GetEnumDescription(x.State);
                    model.NowTheme = EnumHelper.GetEnumDescription(x.NowTheme);
                    model.DropOutDate = x.DropOutDate.HasValue ? x.DropOutDate.Value.ToString("yyyy-MM-dd") : "";

                    //报名地
                    if (!string.IsNullOrEmpty(x.EnteredProvinceCode) && areaDic.ContainsKey(x.EnteredProvinceCode))
                        model.EnteredProvinceName = areaDic[x.EnteredProvinceCode]?.Value;
                    //报名地
                    if (!string.IsNullOrEmpty(x.EnteredCityCode) && areaDic.ContainsKey(x.EnteredCityCode))
                        model.EnteredCityName = areaDic[x.EnteredCityCode]?.Value;
                    //制卡地
                    if (!string.IsNullOrEmpty(x.MakeCardCityCode) && areaDic.ContainsKey(x.MakeCardCityCode))
                        model.MakeCardCityName = areaDic[x.MakeCardCityCode]?.Value;
                    //培训方式
                    if (!string.IsNullOrEmpty(x.TrianID) && trianDic.ContainsKey(x.TrianID))
                        model.TrianName = trianDic[x.TrianID]?.Value;
                    //制卡驾校
                    if (!string.IsNullOrEmpty(x.WantDriverShopID) && driverShopDic.ContainsKey(x.WantDriverShopID))
                        model.WantDriverShopName = driverShopDic[x.WantDriverShopID]?.Name;
                    //制卡驾校
                    if (!string.IsNullOrEmpty(x.MakeDriverShopID) && driverShopDic.ContainsKey(x.MakeDriverShopID))
                        model.MakeDriverShopName = driverShopDic[x.MakeDriverShopID]?.Name;
                    //制卡地
                    if (!string.IsNullOrEmpty(x.MakeCardCityCode) && areaDic.ContainsKey(x.MakeCardCityCode))
                        model.MakeCardCityName = areaDic[x.MakeCardCityCode]?.Value;

                    //报名点
                    if (!string.IsNullOrEmpty(x.EnteredPointID) && enPointDic.ContainsKey(x.EnteredPointID))
                        model.EnteredPointName = enPointDic[x.EnteredPointID]?.Name;
                    //推荐人
                    if (!string.IsNullOrEmpty(x.ReferenceID) && referenceDic.ContainsKey(x.ReferenceID))
                        model.ReferenceName = referenceDic[x.ReferenceID]?.Name;
                    //支付方式
                    if (!string.IsNullOrEmpty(x.PayMethodID) && payMethodDic.ContainsKey(x.PayMethodID))
                        model.PayMethodName = payMethodDic[x.PayMethodID]?.Value;

                    //省
                    if (!string.IsNullOrEmpty(x.ProvinceCode) && areaDic.ContainsKey(x.ProvinceCode))
                        model.ProvinceName = areaDic[x.ProvinceCode]?.Value;
                    //省
                    if (!string.IsNullOrEmpty(x.CityCode) && areaDic.ContainsKey(x.CityCode))
                        model.CityName = areaDic[x.CityCode]?.Value;

                    //证书
                    if (!string.IsNullOrEmpty(x.CertificateID) && cerDic.ContainsKey(x.CertificateID))
                        model.CertificateName = cerDic[x.CertificateID]?.Value;

                    //科目二教练
                    if (!string.IsNullOrEmpty(x.ThemeThreeCoachID) && coachDic.ContainsKey(x.ThemeThreeCoachID))
                        model.ThemeThreeCoachName = coachDic[x.ThemeThreeCoachID]?.Name;
                    //科目三教练
                    if (!string.IsNullOrEmpty(x.ThemeTwoCoachID) && coachDic.ContainsKey(x.ThemeTwoCoachID))
                        model.ThemeTwoCoachName = coachDic[x.ThemeTwoCoachID]?.Name;
                    returnList.Add(model);
                });
                return returnList;

            }
        }


        /// <summary>
        ///导出 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<bool> ExportInto_Student(List<StudentExportModel> list
            )
        {
            if (list == null)
            {
                return Result(false, ErrorCode.param_null);
            }
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_StudentList().AsQueryable().AsNoTracking();
                var enPointDic = Cache_Get_EnteredPoint_Dic();
                var referenceDic = Cache_Get_ReferenceList_Dic();
                var driverShopDic = Cache_Get_DriverShopList_Dic();
                var areaDic = Cache_Get_DataDictionary()[GroupCode.Area];
                var enteredPointDic = Cache_Get_EnteredPoint_Dic();
                var cerDic = Cache_Get_DataDictionary()[GroupCode.Certificate];
                var payMethodDic = Cache_Get_DataDictionary()[GroupCode.PayMethod];
                var trianDic = Cache_Get_DataDictionary()[GroupCode.Train];
                var coachDic = Cache_Get_CoachList_Dic();
                var userDic = Cache_Get_UserDic();
                var returnList = new List<Student>();
                string msg = string.Empty;
                list.ForEach(x =>
                {
                    if (string.IsNullOrEmpty(msg))
                    {
                        var model = new Student();
                        model.ID = Guid.NewGuid().ToString("N");
                        model.CreatedTime = DateTime.Now;
                        model.Flag = (long)GlobalFlag.Normal;
                        model.UpdatedTime = DateTime.Now;
                        model.UpdaterID = Client.LoginUser.ID;
                        model.Name = x.Name;
                        model.IDCard = x.IDCard;
                        if (Cache_Get_StudentList().Where(y => y.IDCard.Equals(model.IDCard)).Any())
                        {
                            msg = "学生" + x.Name + "身份证已存在";
                        }
                        model.GenderCode = (GenderCode)EnumHelper.GetEnumKey(typeof(GenderCode), x.GenderCode);
                        //(x.GenderCode == GenderCode.Female ? "女" : (x.GenderCode == GenderCode.Male ? "男" : "未知"));
                        model.Address = x.Address;
                        model.Mobile = x.Mobile;
                        model.Money = x.Money.GetDecimal();
                        model.HadPayMoney = x.HadPayMoney.GetDecimal();
                        if (model.HadPayMoney > 0)
                        {
                            var newPay = new PayOrder()
                            {
                                ID = Guid.NewGuid().ToString("N"),
                                UpdatedTime = DateTime.Now,
                                CreatedTime = DateTime.Now,
                                Flag = (long)GlobalFlag.Normal,
                                IsConfirm = YesOrNoCode.Yes,
                                IsDrop = YesOrNoCode.No,
                                CreaterID = Client.LoginUser.ID,
                                UpdaterID = Client.LoginUser.ID,
                                StudentID = model.ID,
                                ConfirmUserID = Client.LoginUser.ID,
                                VoucherThum = "",
                                VoucherNO = x.VoucherNO,
                                ConfirmDate = DateTime.Now,
                                PayMoney = model.HadPayMoney,
                                PayTime = DateTime.Now

                            };
                            //培训方式
                            if (!string.IsNullOrEmpty(x.PayOrderTypeName))
                            {
                                var typeList = Get_DataDictorySelectItem(GroupCode.PayType);

                                var dicModel = typeList.Where(y => y.Text.Equals(x.PayOrderTypeName)).FirstOrDefault();
                                if (dicModel == null)
                                {
                                    msg = "学生" + x.Name + "支付渠道不存在,请新增";
                                }
                                else
                                {
                                    newPay.PayTypeID = dicModel.Value;
                                }
                            }
                            else
                            {
                                msg = "学生" + x.Name + "支付渠道为空";
                            }
                            entities.PayOrder.Add(newPay);

                        }
                        model.MoneyIsFull = (YesOrNoCode)EnumHelper.GetEnumKey(typeof(YesOrNoCode), x.MoneyIsFull);
                        model.Remark = x.Remark;
                        if (x.EnteredDate.IsNotNullOrEmpty())
                            model.EnteredDate = x.EnteredDate.GetDateTime();
                        if (x.MakeCardDate.IsNotNullOrEmpty())
                            model.MakeCardDate = x.MakeCardDate.GetDateTime();
                        model.MakeCardRemark = x.MakeCardRemark;

                        if (x.ThemeOneDate.IsNotNullOrEmpty())
                            model.ThemeOneDate = x.ThemeOneDate.GetDateTime();
                        model.ThemeOnePass = (YesOrNoCode)EnumHelper.GetEnumKey(typeof(YesOrNoCode), x.ThemeOnePass);


                        if (x.ThemeTwoDate.IsNotNullOrEmpty())
                            model.ThemeTwoDate = x.ThemeTwoDate.GetDateTime();
                        model.ThemeTwoPass = (YesOrNoCode)EnumHelper.GetEnumKey(typeof(YesOrNoCode), x.ThemeTwoPass);
                        model.ThemeTwoTimeCode = (ThemeTimeCode)EnumHelper.GetEnumKey(typeof(ThemeTimeCode), x.ThemeTwoTimeCode);

                        if (x.ThemeThreeDate.IsNotNullOrEmpty())
                            model.ThemeThreeDate = x.ThemeThreeDate.GetDateTime();
                        model.ThemeThreePass = (YesOrNoCode)EnumHelper.GetEnumKey(typeof(YesOrNoCode), x.ThemeThreePass);
                        model.ThemeThreeTimeCode = (ThemeTimeCode)EnumHelper.GetEnumKey(typeof(ThemeTimeCode), x.ThemeThreeTimeCode);

                        if (x.ThemeFourDate.IsNotNullOrEmpty())
                            model.ThemeFourDate = x.ThemeFourDate.GetDateTime();
                        model.ThemeFourPass = (YesOrNoCode)EnumHelper.GetEnumKey(typeof(YesOrNoCode), x.ThemeFourPass);

                        model.State = (StudentCode)EnumHelper.GetEnumKey(typeof(StudentCode), x.State);
                        model.NowTheme = (ThemeCode)EnumHelper.GetEnumKey(typeof(ThemeCode), x.NowTheme);
                        if (x.DropOutDate.IsNotNullOrEmpty())
                            model.DropOutDate = x.DropOutDate.GetDateTime();

                        model.ProvinceCode = x.IDCard.Substring(0, 2) + "0000";
                        model.CityCode = x.IDCard.Substring(0, 4) + "00";
                        model.GenderCode = (GenderCode)EnumHelper.GetEnumKey(typeof(GenderCode), x.GenderCode);

                        model.MakeCardRemark = x.MakeCardRemark;
                        if (!string.IsNullOrEmpty(x.MakeDriverShopName))
                        {
                            var makeShop = Get_DriverShopByName(x.MakeDriverShopName);
                            if (makeShop == null)
                            {
                                msg = "学生" + x.Name + "制卡驾校不存在,请新增该驾校";
                            }
                            else
                            {
                                model.MakeDriverShopID = makeShop.ID;
                                model.MakeCardCityCode = makeShop.CityCode;
                            }
                        }


                        //培训方式
                        if (!string.IsNullOrEmpty(x.TrianName))
                        {
                            var trianList = Get_DataDictorySelectItem(GroupCode.Train);

                            var dicModel = trianList.Where(y => y.Text.Equals(x.TrianName)).FirstOrDefault();
                            if (dicModel == null)
                            {
                                msg = "学生" + x.Name + "培训班别不存在,请新增";
                            }
                            else
                            {
                                model.TrianID = dicModel.Value;
                            }
                        }
                        else
                        {
                            msg = "学生" + x.Name + "培训班别为空";
                        }

                        //意向驾校
                        if (!string.IsNullOrEmpty(x.WantDriverShopName))
                        {
                            var wantShop = Get_DriverShopByName(x.WantDriverShopName);
                            if (wantShop == null)
                            {
                                msg = "学生" + x.Name + "意向驾校不存在,请新增该驾校";
                            }
                            else
                            {
                                model.WantDriverShopID = wantShop.ID;
                            }
                        }

                        //报名点
                        if (!string.IsNullOrEmpty(x.EnteredPointName))
                        {
                            var enPointModel = Get_EnteredPointByName(x.EnteredPointName);
                            if (enPointModel == null)
                            {
                                msg = "学生" + x.Name + "报名点不存在,请新增该报名点";
                            }
                            else
                            {
                                model.EnteredPointID = enPointModel.ID;
                                model.EnteredDate = x.EnteredDate.GetDateTime();
                                model.EnteredProvinceCode = enPointModel.ProvinceCode;
                                model.EnteredCityCode = enPointModel.CityCode;

                            }
                        }
                        else
                        {
                            msg = "学生" + x.Name + "报名点为空";
                        }

                        //推荐人
                        if (!string.IsNullOrEmpty(x.ReferenceName))
                        {
                            var reFerModel = Get_ReferenceByName(x.ReferenceName);
                            if (reFerModel == null)
                            {
                                msg = "学生" + x.Name + "报名点不存在,请新增该报名点";
                            }
                            else
                            {
                                model.ReferenceID = reFerModel.ID;

                            }
                        }
                        else
                        {
                            msg = "学生" + x.Name + "报名点为空";
                        }


                        //培训方式
                        if (!string.IsNullOrEmpty(x.PayMethodName))
                        {
                            var mehtodList = Get_DataDictorySelectItem(GroupCode.PayMethod);

                            var dicModel = mehtodList.Where(y => y.Text.Equals(x.PayMethodName)).FirstOrDefault();
                            if (dicModel == null)
                            {
                                msg = "学生" + x.Name + "支付方式不存在,请新增";
                            }
                            else
                            {
                                model.PayMethodID = dicModel.Value;
                            }
                        }
                        else
                        {
                            msg = "学生" + x.Name + "支付方式为空";
                        }
                        //证书
                        if (!string.IsNullOrEmpty(x.CertificateName))
                        {
                            var ceList = Get_DataDictorySelectItem(GroupCode.Certificate);

                            var dicModel = ceList.Where(y => y.Text.Equals(x.CertificateName)).FirstOrDefault();
                            if (dicModel == null)
                            {
                                msg = "学生" + x.Name + "证书类别不存在,请新增";
                            }
                            else
                            {
                                model.CertificateID = dicModel.Value;
                            }
                        }
                        else
                        {
                            msg = "学生" + x.Name + "证书类别为空";
                        }

                        //科二教练
                        if (!string.IsNullOrEmpty(x.ThemeTwoCoachName))
                        {
                            var reFerModel = Get_CoachByName(x.ThemeTwoCoachName);
                            if (reFerModel == null)
                            {
                                msg = "学生" + x.Name + "科二教练不存在,请新增";
                            }
                            else
                            {
                                model.ReferenceID = reFerModel.ID;

                            }
                        }

                        //科二教练
                        if (!string.IsNullOrEmpty(x.ThemeThreeCoachName))
                        {
                            var reFerModel = Get_CoachByName(x.ThemeThreeCoachName);
                            if (reFerModel == null)
                            {
                                msg = "学生" + x.Name + "科三教练不存在,请新增";
                            }
                            else
                            {
                                model.ReferenceID = reFerModel.ID;

                            }
                        }

                        returnList.Add(model);
                    }
                });

                if (msg.IsNotNullOrEmpty())
                {
                    return Result(false, ErrorCode.sys_fail, msg);
                }
                else
                {

                    entities.Student.AddRange(returnList);
                    if (entities.SaveChanges() > 0)
                    {
                        Cache_Get_StudentList().AddRange(returnList);
                        return Result(false, ErrorCode.sys_success);
                    }
                    else
                    {
                        return Result(false, ErrorCode.sys_fail);
                    }

                }
            }
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<Student>> Get_StudentMoreInfoPageList(
            int pageIndex,
            int pageSize,
            string name,
            string referenceID,
            string no,
            string trianID,
            string driverShopID,
            string themeTwoCoachID,
            string themeThreeCoachID,
            string enteredPointID,
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
            using (DbRepository entities = new DbRepository())
            {
  
                var query = entities.Student.AsQueryable().AsNoTracking().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0&&x.State != StudentCode.HadDropOut);
                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }
                if (referenceID.IsNotNullOrEmpty() && referenceID != "-1")
                {
                    query = query.Where(x => x.ReferenceID.Equals(referenceID));
                }
                if (enteredPointID.IsNotNullOrEmpty() && enteredPointID != "-1")
                {
                    query = query.Where(x => x.EnteredPointID.Equals(enteredPointID));
                }            
                if (driverShopID.IsNotNullOrEmpty() && driverShopID != "-1")
                {
                    query = query.Where(x => x.MakeDriverShopID.Equals(driverShopID));
                }
                if (no.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.IDCard.Contains(no));
                }
                if (!Client.LoginUser.IsAdmin)
                {

                    query = query.Where(x => Client.LoginUser.EnteredPointIDStr.Contains(x.EnteredPointID));
                    if (Client.LoginUser.IsStoreAdmin == YesOrNoCode.No)
                    {
                        query = query.Where(x => x.CreaterID.Equals(Client.LoginUser.ID));
                    }
                }
                if (trianID.IsNotNullOrEmpty() && trianID != "-1")
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.TrianID) && x.TrianID.Equals(trianID));
                }
                if (themeThreeCoachID.IsNotNullOrEmpty() && themeThreeCoachID != "-1")
                {
                    if (themeThreeCoachID.Equals("0"))
                        query = query.Where(x => string.IsNullOrEmpty(x.ThemeThreeCoachID));
                    else
                        query = query.Where(x => !string.IsNullOrEmpty(x.ThemeThreeCoachID) && x.ThemeThreeCoachID.Equals(themeThreeCoachID));
                }
                if (themeTwoCoachID.IsNotNullOrEmpty() && themeTwoCoachID != "-1")
                {
                    if(themeTwoCoachID.Equals("0"))
                        query = query.Where(x => string.IsNullOrEmpty(x.ThemeTwoCoachID));
                    else
                        query = query.Where(x => !string.IsNullOrEmpty(x.ThemeTwoCoachID) && x.ThemeTwoCoachID.Equals(themeTwoCoachID));
                }
                if ((int)themeTwoTimeCode != -1)
                {
                    query = query.Where(x => x.ThemeTwoTimeCode== themeTwoTimeCode);
                }
                if ((int)themeThreeTimeCode != -1)
                {
                    query = query.Where(x => x.ThemeThreeTimeCode == themeThreeTimeCode);
                }
                if (themeOnePass==YesOrNoCode.Yes)
                {
                    query = query.Where(x => x.ThemeOnePass==YesOrNoCode.Yes);
                }
                else if (themeOnePass==YesOrNoCode.No)
                {
                    query = query.Where(x => x.ThemeOnePass==YesOrNoCode.No);
                }

                if (themeTwoPass==YesOrNoCode.Yes)
                {
                    query = query.Where(x => x.ThemeTwoPass==YesOrNoCode.Yes);
                }
                else if (themeTwoPass==YesOrNoCode.No)
                {
                    query = query.Where(x => x.ThemeTwoPass==YesOrNoCode.No);
                }

                if (themeThreePass==YesOrNoCode.Yes)
                {
                    query = query.Where(x => x.ThemeThreePass==YesOrNoCode.Yes);
                }
                else if (themeThreePass==YesOrNoCode.No)
                {
                    query = query.Where(x => x.ThemeThreePass==YesOrNoCode.No);
                }

                if (themeFourPass==YesOrNoCode.Yes)
                {
                    query = query.Where(x => x.ThemeFourPass==YesOrNoCode.Yes);
                }
                else if (themeFourPass==YesOrNoCode.No)
                {
                    query = query.Where(x => x.ThemeFourPass==YesOrNoCode.No);
                }

                if (themeOneTimeStart != null)
                {
                    query = query.Where(x => x.ThemeOneDate >= themeOneTimeStart);
                }
                if (themeOneTimeEnd != null)
                {
                    themeOneTimeEnd = themeOneTimeEnd.Value.AddDays(1);
                    query = query.Where(x => x.ThemeOneDate < themeOneTimeEnd);
                }

                if (themeTwoTimeStart != null)
                {
                    query = query.Where(x => x.ThemeTwoDate >= themeTwoTimeStart);
                }
                if (themeTwoTimeEnd != null)
                {
                    themeTwoTimeEnd = themeTwoTimeEnd.Value.AddDays(1);
                    query = query.Where(x => x.ThemeTwoDate < themeTwoTimeEnd);
                }


                if (themeThreeTimeStart != null)
                {
                    query = query.Where(x => x.ThemeThreeDate >= themeThreeTimeStart);
                }
                if (themeThreeTimeEnd != null)
                {
                    themeThreeTimeEnd = themeThreeTimeEnd.Value.AddDays(1);
                    query = query.Where(x => x.ThemeThreeDate < themeThreeTimeEnd);
                }

                if (themeFourTimeStart != null)
                {
                    query = query.Where(x => x.ThemeFourDate >= themeFourTimeStart);
                }
                if (themeFourTimeEnd != null)
                {
                    themeFourTimeEnd = themeFourTimeEnd.Value.AddDays(1);
                    query = query.Where(x => x.ThemeFourDate < themeFourTimeEnd);
                }
                if (orderBy == 0)
                {
                    query = query.OrderByDescending(x => x.EnteredDate).ThenByDescending(x => x.CreatedTime);
                }
                else if (orderBy == 1)
                {
                    query = query.OrderByDescending(x => x.CreatedTime);
                }
                else if (orderBy == 2)
                {
                    query = query.OrderByDescending(x => x.MakeCardDate).ThenByDescending(x => x.CreatedTime);
                }
                else if (orderBy == 3)
                {
                    query = query.OrderByDescending(x => x.ThemeOneDate).ThenByDescending(x => x.CreatedTime);
                }
                var count = query.Count();
                var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var driverShopDic = Cache_Get_DriverShopList_Dic();
                var coachDic = Cache_Get_CoachList_Dic();
                //var trianDic = Cache_Get_DataDictionary()[GroupCode.Train];
                //var userDic = Cache_Get_UserDic();
                var referenceDic = Cache_Get_ReferenceList_Dic();
                var studentIdList = list.Select(x => x.ID).ToList();
                var cerDic = Cache_Get_DataDictionary()[GroupCode.Certificate];
                var examDic = Cache_Get_ExamList().Where(x => studentIdList.Contains(x.StudentID)).GroupBy(x => x.StudentID).ToDictionary(x => x.Key);
                list.ForEach(x =>
                {
                    //制卡驾校
                    if (!string.IsNullOrEmpty(x.WantDriverShopID) && driverShopDic.ContainsKey(x.WantDriverShopID))
                        x.WantDriverShopName = driverShopDic[x.WantDriverShopID]?.Name;

                    //推荐人
                    if (!string.IsNullOrEmpty(x.ReferenceID) && referenceDic.ContainsKey(x.ReferenceID))
                        x.ReferenceName = referenceDic[x.ReferenceID]?.Name;

                    if (examDic.ContainsKey(x.ID))
                        x.ExamCount = examDic[x.ID].Where(y => y.Code == x.NowTheme).Count() + 1;
                    else
                        x.ExamCount = 1;

                    if (x.NowTheme == ThemeCode.Two || x.NowTheme == ThemeCode.Three)
                    {
                        var otherNowTheme = x.NowTheme == ThemeCode.Two ? ThemeCode.Three : ThemeCode.Two;
                        x.OtherExamCount = examDic[x.ID].Where(y => y.Code == otherNowTheme).Count() + 1;
                    }

                    //培训方式
                    //if (!string.IsNullOrEmpty(x.TrianID) && trianDic.ContainsKey(x.TrianID))
                    //x.TrianName = trianDic[x.TrianID]?.Value;
                    //制卡驾校
                    if (!string.IsNullOrEmpty(x.MakeDriverShopID) && driverShopDic.ContainsKey(x.MakeDriverShopID))
                        x.MakeDriverShopName = driverShopDic[x.MakeDriverShopID]?.Name;

                    //证书
                    if (!string.IsNullOrEmpty(x.CertificateID) && cerDic.ContainsKey(x.CertificateID))
                        x.CertificateName = cerDic[x.CertificateID]?.Value;
                    //科目二教练
                    if (!string.IsNullOrEmpty(x.ThemeThreeCoachID) && coachDic.ContainsKey(x.ThemeThreeCoachID))
                        x.ThemeThreeCoachName = coachDic[x.ThemeThreeCoachID]?.Name;
                    //科目三教练
                    if (!string.IsNullOrEmpty(x.ThemeTwoCoachID) && coachDic.ContainsKey(x.ThemeTwoCoachID))
                        x.ThemeTwoCoachName = coachDic[x.ThemeTwoCoachID]?.Name;
                    //修改人
                    //if (!string.IsNullOrEmpty(x.UpdaterID) && userDic.ContainsKey(x.UpdaterID))
                    //x.UpdaterName = userDic[x.UpdaterID]?.Name;
                });

                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<Student>> Get_StudentSchoolPageList(
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
            using (DbRepository entities = new DbRepository())
            {
                var query = entities.Student.AsQueryable().AsNoTracking().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0&&x.State != StudentCode.HadDropOut);
                if (!Client.LoginUser.IsAdmin)
                {

                    query = query.Where(x => Client.LoginUser.EnteredPointIDStr.Contains(x.EnteredPointID));
                    if (Client.LoginUser.IsStoreAdmin == YesOrNoCode.No)
                    {
                        query = query.Where(x => x.CreaterID.Equals(Client.LoginUser.ID));
                    }
                }
                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }
                if (schoolID.IsNotNullOrEmpty() && schoolID != "0")
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.SchoolID) && x.SchoolID.Equals(schoolID));
                }
                if (collegeID.IsNotNullOrEmpty() && collegeID != "0")
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.CollegeID) && x.CollegeID.Equals(collegeID));
                }
                if (majorID.IsNotNullOrEmpty() && majorID != "0")
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.MajorID) && x.MajorID.Equals(majorID));
                }
                if (age.IsNotNullOrEmpty())
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.SchoolAge) && x.SchoolAge.Contains(age));
                }
                if (wantDriverShopID.IsNotNullOrEmpty() && wantDriverShopID != "-1")
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.WantDriverShopID) && x.WantDriverShopID.Contains(wantDriverShopID));
                }
                if (makeDriverShopID.IsNotNullOrEmpty() && makeDriverShopID != "-1")
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.MakeDriverShopID) && x.MakeDriverShopID.Contains(makeDriverShopID));
                }
                if (provinceCode.IsNotNullOrEmpty() && provinceCode != "0")
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.ProvinceCode) && x.ProvinceCode.Contains(provinceCode));
                }
                if (cityCode.IsNotNullOrEmpty() && cityCode != "0")
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.CityCode) && x.CityCode.Contains(cityCode));
                }
                if (orderBy == 0)
                {
                    query = query.OrderByDescending(x => x.EnteredDate).ThenByDescending(x => x.CreatedTime);
                }
                else if (orderBy == 1)
                {
                    query = query.OrderByDescending(x => x.CreatedTime);
                }
                else if (orderBy == 2)
                {
                    query = query.OrderByDescending(x => x.MakeCardDate).ThenByDescending(x => x.CreatedTime);
                }
                else if (orderBy == 3)
                {
                    query = query.OrderByDescending(x => x.ThemeOneDate).ThenByDescending(x => x.CreatedTime);
                }
                var count = query.Count();
                var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var driverShopDic = Cache_Get_DriverShopList_Dic();
                var referenceDic = Cache_Get_ReferenceList_Dic();
                var studentIdList = list.Select(x => x.ID).ToList();
                var areaDic = Cache_Get_DataDictionary()[GroupCode.Area];
                var schoolDic = Cache_Get_DataDictionary()[GroupCode.School];
                var colelgeDic = Cache_Get_DataDictionary()[GroupCode.College];
                var majorDic = Cache_Get_DataDictionary()[GroupCode.Major];
                var examDic = Cache_Get_ExamList().Where(x => studentIdList.Contains(x.StudentID)).GroupBy(x => x.StudentID).ToDictionary(x => x.Key);
                list.ForEach(x =>
                {
                    //意向驾校
                    if (!string.IsNullOrEmpty(x.WantDriverShopID) && driverShopDic.ContainsKey(x.WantDriverShopID))
                        x.WantDriverShopName = driverShopDic[x.WantDriverShopID]?.Name;
                    //制卡驾校
                    if (!string.IsNullOrEmpty(x.MakeDriverShopID) && driverShopDic.ContainsKey(x.MakeDriverShopID))
                        x.MakeDriverShopName = driverShopDic[x.MakeDriverShopID]?.Name;
                    //省
                    if (!string.IsNullOrEmpty(x.ProvinceCode) && areaDic.ContainsKey(x.ProvinceCode))
                        x.ProvinceName = areaDic[x.ProvinceCode]?.Value;
                    //省
                    if (!string.IsNullOrEmpty(x.CityCode) && areaDic.ContainsKey(x.CityCode))
                        x.CityName = areaDic[x.CityCode]?.Value;

                    //推荐人
                    if (!string.IsNullOrEmpty(x.ReferenceID) && referenceDic.ContainsKey(x.ReferenceID))
                        x.ReferenceName = referenceDic[x.ReferenceID]?.Name;

                    if (examDic.ContainsKey(x.ID))
                        x.ExamCount = examDic[x.ID].Where(y => y.Code == x.NowTheme).Count() + 1;
                    else
                        x.ExamCount = 1;
                    if (x.NowTheme == ThemeCode.Two || x.NowTheme == ThemeCode.Three)
                    {
                        var otherNowTheme = x.NowTheme == ThemeCode.Two ? ThemeCode.Three : ThemeCode.Two;
                        x.OtherExamCount = examDic[x.ID].Where(y => y.Code == otherNowTheme).Count() + 1;
                    }
                    //高校
                    if (!string.IsNullOrEmpty(x.SchoolID) && schoolDic.ContainsKey(x.SchoolID))
                        x.SchoolName = schoolDic[x.SchoolID]?.Value;
                    //院校
                    if (!string.IsNullOrEmpty(x.CollegeID) && colelgeDic.ContainsKey(x.CollegeID))
                        x.CollegeName = colelgeDic[x.CollegeID]?.Value;
                    //专业
                    if (!string.IsNullOrEmpty(x.MajorID) && majorDic.ContainsKey(x.MajorID))
                        x.MajorName = majorDic[x.MajorID]?.Value;

                    //修改人
                    //if (!string.IsNullOrEmpty(x.UpdaterID) && userDic.ContainsKey(x.UpdaterID))
                    //x.UpdaterName = userDic[x.UpdaterID]?.Name;
                });

                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }



        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_Student(Student model)
        {
            if (model.MakeCardDate.HasValue && model.MakeCardDate.Value < model.EnteredDate)
            {
                return Result(false, ErrorCode.make_card_time_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                if (Cache_Get_StudentList().AsQueryable().AsNoTracking().Where(x => x.IDCard.Equals(model.IDCard)).Any())
                    return Result(false, ErrorCode.datadatabase_idcards__had);
                if (!string.IsNullOrEmpty(model.MakeDriverShopID))
                {
                    var makecardShop = Cache_Get_DriverShopList().FirstOrDefault(x => x.ID.Equals(model.MakeDriverShopID));
                    if (makecardShop == null)
                        return Result(false, ErrorCode.sys_param_format_error);
                    model.MakeCardCityCode = makecardShop.CityCode;
                }
                if(model.Money==0)
                {
                    model.MoneyIsFull = YesOrNoCode.Yes;
                }
                if (model.MakeDriverShopID == "-1")
                    model.MakeDriverShopID = string.Empty;
                if (!string.IsNullOrEmpty(model.MakeDriverShopID) && model.MakeCardDate.HasValue)
                {
                    model.State = StudentCode.ThemeOne;
                    model.NowTheme = ThemeCode.One;
                }
                else
                {
                    model.State = StudentCode.DontMakeCard;
                }
                if (model.SchoolID.IsNotNullOrEmpty() && model.SchoolID.Equals("-1"))
                {
                    model.SchoolID = null;
                }
                if (model.CollegeID.IsNotNullOrEmpty() && model.CollegeID.Equals("0"))
                {
                    model.CollegeID = null;
                }
                if (model.MajorID.IsNotNullOrEmpty() && model.MajorID.Equals("0"))
                {
                    model.MajorID = null;
                }
                model.CreatedTime = DateTime.Now;
                model.UpdatedTime = DateTime.Now;
                model.UpdaterID=model.CreaterID = Client.LoginUser.ID;
                model.Flag = (long)GlobalFlag.Normal;
                if(model.Money!=0)
                    model.MoneyIsFull = YesOrNoCode.No;
                else
                    model.MoneyIsFull = YesOrNoCode.Yes;
                //if (model.ThemeOnePass == YesOrNoCode.Yes)
                //{
                //    model.NowTheme = ThemeCode.One;
                //}
                //if (model.ThemeTwoPass == YesOrNoCode.Yes)
                //    model.NowTheme = ThemeCode.Two;
                //if (model.ThemeThreePass == YesOrNoCode.Yes)
                //    model.NowTheme = ThemeCode.Three;
                //if (model.ThemeFourPass == YesOrNoCode.Yes)
                //{
                //    model.NowTheme = ThemeCode.Four;
                //}
                entities.Student.Add(model);
                Add_Log(LogCode.AddStudent, model.ID, string.Format("{0}在{1}新增了学员{2}", Client.LoginUser.Name, DateTime.Now.ToString(), model.Name), "", "", "");
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_StudentList();
                    list.Add(model);
                    return Result(true);
                }
                else
                {
                    return Result(false, ErrorCode.sys_fail);
                }
            }

        }

        /// <summary>
        /// 获取搜索集合
        /// </summary>
        /// <returns></returns>
        public WebResult<StudentIndexModel> Get_SelectItemList(string id, bool isAll)
        {
            var referenceList = Cache_Get_ReferenceList();
            if (!isAll)
            {
                referenceList = referenceList.Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0).ToList();
            }

            var driverShopList = Cache_Get_DriverShopList().ToList();
            driverShopList = driverShopList.Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0).ToList();
            var enteredPointList = Cache_Get_EnteredPointList();
            if (!Client.LoginUser.CoachID.IsNotNullOrEmpty())
            {
                enteredPointList = enteredPointList.Where(x => !Client.LoginUser.IsAdmin ? (Client.LoginUser.EnteredPointIDStr.IsNotNullOrEmpty() && Client.LoginUser.EnteredPointIDStr.Contains(x.ID)) : 1 == 1).ToList();
            }
            enteredPointList = enteredPointList.Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0).ToList();
            var coachList = Cache_Get_CoachList().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0||!x.IsQuit).ToList();

            //if (dsid.IsNotNullOrEmpty())
            //    coachList = coachList.Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0 && x.DriverShopID.Equals(dsid)).ToList();
            //else
            coachList = coachList.Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0).ToList();
            var majorList = new List<SelectItem>();
            var collegeList = new List<SelectItem>();
            if (id != null)
            {
                var student = Cache_Get_StudentList_Dic().ContainsKey(id) ? Cache_Get_StudentList_Dic()[id] : null;
                if (student != null)
                {
                    collegeList = Get_DataDictorySelectItem(GroupCode.College, x => (x.ParentKey == student.SchoolID));
                    majorList = Get_DataDictorySelectItem(GroupCode.Major, x => (x.ParentKey == student.CollegeID));
                }
            }
            return Result(new StudentIndexModel()
            {
                ReferenceList = referenceList,
                DriverShopList = driverShopList,
                EnteredPointList = enteredPointList,
                CoachList = coachList,
                CertificateList = Get_DataDictorySelectItem(GroupCode.Certificate),
                PayMethodList = Get_DataDictorySelectItem(GroupCode.PayMethod),
                TrainList = Get_DataDictorySelectItem(GroupCode.Train),
                PayTypeList = Get_DataDictorySelectItem(GroupCode.PayType),
                AccountList = Get_DataDictorySelectItem(GroupCode.Account),
                CollegeList = collegeList,
                MajorList = majorList,
                SchoolList = Get_DataDictorySelectItem(GroupCode.School),
            });
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update_Student(Student model)
        {
            if (model == null
                 || !model.Name.IsNotNullOrEmpty()
                )
                return Result(false, ErrorCode.sys_param_format_error);
            if (model.MakeCardDate.HasValue && model.MakeCardDate.Value < model.EnteredDate)
            {
                return Result(false, ErrorCode.make_card_time_error);
            }
            using (DbRepository entities = new DbRepository())
            {

                var list = Cache_Get_StudentList();
                var oldEntity = entities.Student.Find(model.ID);
                if (oldEntity != null)
                {
                    if (list.AsQueryable().Where(x => x.Name.Equals(model.IDCard) && !x.ID.Equals(model.ID)).Any())
                        return Result(false, ErrorCode.datadatabase_idcards__had);

                    //修改前
                    string beforeJson = oldEntity.ToJson();

                    if (!string.IsNullOrEmpty(model.MakeDriverShopID))
                    {
                        if (Cache_Get_DriverShopList_Dic().ContainsKey(model.MakeDriverShopID))
                        {

                            oldEntity.MakeCardCityCode = Cache_Get_DriverShopList_Dic()[model.MakeDriverShopID].CityCode;
                        }
                    }
                    if ((!oldEntity.MakeCardDate.HasValue || string.IsNullOrEmpty(model.MakeDriverShopID)) && !string.IsNullOrEmpty(model.MakeDriverShopID) && model.MakeCardDate.HasValue)
                    {
                        oldEntity.State = StudentCode.ThemeOne;
                        oldEntity.NowTheme = ThemeCode.One;
                    }
                    oldEntity.IDCard = model.IDCard;
                    oldEntity.Mobile = model.Mobile;
                    oldEntity.Name = model.Name;
                    oldEntity.GenderCode = model.GenderCode;
                    oldEntity.Address = model.Address;
                    oldEntity.ProvinceCode = model.ProvinceCode;
                    oldEntity.CityCode = model.CityCode;
                    oldEntity.Remark = model.Remark;
                    oldEntity.EnteredProvinceCode = model.EnteredProvinceCode;
                    oldEntity.EnteredCityCode = model.EnteredCityCode;
                    oldEntity.EnteredDate = model.EnteredDate;
                    oldEntity.EnteredPointID = model.EnteredPointID;
                    oldEntity.ReferenceID = model.ReferenceID;
                    oldEntity.WantDriverShopID = model.WantDriverShopID;
                    oldEntity.TrianID = model.TrianID;
                    oldEntity.Money = model.Money;
                    oldEntity.PayMethodID = model.PayMethodID;
                    oldEntity.MakeDriverShopID = model.MakeDriverShopID;
                    oldEntity.MakeCardRemark = model.MakeCardRemark;
                    oldEntity.CertificateID = model.CertificateID;
                    oldEntity.ThemeTwoTimeCode = model.ThemeTwoTimeCode;
                    oldEntity.ThemeThreeTimeCode = model.ThemeThreeTimeCode;
                    if (oldEntity.Money <= oldEntity.HadPayMoney)
                    {
                        oldEntity.MoneyIsFull = YesOrNoCode.Yes;
                    }
                    //oldEntity.ThemeOneDate = model.ThemeOneDate;
                    //oldEntity.ThemeTwoPass = model.ThemeOnePass;

                    //oldEntity.ThemeTwoDate = model.ThemeTwoDate;
                    //oldEntity.ThemeTwoCoachID = model.ThemeTwoCoachID;
                    //oldEntity.ThemeTwoPass = model.ThemeTwoPass;

                    //oldEntity.ThemeThreeDate = model.ThemeThreeDate;
                    //oldEntity.ThemeThreeCoachID = model.ThemeThreeCoachID;
                    //oldEntity.ThemeThreePass = model.ThemeThreePass;

                    //oldEntity.ThemeFourDate = model.ThemeFourDate;
                    //oldEntity.ThemeFourPass = model.ThemeFourPass;

                    oldEntity.MakeCardDate = model.MakeCardDate;
                    //if (model.ThemeOnePass == YesOrNoCode.Yes)
                    //    oldEntity.NowTheme = ThemeCode.One;
                    //if (model.ThemeTwoPass == YesOrNoCode.Yes)
                    //    oldEntity.NowTheme = ThemeCode.Two;
                    //if (model.ThemeThreePass == YesOrNoCode.Yes)
                    //    oldEntity.NowTheme = ThemeCode.Three;
                    //if (model.ThemeFourPass == YesOrNoCode.Yes)
                    //{
                    //    oldEntity.NowTheme = ThemeCode.Four;
                    //    oldEntity.State = StudentCode.Graduate;
                    //}

                    oldEntity.From = model.From;
                    oldEntity.SchoolID = model.SchoolID;
                    oldEntity.CollegeID = model.CollegeID;
                    oldEntity.MajorID = model.MajorID;
                    oldEntity.SchoolAge = model.SchoolAge;
                    if (model.SchoolID.IsNotNullOrEmpty() && model.SchoolID.Equals("-1"))
                    {
                        oldEntity.SchoolID = null;
                    }
                    if (model.CollegeID.IsNotNullOrEmpty() && model.CollegeID.Equals("0"))
                    {
                        oldEntity.CollegeID = null;
                    }
                    if (model.MajorID.IsNotNullOrEmpty() && model.MajorID.Equals("0"))
                    {
                        oldEntity.MajorID = null;
                    }

                    oldEntity.UpdatedTime = DateTime.Now;
                    oldEntity.UpdaterID = Client.LoginUser.ID;
                    string afterJSon = oldEntity.ToJson();
                    string info = SearchModifyHelper.CompareProperty<Student, Student>(Cache_Get_StudentList_Dic()[oldEntity.ID], oldEntity);
                    Add_Log(LogCode.UpdateStudent, oldEntity.ID, string.Format("{0}在{1}编辑{2}的信息", Client.LoginUser.Name, DateTime.Now.ToString(), oldEntity.Name), beforeJson, afterJSon, info);
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var index = list.FindIndex(x => x.ID.Equals(model.ID));
                    if (index > -1)
                    {
                        list[index] = oldEntity;
                    }
                    else
                    {
                        list.Add(oldEntity);
                    }
                    return Result(true);
                }
                else
                {
                    return Result(false, ErrorCode.sys_fail);
                }
            }

        }


        /// <summary>
        /// 修改教练
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update_StudentDriver(string id, string makeDriverShopID, DateTime makeCardDate)
        {
            if (!id.IsNotNullOrEmpty() || !makeDriverShopID.IsNotNullOrEmpty()
                )
                return Result(false, ErrorCode.sys_param_format_error);
            using (DbRepository entities = new DbRepository())
            {

                var list = Cache_Get_StudentList();
                var oldEntity = entities.Student.Find(id);
                if (oldEntity != null)
                {
                    string beforeJson = oldEntity.ToJson();
                    oldEntity.MakeDriverShopID = makeDriverShopID;
                    oldEntity.MakeCardDate = makeCardDate;
                    oldEntity.UpdatedTime = DateTime.Now;
                    oldEntity.UpdaterID = Client.LoginUser.ID;
                    oldEntity.State = StudentCode.ThemeOne;
                    oldEntity.NowTheme = ThemeCode.One;
                    string afterJSon = oldEntity.ToJson();
                    string info = SearchModifyHelper.CompareProperty<Student, Student>(Cache_Get_StudentList_Dic()[oldEntity.ID], oldEntity);
                    Add_Log(LogCode.UpdateStudent, oldEntity.ID, string.Format("{0}在{1}编辑{2}的信息,修改制卡院校和制卡时间", Client.LoginUser.Name, DateTime.Now.ToString(), oldEntity.Name), beforeJson, afterJSon, info);
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var index = list.FindIndex(x => x.ID.Equals(id));
                    if (index > -1)
                    {
                        list[index] = oldEntity;
                    }
                    else
                    {
                        list.Add(oldEntity);
                    }
                    return Result(true);
                }
                else
                {
                    return Result(false, ErrorCode.sys_fail);
                }
            }

        }


        /// <summary>
        /// 修改教练
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update_BatchStudentDriver(string ids, string makeDriverShopID, DateTime makeCardDate)
        {
            if (!ids.IsNotNullOrEmpty() || !makeDriverShopID.IsNotNullOrEmpty()
                )
                return Result(false, ErrorCode.sys_param_format_error);
            using (DbRepository entities = new DbRepository())
            {

                var list = Cache_Get_StudentList();
                entities.Student.Where(x => ids.Contains(x.ID)).ToList().ForEach(oldEntity =>
                {
                    if (oldEntity.State == StudentCode.DontMakeCard)
                    {
                        string beforeJson = oldEntity.ToJson();
                        oldEntity.MakeDriverShopID = makeDriverShopID;
                        oldEntity.MakeCardDate = makeCardDate;
                        oldEntity.UpdatedTime = DateTime.Now;
                        oldEntity.UpdaterID = Client.LoginUser.ID;
                        oldEntity.State = StudentCode.ThemeOne;
                        oldEntity.NowTheme = ThemeCode.One;
                        string afterJSon = oldEntity.ToJson();
                        string info = SearchModifyHelper.CompareProperty<Student, Student>(Cache_Get_StudentList_Dic()[oldEntity.ID], oldEntity);
                        Add_Log(LogCode.UpdateStudent, oldEntity.ID, string.Format("{0}在{1}编辑{2}的信息,修改制卡院校和制卡时间", Client.LoginUser.Name, DateTime.Now.ToString(), oldEntity.Name), beforeJson, afterJSon, info);
                        var index = list.FindIndex(x => x.ID.Equals(oldEntity.ID));
                        if (index > -1)
                        {
                            list[index] = oldEntity;
                        }
                        else
                        {
                            list.Add(oldEntity);
                        }
                    }
                });
                if (entities.SaveChanges() > 0)
                {
                    return Result(true);
                }
                else
                {
                    return Result(false, ErrorCode.sys_fail);
                }
            }

        }

        /// <summary>
        /// 修改教练
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update_BatchStudentCoach(string ids, string themeTwoCoachID, string themeThreeCoachID)
        {
            if (!ids.IsNotNullOrEmpty() || !themeTwoCoachID.IsNotNullOrEmpty()
                )
                return Result(false, ErrorCode.sys_param_format_error);
            using (DbRepository entities = new DbRepository())
            {

                var list = Cache_Get_StudentList();
                entities.Student.Where(x => ids.Contains(x.ID)).ToList().ForEach(oldEntity =>
                {
                    //在校才能改
                    if (oldEntity.State != StudentCode.DontMakeCard && oldEntity.State != StudentCode.WantDropOut && oldEntity.State != StudentCode.Graduate && oldEntity.State != StudentCode.HadDropOut)
                    {
                        if ((oldEntity.ThemeTwoCoachID.IsNotNullOrEmpty()&&!oldEntity.ThemeTwoCoachID.Equals(themeTwoCoachID))|| oldEntity.ThemeTwoCoachID.IsNullOrEmpty() || oldEntity.ThemeThreeCoachID.IsNullOrEmpty() || (oldEntity.ThemeThreeCoachID.IsNotNullOrEmpty()&&!oldEntity.ThemeThreeCoachID.Equals(themeThreeCoachID)))
                        {
                            string beforeJson = oldEntity.ToJson();

                            //修改前
                            if (themeTwoCoachID.IsNotNullOrEmpty() && themeTwoCoachID.Equals("-1"))
                            {
                                oldEntity.ThemeTwoCoachID = string.Empty;
                            }
                            else
                            {
                                oldEntity.ThemeTwoCoachID = themeTwoCoachID;
                                oldEntity.ThemeTwoConfirm = YesOrNoCode.No;
                            }

                            if (themeThreeCoachID.IsNotNullOrEmpty() && themeThreeCoachID.Equals("-1"))
                            {
                                oldEntity.ThemeThreeCoachID = string.Empty;
                            }
                            else
                            {
                                oldEntity.ThemeThreeCoachID = themeThreeCoachID;
                                oldEntity.ThemeThreeConfirm = YesOrNoCode.No;
                            }

                            oldEntity.UpdatedTime = DateTime.Now;
                            oldEntity.UpdaterID = Client.LoginUser.ID;
                            string afterJSon = oldEntity.ToJson();
                            string info = SearchModifyHelper.CompareProperty<Student, Student>(Cache_Get_StudentList_Dic()[oldEntity.ID], oldEntity);
                            Add_Log(LogCode.UpdateStudent, oldEntity.ID, string.Format("{0}在{1}编辑{2}的信息,分配教练", Client.LoginUser.Name, DateTime.Now.ToString(), oldEntity.Name), beforeJson, afterJSon, info);
                            var index = list.FindIndex(x => x.ID.Equals(oldEntity.ID));
                            if (index > -1)
                            {
                                list[index] = oldEntity;
                            }
                            else
                            {
                                list.Add(oldEntity);
                            }
                        }
                    }
                });

                if (entities.SaveChanges() > 0)
                {
                    return Result(true);
                }
                else
                {
                    return Result(false, ErrorCode.sys_fail);
                }
            }

        }

        public WebResult<bool> Update_StudentCreater(string ID, string createrID)
        {
            if (!ID.IsNotNullOrEmpty() || !createrID.IsNotNullOrEmpty() || createrID=="-1"
              )
                return Result(false, ErrorCode.sys_param_format_error);
            using (DbRepository entities = new DbRepository())
            {

                var list = Cache_Get_StudentList();
                var oldEntity = entities.Student.Find(ID);
                if (oldEntity != null)
                {
                    string oldCreaterName = oldEntity.CreaterID;
                    oldEntity.CreaterID = createrID;
                    oldEntity.UpdatedTime = DateTime.Now;
                    oldEntity.UpdaterID = Client.LoginUser.ID;
                    Add_Log(LogCode.UpdateStudent, oldEntity.ID, string.Format("{0}在{1}编辑{2}的信息,修改创建者", Client.LoginUser.Name, DateTime.Now.ToString(), oldEntity.Name), oldCreaterName, createrID, "");
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var index = list.FindIndex(x => x.ID.Equals(ID));
                    if (index > -1)
                    {
                        list[index] = oldEntity;
                    }
                    else
                    {
                        list.Add(oldEntity);
                    }
                    return Result(true);
                }
                else
                {
                    return Result(false, ErrorCode.sys_fail);
                }
            }
        }
        /// <summary>
        /// 修改教练
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update_StudentCoach(string id, string themeTwoCoachID, string themeThreeCoachID)
        {
            if (!id.IsNotNullOrEmpty() || !themeTwoCoachID.IsNotNullOrEmpty()
                )
                return Result(false, ErrorCode.sys_param_format_error);
            using (DbRepository entities = new DbRepository())
            {

                var list = Cache_Get_StudentList();
                var oldEntity = entities.Student.Find(id);
                if (oldEntity != null)
                {
                    string beforeJson = oldEntity.ToJson();
                    //if (oldEntity.NowTheme != ThemeCode.Two || oldEntity.NowTheme != ThemeCode.Three)
                    //{
                    //    return Result(false, ErrorCode.themecode_no_allow);
                    //}
                    //if (oldEntity.NowTheme == ThemeCode.Two && string.IsNullOrEmpty(themeTwoCoachID))
                    //{
                    //    return Result(false, ErrorCode.themetwo_no_had_coach);
                    //}
                    //if (oldEntity.NowTheme == ThemeCode.Three && (string.IsNullOrEmpty(themeThreeCoachID)|| string.IsNullOrEmpty(themeTwoCoachID)))
                    //{
                    //    return Result(false, ErrorCode.themethree_no_had_coach);
                    //}
                    //修改前
                    if (themeTwoCoachID.IsNotNullOrEmpty() && themeTwoCoachID.Equals("-1"))
                    {
                        oldEntity.ThemeTwoCoachID = string.Empty;
                    }
                    else
                    {
                        oldEntity.ThemeTwoCoachID = themeTwoCoachID;
                        oldEntity.ThemeTwoConfirm = YesOrNoCode.No;
                    }

                    if (themeThreeCoachID.IsNotNullOrEmpty() && themeThreeCoachID.Equals("-1"))
                    {
                        oldEntity.ThemeThreeCoachID = string.Empty;
                    }
                    else
                    {
                        oldEntity.ThemeThreeCoachID = themeThreeCoachID;
                        oldEntity.ThemeThreeConfirm = YesOrNoCode.No;
                    }
                    oldEntity.UpdatedTime = DateTime.Now;
                    oldEntity.UpdaterID = Client.LoginUser.ID;
                    string afterJSon = oldEntity.ToJson();
                    string info = SearchModifyHelper.CompareProperty<Student, Student>(Cache_Get_StudentList_Dic()[oldEntity.ID], oldEntity);
                    Add_Log(LogCode.UpdateStudent, oldEntity.ID, string.Format("{0}在{1}编辑{2}的信息,分配教练", Client.LoginUser.Name, DateTime.Now.ToString(), oldEntity.Name), beforeJson, afterJSon, info);
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var index = list.FindIndex(x => x.ID.Equals(id));
                    if (index > -1)
                    {
                        list[index] = oldEntity;
                    }
                    else
                    {
                        list.Add(oldEntity);
                    }
                    return Result(true);
                }
                else
                {
                    return Result(false, ErrorCode.sys_fail);
                }
            }

        }

        /// <summary>
        /// 修改学时
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update_StudentTimeCode(string id, ThemeTimeCode themeTwoTimeCode, ThemeTimeCode themeThreeTimeCode)
        {
            if (!id.IsNotNullOrEmpty()
                )
                return Result(false, ErrorCode.sys_param_format_error);
            using (DbRepository entities = new DbRepository())
            {

                var list = Cache_Get_StudentList();
                var oldEntity = entities.Student.Find(id);
                if (oldEntity != null)
                {
                    if (oldEntity.NowTheme == ThemeCode.One)
                    {
                        return Result(false, ErrorCode.themeont_no_pass);
                    }
                    string beforeJson = oldEntity.ToJson();
                    //if (oldEntity.NowTheme != ThemeCode.Two || oldEntity.NowTheme != ThemeCode.Three)
                    //{
                    //    return Result(false, ErrorCode.themecode_no_allow);
                    //}
                    //if (oldEntity.NowTheme == ThemeCode.Two && string.IsNullOrEmpty(themeTwoCoachID))
                    //{
                    //    return Result(false, ErrorCode.themetwo_no_had_coach);
                    //}
                    //if (oldEntity.NowTheme == ThemeCode.Three && (string.IsNullOrEmpty(themeThreeCoachID) || string.IsNullOrEmpty(themeTwoCoachID)))
                    //{
                    //    return Result(false, ErrorCode.themethree_no_had_coach);
                    //}
                    //修改前

                    oldEntity.ThemeTwoTimeCode = themeTwoTimeCode;
                    oldEntity.ThemeThreeTimeCode = themeThreeTimeCode;
                    oldEntity.UpdatedTime = DateTime.Now;
                    oldEntity.UpdaterID = Client.LoginUser.ID;
                    string afterJSon = oldEntity.ToJson();
                    string info = SearchModifyHelper.CompareProperty<Student, Student>(Cache_Get_StudentList_Dic()[oldEntity.ID], oldEntity);
                    Add_Log(LogCode.UpdateStudent, oldEntity.ID, string.Format("{0}在{1}编辑{2}的信息，记录学时", Client.LoginUser.Name, DateTime.Now.ToString(), oldEntity.Name), beforeJson, afterJSon, info);
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var index = list.FindIndex(x => x.ID.Equals(id));
                    if (index > -1)
                    {
                        list[index] = oldEntity;
                    }
                    else
                    {
                        list.Add(oldEntity);
                    }
                    return Result(true);
                }
                else
                {
                    return Result(false, ErrorCode.sys_fail);
                }
            }

        }

        /// <summary>
        /// 修改学时
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update_BatchStudentTimeCode(string ids, ThemeTimeCode themeTwoTimeCode, ThemeTimeCode themeThreeTimeCode)
        {
            if (!ids.IsNotNullOrEmpty()
                )
                return Result(false, ErrorCode.sys_param_format_error);
            using (DbRepository entities = new DbRepository())
            {

                var list = Cache_Get_StudentList();
                entities.Student.Where(x => ids.Contains(x.ID)).ToList().ForEach(oldEntity =>
                {
                    //在校才能改
                    if (oldEntity.State == StudentCode.ThemeOne || oldEntity.State == StudentCode.ThemeTwo)
                    {
                        if (oldEntity.ThemeTwoTimeCode != themeTwoTimeCode || oldEntity.ThemeThreeTimeCode != themeThreeTimeCode)
                        {
                            string beforeJson = oldEntity.ToJson();

                            oldEntity.ThemeTwoTimeCode = themeTwoTimeCode;
                            oldEntity.ThemeThreeTimeCode = themeThreeTimeCode;
                            oldEntity.UpdatedTime = DateTime.Now;
                            oldEntity.UpdaterID = Client.LoginUser.ID;
                            string afterJSon = oldEntity.ToJson();
                            string info = SearchModifyHelper.CompareProperty<Student, Student>(Cache_Get_StudentList_Dic()[oldEntity.ID], oldEntity);
                            Add_Log(LogCode.UpdateStudent, oldEntity.ID, string.Format("{0}在{1}编辑{2}的信息,分配教练", Client.LoginUser.Name, DateTime.Now.ToString(), oldEntity.Name), beforeJson, afterJSon, info);
                            var index = list.FindIndex(x => x.ID.Equals(oldEntity.ID));
                            if (index > -1)
                            {
                                list[index] = oldEntity;
                            }
                            else
                            {
                                list.Add(oldEntity);
                            }
                        }
                    }
                });

                if (entities.SaveChanges() > 0)
                {
                    return Result(true);
                }
                else
                {
                    return Result(false, ErrorCode.sys_fail);
                }
            }

        }

       
        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Delete_Student(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_StudentList();
                ErrorCode code = ErrorCode.sys_success;
                //找到实体
                entities.Student.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    if (Cache_Get_PayOrderList().Where(y => y.IsConfirm == YesOrNoCode.No && x.Flag == 0 && y.StudentID.Equals(x.ID)).Any())
                    {
                        code = ErrorCode.cant_delete_unconfirm_payorder__had;
                    }
                    else
                    {
                        x.Flag = x.Flag | (long)GlobalFlag.Removed;
                        Add_Log(LogCode.DeleteStudent, x.ID, string.Format("{0}在{1}删除了学员{2}", Client.LoginUser.Name, DateTime.Now.ToString(), x.Name), "", "", "");
                        var index = list.FindIndex(y => y.ID.Equals(x.ID));
                        if (index > -1)
                        {
                            list[index] = x;
                        }
                        else
                        {
                            list.Add(x);
                        }
                    }
                });
                if (code != ErrorCode.sys_success)
                {
                    return Result(false, code);
                }
                if (entities.SaveChanges() > 0)
                {
                    return Result(true);
                }
                else
                {
                    return Result(false, ErrorCode.sys_fail);
                }
            }
        }


        /// <summary>
        /// 申请退学
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public WebResult<bool> WantDrop_Student(PayOrder model, string remark)
        {
            using (DbRepository entities = new DbRepository())
            {

                var studentList = Cache_Get_StudentList();
                var list = Cache_Get_PayOrderList();


                if (list.Where(x => x.StudentID.Equals(model.StudentID) && x.IsConfirm == YesOrNoCode.No && x.IsDrop == YesOrNoCode.No&&x.Flag==0).Any())
                {
                    return Result(false, ErrorCode.cant_drop_unconfirm_payorder__had);
                }


                //找到实体
                var student = entities.Student.Find(model.StudentID);

                if (student == null)
                    return Result(false, ErrorCode.sys_param_format_error);

                student.State = StudentCode.WantDropOut;
                student.UpdatedTime = DateTime.Now;
                student.Remark = remark;
                var payOrder = new PayOrder()
                {
                    StudentID = model.StudentID,
                    PayMoney = student.HadPayMoney,
                    CreaterID = Client.LoginUser.ID,
                    IsConfirm = YesOrNoCode.No,
                    ID = Guid.NewGuid().ToString("N"),
                    PayTypeID = model.PayTypeID,
                    AccountNO = model.AccountNO,
                    WantDropMoney = model.WantDropMoney,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now,
                    IsDrop = YesOrNoCode.Yes,
                    WantDropDate = DateTime.Now,
                    Flag = (long)GlobalFlag.Normal,
                    UpdaterID = Client.LoginUser.ID,
                    PayTime = DateTime.Now

                };
                student.DropOutPayOrderId = payOrder.ID;
                entities.PayOrder.Add(payOrder);
                if (entities.SaveChanges() > 0)
                {
                    list.Add(payOrder);
                    var studentIndex = studentList.FindIndex(y => y.ID.Equals(payOrder.StudentID));
                    if (studentIndex > -1)
                    {
                        studentList[studentIndex] = student;
                    }
                    else
                    {
                        studentList.Add(student);
                    }

                    return Result(true);
                }
                else
                {
                    return Result(false, ErrorCode.sys_fail);
                }
            }
        }


        /// <summary>
        /// 获取教练员学员考试信息 和工资
        /// </summary>
        /// <param name="time"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Dictionary<string,Dictionary<StudentCode, int>> Get_StudenState(DateTime? startTime, DateTime? endTime, SteteReportEnum? state)
        {

            //科目二是该教练的学员
            var query = Cache_Get_StudentList().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0);
            if (startTime != null)
            {
                query = query.Where(x => x.EnteredDate > startTime);
            }
            if (endTime != null)
            {
                query = query.Where(x => x.EnteredDate < endTime);
            }
            var studentList = query.ToList();
            var dic =new  Dictionary<SteteReportEnum, Dictionary<string, Dictionary<StudentCode, int>>>();

            if (state == null)
            {
                state = SteteReportEnum.School;
            }
            #region 驾校
            int i = 1;
            if (state == SteteReportEnum.School)
            {
                var schoolDic = new Dictionary<string, Dictionary<StudentCode, int>>();
                var schoolList = Cache_Get_DriverShopList().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0).ToList();
                if (schoolList != null && schoolList.Count > 0)
                {
                    schoolList.ForEach(x =>
                    {
                        // 不包含科目2和科目3和未制卡
                        var scooDic = studentList.Where(y => y.State != StudentCode.DontMakeCard&& y.State != StudentCode.ThemeTwo&& y.State != StudentCode.ThemeThree && !string.IsNullOrEmpty(y.MakeDriverShopID) && y.MakeDriverShopID == x.ID).GroupBy(y => y.State).ToDictionary(y => y.Key, y => y.Count());
                        //未制卡为意向驾校
                        scooDic.Add(StudentCode.DontMakeCard, studentList.Where(y => y.State == StudentCode.DontMakeCard && !string.IsNullOrEmpty(y.WantDriverShopID) && y.WantDriverShopID == x.ID).Count());
                        //科目二 科目一过，科目三未通过
                        scooDic.Add(StudentCode.ThemeTwo, studentList.Where(y => y.State == StudentCode.ThemeTwo&&y.ThemeThreePass== YesOrNoCode.No && y.ThemeOnePass == YesOrNoCode.Yes && !string.IsNullOrEmpty(y.MakeDriverShopID) && y.MakeDriverShopID == x.ID).Count());
                        //科三过 科目一、科三过，科目二未通过
                        scooDic.Add(StudentCode.ThemeThreePass, studentList.Where(y => y.State == StudentCode.ThemeTwo && y.ThemeThreePass == YesOrNoCode.Yes && y.ThemeOnePass == YesOrNoCode.Yes && !string.IsNullOrEmpty(y.MakeDriverShopID) && y.MakeDriverShopID == x.ID).Count());
                        //科目三 科目一、科二过，科目三未通过
                        scooDic.Add(StudentCode.ThemeThree, studentList.Where(y => y.State == StudentCode.ThemeThree && y.ThemeTwoPass == YesOrNoCode.Yes && y.ThemeOnePass == YesOrNoCode.Yes && !string.IsNullOrEmpty(y.MakeDriverShopID) && y.MakeDriverShopID == x.ID).Count());
                        schoolDic.Add(i+"."+x.Name, scooDic);
                        i++;
                    });
                }
            dic.Add(SteteReportEnum.School, schoolDic);
            }
            #endregion

            #region 教练员
            i = 1;
            if (state == SteteReportEnum.Coach)
            {
                var coachDic = new Dictionary<string, Dictionary<StudentCode, int>>();
                var coachList = Cache_Get_CoachList().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0).ToList();
                if (coachList != null && coachList.Count > 0)
                {
                    coachList.ForEach(x =>
                    {
                        //不包含科目三 科三过
                        var stateDic=studentList.Where(y => !string.IsNullOrEmpty(y.ThemeTwoCoachID) && y.ThemeTwoCoachID == x.ID).GroupBy(y => y.State).ToDictionary(y => y.Key, y => y.Count());

                        //科目三 （科目三为该教练。科目一、二通过科目三未过的）
                        stateDic[StudentCode.ThemeThree]=studentList.Where(y => y.State == StudentCode.ThemeThree && y.ThemeTwoPass == YesOrNoCode.Yes && y.ThemeOnePass == YesOrNoCode.Yes && !string.IsNullOrEmpty(y.ThemeThreeCoachID) && y.ThemeThreeCoachID == x.ID).Count();
                        //科三过 （科目二为该教练，科目一、三通过科目二未过的）
                        stateDic.Add(StudentCode.ThemeThreePass, studentList.Where(y => (y.State == StudentCode.ThemeTwo && y.ThemeThreePass == YesOrNoCode.Yes && y.ThemeOnePass == YesOrNoCode.Yes && !string.IsNullOrEmpty(y.ThemeTwoCoachID) && y.ThemeTwoCoachID == x.ID)).Count());
                        //科三未 科目二为该教练，科目一、二通过科目三未过的
                        stateDic.Add(StudentCode.ThemeThreeNoPass, studentList.Where(y => (y.State == StudentCode.ThemeThree && y.ThemeOnePass == YesOrNoCode.Yes && y.ThemeTwoPass == YesOrNoCode.Yes && !string.IsNullOrEmpty(y.ThemeTwoCoachID) && y.ThemeTwoCoachID == x.ID)).Count());
                        coachDic.Add(i + "." + x.Name, stateDic);
                        i++;
                    });
                }
                dic.Add(SteteReportEnum.Coach, coachDic);
            }
            #endregion

            #region 报名点
            i = 1;
            if (state == SteteReportEnum.EnteredPoint)
            {
                var enPointDic = new Dictionary<string, Dictionary<StudentCode, int>>();
                var enpointList = Cache_Get_EnteredPointList().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0).ToList();
                if (enpointList != null && enpointList.Count > 0)
                {
                    enpointList.ForEach(x =>
                    {
                        var enpoDic = studentList.Where(y => y.EnteredPointID == x.ID).GroupBy(y => y.State).ToDictionary(y => y.Key, y => y.Count());
                        //科三过 科目一、科三过，科目二未通过
                        enpoDic.Add(StudentCode.ThemeThreePass, studentList.Where(y => y.State == StudentCode.ThemeTwo && y.ThemeThreePass == YesOrNoCode.Yes && y.ThemeOnePass == YesOrNoCode.Yes && y.EnteredPointID == x.ID).Count());

                        enPointDic.Add(i + "." + x.Name, enpoDic);
                        i++;
                    });
                }
                dic.Add(SteteReportEnum.EnteredPoint, enPointDic);
            }
            #endregion

            #region 推荐人 
            i = 1;
            if (state == SteteReportEnum.Reference)
            {
                var referenceDic = new Dictionary<string, Dictionary<StudentCode, int>>();
                var referenceList = Cache_Get_ReferenceList().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0).ToList();
                var refeDic = new Dictionary<StudentCode, int>();
                if (referenceList != null && referenceList.Count > 0)
                {
                    referenceList.ForEach(x =>
                    {
                        if (x.ID == "5cff6d4e43004032a2f79ec8ff046d8b")
                        {
                            var s= 11;
                        }
                        var referDic = studentList.Where(y => !string.IsNullOrEmpty(y.ReferenceID) && y.ReferenceID == x.ID).GroupBy(y => y.State).ToDictionary(y => y.Key, y => y.Count());
                        //科三过 科目一、科三过，科目二未通过
                        referDic.Add(StudentCode.ThemeThreePass, studentList.Where(y => y.State == StudentCode.ThemeTwo && y.ThemeThreePass == YesOrNoCode.Yes && y.ThemeOnePass == YesOrNoCode.Yes && y.ReferenceID == x.ID).Count());
                        referenceDic.Add(i + "." + x.Name, referDic);
                        i++;
                    });
                }
                dic.Add(SteteReportEnum.Reference, referenceDic);
            }
            #endregion

            return dic[state.Value];
        }

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Student Find_Student(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            using (DbRepository entities = new DbRepository())
            {
                var model = Cache_Get_StudentList().FirstOrDefault(x => x.ID.Equals(id));
                model.ExamCount = Cache_Get_ExamList().Where(x => x.StudentID.Equals(id) && x.Code == model.NowTheme).Count() + 1;
                if (model.NowTheme == ThemeCode.Two || model.NowTheme == ThemeCode.Three)
                {
                    var otherNowTheme = model.NowTheme == ThemeCode.Two ? ThemeCode.Three : ThemeCode.Two;
                    model.OtherExamCount= Cache_Get_ExamList().Where(x => x.StudentID.Equals(id) && x.Code == otherNowTheme).Count() + 1;
                }
                return model;
            }
        }
    }
}
