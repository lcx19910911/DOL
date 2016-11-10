using DOL.Core;
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
        private List<Student> Cache_Get_StudentList()
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
            StudentCode state,
            DateTime? enteredTimeStart, DateTime? enteredTimeEnd,
            DateTime? makedTimeStart, DateTime? makeTimeEnd,
            bool isDelete=false
            )
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_StudentList().AsQueryable().AsNoTracking();
                if (!isDelete)
                {
                    query = query.Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0);
                }
                else
                {
                    query = query.Where(x => (x.Flag & (long)GlobalFlag.Removed) != 0);
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
                if ((int)state != -1&&!isDelete)
                {
                    query = query.Where(x => x.State.Equals(state));
                }
                if (referenceId.IsNotNullOrEmpty()&& referenceId!="-1")
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
                var count = query.Count();
                var list = query.OrderByDescending(x => x.EnteredDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var referenceDic = Cache_Get_ReferenceList_Dic();
                var driverShopDic = Cache_Get_DriverShopList_Dic();
                var areaDic = Cache_Get_DataDictionary()[GroupCode.Area];
                var enteredPointDic = Cache_Get_EnteredPoint_Dic();
                var cerDic = Cache_Get_DataDictionary()[GroupCode.Certificate];
                var payMethodDic = Cache_Get_DataDictionary()[GroupCode.PayMethod];
                var trianDic = Cache_Get_DataDictionary()[GroupCode.Train];
                var userDic = Cache_Get_UserDic();
                list.ForEach(x =>
                {
                    //报名地
                    if (!string.IsNullOrEmpty(x.EnteredCityCode) && areaDic.ContainsKey(x.EnteredCityCode))
                        x.EnteredCityName = areaDic[x.EnteredCityCode]?.Value;
                    //制卡地
                    if (!string.IsNullOrEmpty(x.MakeCardCityCode) && areaDic.ContainsKey(x.MakeCardCityCode))
                        x.MakeCardCityName = areaDic[x.MakeCardCityCode]?.Value;
                    //培训方式
                    if (!string.IsNullOrEmpty(x.TrianID) && trianDic.ContainsKey(x.TrianID))
                        x.TrianName = trianDic[x.TrianID]?.Value;
                    //制卡驾校
                    if (!string.IsNullOrEmpty(x.MakeDriverShopID) && driverShopDic.ContainsKey(x.MakeDriverShopID))
                        x.MakeDriverShopName = driverShopDic[x.MakeDriverShopID]?.Name;

                    //推荐人
                    if (!string.IsNullOrEmpty(x.ReferenceID) && referenceDic.ContainsKey(x.ReferenceID))
                        x.ReferenceName = referenceDic[x.ReferenceID]?.Name;
                    //支付方式
                    if (!string.IsNullOrEmpty(x.PayMethodID) && payMethodDic.ContainsKey(x.PayMethodID))
                        x.PayMethodName = payMethodDic[x.PayMethodID]?.Value;

                    //省
                    if (!string.IsNullOrEmpty(x.ProvinceCode) && areaDic.ContainsKey(x.ProvinceCode))
                        x.ProvinceName = areaDic[x.ProvinceCode]?.Value;
                    //省
                    if (!string.IsNullOrEmpty(x.CityCode) && areaDic.ContainsKey(x.CityCode))
                        x.CityName = areaDic[x.CityCode]?.Value;

                    //证书
                    if (!string.IsNullOrEmpty(x.CertificateID) && cerDic.ContainsKey(x.CertificateID))
                        x.CertificateName = cerDic[x.CertificateID]?.Value;

                    //修改人
                    if (!string.IsNullOrEmpty(x.UpdaterID) && userDic.ContainsKey(x.UpdaterID))
                        x.UpdaterName = userDic[x.UpdaterID]?.Name;
                });

                return ResultPageList(list, pageIndex, pageSize, count);
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
            DateTime? enteredTimeStart, DateTime? enteredTimeEnd,
            DateTime? makedTimeStart, DateTime? makeTimeEnd,
            bool isAll = false
            )
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_StudentList().AsQueryable().AsNoTracking();
                if (!isAll)
                {

                    query = query.Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0);
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
                }
                var list = query.OrderByDescending(x => x.EnteredDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
                    model.MakeCardDate = x.MakeCardDate.HasValue? x.MakeCardDate.Value.ToString("yyyy-MM-dd"):"";
                    model.MakeCardRemark = x.MakeCardRemark;

                    model.ThemeOneDate = x.ThemeOneDate.HasValue ? x.ThemeOneDate.Value.ToString("yyyy-MM-dd") : "" ;
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
                                ConfirmUserID = Client.LoginUser.ID,
                                VoucherThum = "",
                                VoucherNO = x.VoucherNO,
                                ConfirmDate = DateTime.Now,
                                PayMoney = model.HadPayMoney
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
                        if(x.EnteredDate.IsNotNullOrEmpty())
                            model.EnteredDate =  x.EnteredDate.GetDateTime();
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
                    return Result(false, ErrorCode.sys_fail,msg);
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
            string no,
            string trianID,
            string driverShopID,
            string themeTwoCoachID,
            string themeThreeCoachID,
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
                var query = Cache_Get_StudentList().AsQueryable().AsNoTracking().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0);
                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }
                if (no.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.IDCard.Contains(no));
                }

                if (trianID.IsNotNullOrEmpty() && trianID != "-1")
                {
                    query = query.Where(x =>!string.IsNullOrEmpty(x.TrianID)&&x.TrianID.Equals(trianID));
                }
                if (themeTwoCoachID.IsNotNullOrEmpty() && themeTwoCoachID != "-1")
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.ThemeTwoCoachID) && x.ThemeTwoCoachID.Equals(themeTwoCoachID));
                }
                if (themeThreeCoachID.IsNotNullOrEmpty() && themeThreeCoachID != "-1")
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.ThemeThreeCoachID) && x.ThemeThreeCoachID.Equals(themeThreeCoachID));
                }
                if (themeTwoCoachID.IsNotNullOrEmpty() && themeTwoCoachID != "-1")
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.ThemeTwoCoachID) && x.ThemeTwoCoachID.Equals(themeTwoCoachID));
                }

                if (themeOnePass!=null&& (int)themeOnePass!=-1)
                {
                    query = query.Where(x => x.ThemeOnePass.Equals(themeOnePass));
                }
                if (themeTwoPass != null && (int)themeOnePass != -1)
                {
                    query = query.Where(x => x.ThemeTwoPass.Equals(themeTwoPass));
                }
                if (themeThreePass != null && (int)themeOnePass != -1)
                {
                    query = query.Where(x => x.ThemeThreePass.Equals(themeThreePass));
                }
                if (themeFourPass != null && (int)themeOnePass != -1)
                {
                    query = query.Where(x => x.ThemeFourPass.Equals(themeFourPass));
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

                var count = query.Count();
                var list = query.OrderByDescending(x => x.EnteredDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var driverShopDic = Cache_Get_DriverShopList_Dic();
                var coachDic = Cache_Get_CoachList_Dic(); 
                var trianDic = Cache_Get_DataDictionary()[GroupCode.Train];
                var userDic = Cache_Get_UserDic();
                list.ForEach(x =>
                {
                    //培训方式
                    if (!string.IsNullOrEmpty(x.TrianID) && trianDic.ContainsKey(x.TrianID))
                        x.TrianName = trianDic[x.TrianID]?.Value;
                    //制卡驾校
                    if (!string.IsNullOrEmpty(x.MakeDriverShopID) && driverShopDic.ContainsKey(x.MakeDriverShopID))
                        x.MakeDriverShopName = driverShopDic[x.MakeDriverShopID]?.Name;

                    //科目二教练
                    if (!string.IsNullOrEmpty(x.ThemeThreeCoachID) && coachDic.ContainsKey(x.ThemeThreeCoachID))
                        x.ThemeThreeCoachName = coachDic[x.ThemeThreeCoachID]?.Name;
                    //科目三教练
                    if (!string.IsNullOrEmpty(x.ThemeTwoCoachID) && coachDic.ContainsKey(x.ThemeTwoCoachID))
                        x.ThemeTwoCoachName = coachDic[x.ThemeTwoCoachID]?.Name;
                    //修改人
                    if (!string.IsNullOrEmpty(x.UpdaterID) && userDic.ContainsKey(x.UpdaterID))
                        x.UpdaterName = userDic[x.UpdaterID]?.Name;
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
                if (!string.IsNullOrEmpty(model.MakeDriverShopID) && model.MakeCardDate.HasValue)
                {
                    model.State = StudentCode.ThemeOne;
                    model.NowTheme = ThemeCode.One;
                }
                model.CreatedTime = DateTime.Now;
                model.UpdatedTime = DateTime.Now;
                model.State = StudentCode.DontMakeCard;
                model.UpdaterID = Client.LoginUser.ID;
                model.Flag = (long)GlobalFlag.Normal;
                if (model.MakeDriverShopID == "-1")
                    model.MakeDriverShopID = string.Empty;
                model.MoneyIsFull = YesOrNoCode.No;
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
                Add_Log(LogCode.AddStudent, model.ID, string.Format("{0}在{1}新增了学员{2}", Client.LoginUser.Name, DateTime.Now.ToString(), model.Name), "", "");
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
        public WebResult<StudentIndexModel> Get_SelectItemList()
        {
            var referenceList = Cache_Get_ReferenceList();
            var driverShopList = Cache_Get_DriverShopList();
            var enteredPointList = Cache_Get_EnteredPointList().Where(x => Client.LoginUser.MenuFlag != -1 ? (Client.LoginUser.EnteredPointIDStr.Contains(x.ID)) : 1 == 1).ToList();
            var coachList = Cache_Get_CoachList();
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
                AccountList = Get_DataDictorySelectItem(GroupCode.Account)
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
                    if ((!oldEntity.MakeCardDate.HasValue||string.IsNullOrEmpty(model.MakeDriverShopID)) && !string.IsNullOrEmpty(model.MakeDriverShopID)&&model.MakeCardDate.HasValue)
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
                    
                    oldEntity.ThemeOneDate = model.ThemeOneDate;
                    oldEntity.ThemeTwoPass = model.ThemeOnePass;

                    oldEntity.ThemeTwoDate = model.ThemeTwoDate;
                    oldEntity.ThemeTwoCoachID = model.ThemeTwoCoachID;
                    oldEntity.ThemeTwoPass = model.ThemeTwoPass;

                    oldEntity.ThemeThreeDate = model.ThemeThreeDate;
                    oldEntity.ThemeThreeCoachID = model.ThemeThreeCoachID;
                    oldEntity.ThemeThreePass = model.ThemeThreePass;

                    oldEntity.ThemeFourDate = model.ThemeFourDate;
                    oldEntity.ThemeFourPass = model.ThemeFourPass;

                    oldEntity.IsAddCertificate = model.IsAddCertificate;
                    if (model.IsAddCertificate == YesOrNoCode.Yes)
                        oldEntity.OldCertificate = model.OldCertificate;

                    oldEntity.ThemeTwoTimeCode = model.ThemeTwoTimeCode;
                    oldEntity.ThemeThreeTimeCode = model.ThemeThreeTimeCode;

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
                    oldEntity.UpdatedTime = DateTime.Now;
                    oldEntity.UpdaterID = Client.LoginUser.ID;
                    string afterJSon = oldEntity.ToJson();

                    Add_Log(LogCode.UpdateStudent, oldEntity.ID, string.Format("{0}在{1}编辑{2}的信息", Client.LoginUser.Name, DateTime.Now.ToString(), oldEntity.Name),beforeJson,afterJSon);
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

        //private string GetTextHT(string JsonStr)
        //{
        //    Dictionary<string, string> hs = new Dictionary<string, string>();
        //    JsonStr.Replace(@"'Name'","姓名");
        //    JsonStr.Replace(@"'IDCard'", "身份证");
        //    JsonStr.Replace(@"'GenderCode'", "性别");
        //    JsonStr.Replace(@"'Name'", "姓名");
        //    JsonStr.Replace(@"'Name'", "姓名");
        //    JsonStr.Replace(@"'Name'", "姓名");
        //    JsonStr.Replace(@"'Name'", "姓名");
        //    JsonStr.Replace(@"'Name'", "姓名");
        //    JsonStr.Replace(@"'Name'", "姓名");
        //    JsonStr.Replace(@"'Name'", "姓名");
        //    JsonStr.Replace(@"'Name'", "姓名");
        //    JsonStr.Replace(@"'Name'", "姓名");
        //    JsonStr.Replace(@"'Name'", "姓名");
        //    hs["IDCard"] = "身份证";
        //    hs["GenderCode"] = "性别";
        //    hs["ProvinceCode"] = "省份";
        //    hs["CityCode"] = "市";
        //    hs["Address"] = "地址";
        //    hs["Mobile"] = "手机号";
        //    hs["CertificateID"] = "证书";
        //    hs["EnteredPointID"] = "报名点";
        //    hs["ReferenceID"] = "推荐人";
        //    hs["WantDriverShopID"] = "意向驾校";
        //    hs["TrianID"] = "培训班别";
        //    hs["Money"] = "费用";
        //    hs["HadPayMoney"] = "已交费用";
        //    hs["MoneyIsFull"] = "是否缴清";
        //    hs["PayMethodID"] = "缴费方式";
        //    hs["Remark"] = "备注";
        //    hs["EnteredDate"] = "报名时间";
        //    hs["EnteredProvinceCode"] = "报名省份";
        //    hs["EnteredCityCode"] = "报名市";

        //    hs["MakeDriverShopID"] = "制卡驾校";
        //    hs["MakeCardDate"] = "制卡日期";
        //    hs["MakeCardCityCOde"] = "制卡地";
        //    hs["MakeCardRemark"] = "制卡备注";
        //    hs["ThemeOneDate"] = "科一时间";
        //    hs["ThemeOnePass"] = "科一是否通过";
        //    hs["ThemeTwoDate"] = "科二时间";
        //    hs["ThemeTwoPass"] = "科二是否通过";
        //    hs["ThemeTwoTimeCode"] = "科二学时状态";
        //    hs["ThemeTwoCoachID"] = "科二教练";
        //    hs["ThemeThreeDate"] = "科三时间";
        //    hs["ThemeThreePass"] = "科三是否通过";
        //    hs["ThemeThreeTimeCode"] = "科三学时状态";
        //    hs["ThemeThreeCoachID"] = "科三教练";
        //    hs["ThemeFourDate"] = "科四时间";
        //    hs["ThemeFourPass"] = "科四是否通过";
        //    hs["State"] = "学员状态";
        //    hs["NowTheme"] = "当前科目";
        //    hs["DropOutDate"] = "退学时间";

        //    hs.GetValue<string,string>()
            
        //}
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
                ErrorCode code=ErrorCode.sys_success;
                //找到实体
                entities.Student.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    if (Cache_Get_PayOrderList().Where(y => y.IsConfirm == YesOrNoCode.No && y.StudentID.Equals(x.ID)).Any())
                    {
                        code = ErrorCode.cant_delete_unconfirm_payorder__had;                     
                    }
                    else
                    {
                        x.Flag = x.Flag | (long)GlobalFlag.Removed;
                        Add_Log(LogCode.DeleteStudent, x.ID, string.Format("{0}在{1}删除了学员{2}", Client.LoginUser.Name, DateTime.Now.ToString(), x.Name), "", "");
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
        public WebResult<bool> WantDrop_Student(string id,string remark,decimal money)
        {
            if (!id.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_StudentList();
                //找到实体
                var student=entities.Student.Find(id);

                if(student==null)
                    return Result(false, ErrorCode.sys_param_format_error);

                student.State = StudentCode.WantDropOut;
                student.Remark = remark;
                student.UpdatedTime = DateTime.Now;
                student.HadPayMoney -= money;
                entities.PayOrder.Add(new PayOrder()
                {
                    StudentID = id,
                    PayMoney = student.HadPayMoney,
                    CreaterID = Client.LoginUser.ID,
                    IsConfirm = YesOrNoCode.No,
                    ID = Guid.NewGuid().ToString("N"),
                    WantDropMoney = money,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now,
                    IsDrop = YesOrNoCode.Yes,
                    WantDropDate = DateTime.Now,
                    Flag = (long)GlobalFlag.Normal,
                    UpdaterID = Client.LoginUser.ID,
                    PayTime = DateTime.Now
                    
                });
                if (entities.SaveChanges() > 0)
                {
                    var index = list.FindIndex(y => y.ID.Equals(id));
                    if (index > -1)
                    {
                        list[index] = student;
                    }
                    else
                    {
                        list.Add(student);
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
                return Cache_Get_StudentList().FirstOrDefault(x => x.ID.Equals(id));
            }
        }
    }
}
