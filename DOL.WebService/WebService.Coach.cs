﻿
using DOL.Core;
using DOL.Model;
using DOL.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Service
{
    public partial class WebService
    {
        string coachKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "Coach");

        /// <summary>
        /// 缓存
        /// </summary>
        /// <returns></returns>
        private List<Coach> Cache_Get_CoachList()
        {

            return CacheHelper.Get<List<Coach>>(coachKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<Coach> list = db.Coach.OrderByDescending(x => x.CreatedTime).ThenBy(x => x.ID).ToList();
                    return list;
                }
            });
        }

        /// <summary>
        /// 缓存 dic
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Coach> Cache_Get_CoachList_Dic()
        {
            return Cache_Get_CoachList().ToDictionary(x => x.ID);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<Coach>> Get_CoachPageList(int pageIndex, int pageSize, string name, string no)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_CoachList().AsQueryable().AsNoTracking();
                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }
                if (no.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.IDCard.Contains(no));
                }
                var driverShopDic = Cache_Get_DriverShopList_Dic();
                var areaDic = Cache_Get_DataDictionary()[GroupCode.Area];
                var count = query.Count();
                var list = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                list.ForEach(x =>
                {
                    //省
                    if (!string.IsNullOrEmpty(x.ProvinceCode) && areaDic.ContainsKey(x.ProvinceCode))
                        x.ProvinceName = areaDic[x.ProvinceCode]?.Value;
                    //省
                    if (!string.IsNullOrEmpty(x.CityCode) && areaDic.ContainsKey(x.CityCode))
                        x.CityName = areaDic[x.CityCode]?.Value;

                    //证书
                    if (!string.IsNullOrEmpty(x.DriverShopID) && driverShopDic.ContainsKey(x.DriverShopID))
                        x.DriverShopName = driverShopDic[x.DriverShopID]?.Name;
                });
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_Coach(Coach model)
        {
            using (DbRepository entities = new DbRepository())
            {
                if (Cache_Get_UserList().AsQueryable().AsNoTracking().Where(x => x.Mobile.Equals(model.Mobile)).Any())
                    return Result(false, ErrorCode.datadatabase_mobile__had);
                model.ID = Guid.NewGuid().ToString("N");
                model.CreatedTime = DateTime.Now;
                model.Flag = (long)GlobalFlag.Normal;
                model.UpdatedTime = DateTime.Now;
                model.UpdaterID = Client.LoginUser.ID;
                entities.Coach.Add(model);

                entities.User.Add(new Model.User()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now,
                    Flag = (long)GlobalFlag.Normal,
                    Name = model.Name,
                    Mobile = model.Mobile,
                    Account = model.Mobile,
                    Password = CryptoHelper.MD5_Encrypt("123456"),
                    MenuFlag = Params.CoachMenuFlag,
                    CreaterId = Client.LoginUser.ID,
                    DepartmentID = "1",
                    RoleID = "1",
                    UpdaterID = Client.LoginUser.ID,
                    CoachID = model.ID
                });
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_CoachList();
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
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<Student>> Get_MyStudenPageList(
            int pageIndex,
            int pageSize,
            string name,
            string no,
            ThemeTimeCode? themeTwoCode,
            ThemeTimeCode? themeThreeCode
            )
        {
            using (DbRepository entities = new DbRepository())
            {
                if (string.IsNullOrEmpty(Client.LoginUser.CoachID))
                    return null;
                var query = Cache_Get_StudentList().AsQueryable().AsNoTracking().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0);
                query = query.Where(x =>(!string.IsNullOrEmpty(x.ThemeTwoCoachID)&&x.ThemeTwoCoachID.Equals(Client.LoginUser.CoachID)) || (!string.IsNullOrEmpty(x.ThemeThreeCoachID)&&x.ThemeThreeCoachID.Equals(Client.LoginUser.CoachID)));
                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }
                if (no.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.IDCard.Contains(no));
                }

                if (themeTwoCode != null && (int)themeTwoCode != -1)
                {
                    query = query.Where(x => x.ThemeTwoTimeCode.Equals(themeTwoCode));
                }
                if (themeThreeCode != null && (int)themeThreeCode != -1)
                {
                    query = query.Where(x => x.ThemeThreeTimeCode.Equals(themeThreeCode));
                }

                var count = query.Count();
                var list = query.OrderByDescending(x => x.EnteredDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).Distinct().ToList();
                var referenceDic = Cache_Get_ReferenceList_Dic();
                var driverShopDic = Cache_Get_DriverShopList_Dic();
                var areaDic = Cache_Get_DataDictionary()[GroupCode.Area];
                var enteredPointDic = Cache_Get_EnteredPoint_Dic();
                var cerDic = Cache_Get_DataDictionary()[GroupCode.Certificate];
                var payMethodDic = Cache_Get_DataDictionary()[GroupCode.PayMethod];
                var coachDic = Cache_Get_CoachList_Dic();
                list.ForEach(x =>
                {
                    //报名地
                    if (!string.IsNullOrEmpty(x.EnteredCityCode) && areaDic.ContainsKey(x.EnteredCityCode))
                        x.EnteredCityName = areaDic[x.EnteredCityCode]?.Value;
                    //制卡地
                    if (!string.IsNullOrEmpty(x.MakeCardCityCode) && areaDic.ContainsKey(x.MakeCardCityCode))
                        x.MakeCardCityName = areaDic[x.MakeCardCityCode]?.Value;
                    //制卡驾校
                    if (!string.IsNullOrEmpty(x.MakeDriverShopID) && driverShopDic.ContainsKey(x.MakeDriverShopID))
                        x.MakeDriverShopName = driverShopDic[x.MakeDriverShopID]?.Name;

                    //推荐人
                    if (!string.IsNullOrEmpty(x.ReferenceID) && referenceDic.ContainsKey(x.ReferenceID))
                        x.ReferenceName = referenceDic[x.ReferenceID]?.Name;

                    //省
                    if (!string.IsNullOrEmpty(x.ProvinceCode) && areaDic.ContainsKey(x.ProvinceCode))
                        x.ProvinceName = areaDic[x.ProvinceCode]?.Value;
                    //省
                    if (!string.IsNullOrEmpty(x.CityCode) && areaDic.ContainsKey(x.CityCode))
                        x.CityName = areaDic[x.CityCode]?.Value;

                    //科二教练
                    if (!string.IsNullOrEmpty(x.ThemeTwoCoachID) && coachDic.ContainsKey(x.ThemeTwoCoachID))
                        x.ThemeTwoCoachName = coachDic[x.ThemeTwoCoachID]?.Name;
                    //科三教练
                    if (!string.IsNullOrEmpty(x.ThemeThreeCoachID) && coachDic.ContainsKey(x.ThemeThreeCoachID))
                        x.ThemeThreeCoachName = coachDic[x.ThemeThreeCoachID]?.Name;

                });

                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }


        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update_Coach(Coach model)
        {
            using (DbRepository entities = new DbRepository())
            {
                var oldEntity = entities.Coach.Find(model.ID);
                if (oldEntity != null)
                {
                    if (Cache_Get_UserList().AsQueryable().AsNoTracking().Where(x => x.Mobile.Equals(model.Mobile) && !x.ID.Equals(model.ID)).Any())
                        return Result(false, ErrorCode.datadatabase_mobile__had);
                    oldEntity.IDCard = model.IDCard;
                    oldEntity.Mobile = model.Mobile;
                    oldEntity.Name = model.Name;
                    oldEntity.GenderCode = model.GenderCode;
                    oldEntity.BasicSalary = model.BasicSalary;
                    oldEntity.Address = model.Address;
                    oldEntity.ProvinceCode = model.ProvinceCode;
                    oldEntity.CityCode = model.CityCode;
                    oldEntity.Remark = model.Remark;
                    oldEntity.DrivingYears = model.DrivingYears;
                    oldEntity.ArchivesNO = model.ArchivesNO;
                    oldEntity.TrainYears = model.TrainYears;
                    oldEntity.ThemeThreeCount = model.ThemeThreeCount;
                    oldEntity.ThemeTwoCount = model.ThemeTwoCount;
                    oldEntity.FirstGetDriverDate = model.FirstGetDriverDate;
                    oldEntity.FirstGetTrainDate = model.FirstGetTrainDate;
                    oldEntity.DriverShopID = model.DriverShopID;
                    oldEntity.EntryDate = model.EntryDate;
                    oldEntity.UpdaterID = Client.LoginUser.ID;
                    oldEntity.UpdatedTime = DateTime.Now;

                    var user = entities.User.Where(x => x.Mobile.Equals(model.Mobile)).FirstOrDefault();
                    if (user == null)
                        return Result(false, ErrorCode.sys_param_format_error);
                    user.Name = model.Name;
                    user.Mobile = model.Mobile;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_CoachList();
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
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Coach Find_Coach(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            return Cache_Get_CoachList().AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
        }


        /// <summary>
        /// 下拉框集合
        /// </summary>
        /// <param name="">门店id</param>
        /// <returns></returns>
        public List<SelectItem> Get_CoachSelectItem(string driverShopId)
        {
            using (DbRepository entities = new DbRepository())
            {
                List<SelectItem> list = new List<SelectItem>();

                var query = Cache_Get_CoachList().AsQueryable().AsNoTracking();

                if (string.IsNullOrEmpty(driverShopId))
                {
                    query.OrderBy(x => x.CreatedTime).ToList().ForEach(x =>
                    {
                        list.Add(new SelectItem()
                        {
                            Text = x.Name,
                            Value = x.ID
                        });
                    });
                }
                else
                {
                    query.Where(x => x.DriverShopID.Equals(driverShopId) && (x.Flag | (long)GlobalFlag.Normal) == 0).OrderBy(x => x.CreatedTime).ToList().ForEach(x =>
                            {
                                list.Add(new SelectItem()
                                {
                                    Text = x.Name,
                                    Value = x.ID
                                });
                            });
                }
                return list;

            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Delete_Coach(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_CoachList();
                var userList = Cache_Get_UserList();
                //找到实体
                entities.Coach.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    var user = entities.User.Where(y => !string.IsNullOrEmpty(y.CoachID) && y.CoachID.Equals(x.ID)).FirstOrDefault();
                    user.Flag = (long)GlobalFlag.Removed;
                    x.Flag = x.Flag & ~(long)GlobalFlag.Removed;
                    var index = list.FindIndex(y => y.ID.Equals(x.ID));
                    var userIndex = userList.FindIndex(y => y.ID.Equals(user.ID));
                    if (index > -1)
                    {
                        list[index] = x;
                        userList[userIndex] = user;
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
        /// 获取教练员学员考试信息 和工资
        /// </summary>
        /// <param name="time"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public CoachReportModel Get_CoachSalary(DateTime? time, string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            //赋值本月
            if (time == null)
                time =DateTime.Parse(DateTime.Now.ToString("yyyy-MM"));

            //本月结束时间
            var endTime = DateTime.Parse(time.Value.AddMonths(1).ToString("yyyy-MM")).AddDays(-1);// && x.Code == ThemeCode.Two
            var coachItem = Cache_Get_CoachList().Where(x => x.ID.Equals(id)).FirstOrDefault();
            if (coachItem == null)
                return null;

            CoachReportModel model = new CoachReportModel();
            model.StartTime = time.Value;
            model.EndTime = endTime;
            model.CoachName = coachItem.Name;
            model.ExamModel = new ExamModel();
            model.ExamModel.List = new List<Tuple<ThemeCode, DateTime, string, int, int, int>>();

            model.ThemeSalaryModel = new ThemeSalaryModel();
            model.ThemeSalaryModel.List = new List<Tuple<ThemeCode,int, string, int, decimal, decimal>>();
            model.ThemeSalaryModel.BasicSalary = coachItem.BasicSalary;

            //科目二是该教练的学员
            var themeTwoStudentList = Cache_Get_StudentList().Where(x => !string.IsNullOrEmpty(x.ThemeTwoCoachID) && x.ThemeTwoCoachID.Equals(id)).ToList();
            var themeTwoStudentIdList = themeTwoStudentList.Select(x => x.ID).ToList();

            //科目二学员集合
            var allThemeTwoList = Cache_Get_ExamList().Where(x => themeTwoStudentIdList.Contains(x.StudentID)).ToList();

            //考试人数 通过人数 通过率
            model.ExamModel.ThemeTwoAllExamCount = allThemeTwoList.Where(x=>x.Code == ThemeCode.Two).Select(x => x.StudentID).Distinct().Count();
            model.ExamModel.ThemeTwoAllPassCount = allThemeTwoList.Where(x=>x.Result==ExamCode.Pass&& x.Code == ThemeCode.Two).Select(x => x.StudentID).Distinct().Count();
            model.ExamModel.ThemeTwoAllPassScaling = model.ExamModel.ThemeTwoAllPassCount != 0 ? (model.ExamModel.ThemeTwoAllPassCount * 100 / model.ExamModel.ThemeTwoAllExamCount) : 0;

            //科目三学员集合
            var allThemeThreeList = Cache_Get_ExamList().Where(x => themeTwoStudentIdList.Contains(x.StudentID)).ToList();
            //考试人数 通过人数 通过率
            model.ExamModel.ThemeThreeAllExamCount = allThemeThreeList.Where(x => x.Code == ThemeCode.Three).Select(x => x.StudentID).Distinct().Count();
            model.ExamModel.ThemeThreeAllPassCount = allThemeThreeList.Where(x => x.Result == ExamCode.Pass && x.Code == ThemeCode.Three).Select(x => x.StudentID).Distinct().Count();
            model.ExamModel.ThemeThreeAllPassScaling = model.ExamModel.ThemeThreeAllPassCount != 0 ? (model.ExamModel.ThemeThreeAllPassCount * 100 / model.ExamModel.ThemeThreeAllExamCount) : 0;

            //考试学员姓名集合
            List<string> studentNameList = new List<string>();

            //考试记录集合
            var list = Cache_Get_ExamList().Where(x => themeTwoStudentIdList.Contains(x.StudentID) && x.CreatedTime >= time && x.CreatedTime <= endTime).ToList();

            //考试人数 通过人数
            int examAllCount = 0;
            int passAllCount = 0;
            //科目二考试人数
            list.Where(x => x.Code == ThemeCode.Two).GroupBy(x => x.CreatedTime).ToList().ForEach(x => {
                if (x != null)
                {
                    studentNameList = new List<string>();
                    //考试人数
                    int examCount = x.Count();
                    examAllCount += examCount;
                    //通过人数
                    int passCount = x.Where(y => y.Result == ExamCode.Pass).Count();
                    passAllCount += passCount;
                    //考试的学员id集合
                    var idList = x.Select(y => y.StudentID).ToList();
                    //学员姓名
                    themeTwoStudentList.Where(y => idList.Contains(y.ID)).ToList().ForEach(y =>
                    {
                        studentNameList.Add(y.Name);
                    });
                    if (studentNameList != null)
                        //添加到集合
                        model.ExamModel.List.Add(new Tuple<ThemeCode, DateTime, string, int, int, int>(ThemeCode.Two, x.Key, string.Join(",", studentNameList), examCount, passCount, passCount != 0 ? (passCount * 100 / examCount) : 0));
                }
            });

            //得出科目二的考试信息
            model.ExamModel.ThemeTwoMonthExamCount = examAllCount;
            model.ExamModel.ThemeTwoMonthPassCount = passAllCount;
            model.ExamModel.ThemeTwoMonthPassScaling = passAllCount!=0?(passAllCount * 100 / examAllCount):0;


            examAllCount = 0;
            passAllCount = 0;
            //科目三考试人数
            list.Where(x => x.Code == ThemeCode.Three).GroupBy(x => x.CreatedTime).ToList().ForEach(x => {
                if (x != null)
                {
                    studentNameList = new List<string>();
                    int examCount = x.Count();
                    examAllCount += examCount;
                    int passCount = x.Where(y => y.Result == ExamCode.Pass).Count();
                    passAllCount += passCount;
                    var idList = x.Select(y => y.StudentID).ToList();

                    themeTwoStudentList.Where(y => idList.Contains(y.ID)).ToList().ForEach(y =>
                    {
                        studentNameList.Add(y.Name);
                    });
                    if (studentNameList != null)
                        model.ExamModel.List.Add(new Tuple<ThemeCode, DateTime, string, int, int, int>(ThemeCode.Two, x.Key, string.Join(",", studentNameList), examCount, passCount, passCount != 0 ? (passCount * 100 / examCount) : 0));
                }
            });

            //科目三的考试信息
            model.ExamModel.ThemeThreeMonthExamCount = examAllCount;
            model.ExamModel.ThemeThreeMonthPassCount = passAllCount;
            model.ExamModel.ThemeThreeMonthPassScaling = passAllCount != 0 ? (passAllCount * 100 / examAllCount) : 0;


            decimal AllMoney = 0;
            //总科目薪资集合
            var themeSalaryList = Cache_Get_ThemeSalaryList();


            ////科目二薪资集合
            //var themeTwoSalaryList = themeSalaryList.Where(x => x.Code == ThemeCode.Two).ToList();
            //List<int> countList = themeTwoSalaryList.Select(x => x.Count).ToList();
            //var dictinctList = countList.Distinct().ToList();
            //dictinctList.ForEach(x => {
            //    countList.Remove(x);
            //});
            //var themeTwoSalaryDic = themeTwoSalaryList.Where(x => x.EndTime.Value <= x.EndTime).ToDictionary(x => x.ID);


            //科目二薪资集合
            var themeTwoSalaryDic = themeSalaryList.Where(x => x.Code == ThemeCode.Two).ToDictionary(x => x.Count);
            //有的次数集合
            List<int> hadCount = new List<int>();
            //该教练有的科目二薪资
            list.Where(x => x.Result == ExamCode.Pass && x.Code == ThemeCode.Two).GroupBy(x => x.Count).ToList().ForEach(x =>
                  {
                      if (themeTwoSalaryDic.ContainsKey(x.Key))
                      {
                          hadCount.Add(x.Key);
                          var money = themeTwoSalaryDic[x.Key].Money;
                          int count = x.Count();
                          var totalMoney = money * count;
                          AllMoney += totalMoney;
                          model.ThemeSalaryModel.List.Add(new Tuple<ThemeCode,int, string, int, decimal, decimal>(ThemeCode.Two,x.Key, themeTwoSalaryDic[x.Key].Name, count, money, totalMoney));
                      }
                  });

            //没有的科目薪资
            themeSalaryList.Where(x => !hadCount.Contains(x.Count)&&x.Code==ThemeCode.Two).ToList().ForEach(x =>
            {
                model.ThemeSalaryModel.List.Add(new Tuple<ThemeCode, int, string, int, decimal, decimal>(ThemeCode.Two, x.Count, x.Name, 0, x.Money, 0));
            });
            
            //科目三的人员薪资
            var themeThreeSalaryDic = themeSalaryList.Where(x => x.Code == ThemeCode.Three).ToDictionary(x => x.Count);
            hadCount = new List<int>();
            list.Where(x => x.Result == ExamCode.Pass && x.Code == ThemeCode.Three).GroupBy(x => x.Count).ToList().ForEach(x =>
            {
                if (themeThreeSalaryDic.ContainsKey(x.Key))
                {
                    var money = themeThreeSalaryDic[x.Key].Money;
                    int count = x.Count();
                    var totalMoney = money * count;
                    AllMoney += totalMoney;
                    model.ThemeSalaryModel.List.Add(new Tuple<ThemeCode, int, string, int, decimal, decimal>(ThemeCode.Three, x.Key, themeTwoSalaryDic[x.Key].Name, count, money, totalMoney));
                }
            });
            //没有人员的薪资
            themeSalaryList.Where(x => !hadCount.Contains(x.Count) && x.Code == ThemeCode.Three).ToList().ForEach(x =>
            {
                model.ThemeSalaryModel.List.Add(new Tuple<ThemeCode, int, string, int, decimal, decimal>(ThemeCode.Three, x.Count, x.Name, 0, x.Money, 0));
            });

            //总工资
            model.ThemeSalaryModel.TotalMoeny = AllMoney+coachItem.BasicSalary;

            return model;
        }


        public Coach Get_CoachByName(string name)
        {
            return Cache_Get_CoachList().Where(x => x.Name.Equals(name)).FirstOrDefault();
        }
    }
}
