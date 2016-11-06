using DOL.Core;
using DOL.Model;
using DOL.Repository;
using System;
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
                    List<Student> list = db.Student.Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0).OrderByDescending(x => x.CreatedTime).ThenBy(x => x.ID).ToList();
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
            DateTime? makedTimeStart, DateTime? makeTimeEnd
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
                if (mobile.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Mobile.Contains(mobile));
                }
                if ((int)state != -1)
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

                if (driverShopID.IsNotNullOrEmpty()&& driverShopID != "-1")
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.DriverShopID) && x.DriverShopID.Equals(driverShopID));
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
                    if (!string.IsNullOrEmpty(x.DriverShopID) && driverShopDic.ContainsKey(x.DriverShopID))
                        x.DriverShopName = driverShopDic[x.DriverShopID]?.Name;

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
                    oldEntity.MakeCardCityCode = model.MakeCardCityCode;
                    oldEntity.MakeCardRemark = model.MakeCardRemark;
                    oldEntity.CertificateID = model.CertificateID;

                    oldEntity.DriverShopID = model.DriverShopID;
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
                //找到实体
                entities.Student.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    x.Flag = x.Flag | (long)GlobalFlag.Removed;
                    Add_Log(LogCode.DeleteStudent, x.ID, string.Format("{0}在{1}删除了学员{2}", Client.LoginUser.Name, DateTime.Now.ToString(), x.Name), "","");
                    var index = list.FindIndex(y => y.ID.Equals(x.ID));
                    if (index > -1)
                    {
                        list[index] = x;
                    }
                    else
                    {
                        list.Add(x);
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
                    IsDrop=YesOrNoCode.Yes,
                    WantDropDate= DateTime.Now,
                    Flag =(long)GlobalFlag.Normal
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
