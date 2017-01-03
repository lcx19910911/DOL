
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
        public List<Coach> Cache_Get_CoachList()
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
                var query = Cache_Get_CoachList().AsQueryable().AsNoTracking().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0);
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
                string menuIdStr = string.Empty;
                if (Cache_Get_RoleList_Dic().ContainsKey(Params.CoachRoleId))
                {
                    menuIdStr = Cache_Get_RoleList_Dic()[Params.CoachRoleId].MenuIDStr;
                }
                var userModel = new Model.User()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now,
                    Flag = (long)GlobalFlag.Normal,
                    Name = model.Name,
                    Mobile = model.Mobile,
                    Account = model.Mobile,
                    Password = CryptoHelper.MD5_Encrypt("123456"),
                    CreaterId = Client.LoginUser.ID,
                    DepartmentID = "1",
                    RoleID = Params.CoachRoleId,
                    MenuIDStr = menuIdStr,
                    UpdaterID = Client.LoginUser.ID,
                    CoachID = model.ID
                };

                entities.User.Add(userModel);
                if (entities.SaveChanges() > 0)
                {
                    Cache_Get_UserList().Add(userModel);
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
            YesOrNoCode? code,
            bool isTwo
            )
        {
            using (DbRepository entities = new DbRepository())
            {
                if (string.IsNullOrEmpty(Client.LoginUser.CoachID))
                    return null;

                var query = Cache_Get_StudentList().AsQueryable().AsNoTracking().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0);
                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }
                if (no.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.IDCard.Contains(no));
                }
                if (isTwo)
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.ThemeTwoCoachID) && x.ThemeTwoCoachID.Equals(Client.LoginUser.CoachID));
                    if (code != null && (int)code != -1)
                    {
                        query = query.Where(x => x.ThemeTwoPass.Equals(code));
                    }
                }
                else
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.ThemeThreeCoachID) && x.ThemeThreeCoachID.Equals(Client.LoginUser.CoachID));
                    if (code != null && (int)code != -1)
                    {
                        query = query.Where(x => x.ThemeTwoPass.Equals(code));
                    }
                }
                // var newList = new List<Student>();
                //var list = query.ToList();
                //list.ForEach(x =>
                //{
                //    if (x.ThemeTwoCoachID.IsNotNullOrEmpty() && x.ThemeTwoCoachID.Equals(Client.LoginUser.CoachID))
                //    {
                //        newList.Add(x);
                //    }
                //    if (x.ThemeThreeCoachID.IsNotNullOrEmpty() && x.ThemeThreeCoachID.Equals(Client.LoginUser.CoachID))
                //    {
                //        newList.Add(x);
                //    }
                //});

                var count = query.Count();
                var list = query.OrderByDescending(x => x.EnteredDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).Distinct().ToList();
                var referenceDic = Cache_Get_ReferenceList_Dic();
                var driverShopDic = Cache_Get_DriverShopList_Dic();
                var areaDic = Cache_Get_DataDictionary()[GroupCode.Area];
                var enteredPointDic = Cache_Get_EnteredPoint_Dic();
                var cerDic = Cache_Get_DataDictionary()[GroupCode.Certificate];
                var payMethodDic = Cache_Get_DataDictionary()[GroupCode.PayMethod];
                var trianDic = Cache_Get_DataDictionary()[GroupCode.Train];
                var coachDic = Cache_Get_CoachList_Dic();
                var studentIdList = list.Select(x => x.ID).ToList();
                var examDic = Cache_Get_ExamList().Where(x => studentIdList.Contains(x.StudentID)).GroupBy(x => x.StudentID).ToDictionary(x => x.Key);
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
                    //证书
                    if (!string.IsNullOrEmpty(x.CertificateID) && cerDic.ContainsKey(x.CertificateID))
                        x.CertificateName = cerDic[x.CertificateID]?.Value;

                    //培训方式
                    if (!string.IsNullOrEmpty(x.TrianID) && trianDic.ContainsKey(x.TrianID))
                        x.TrianName = trianDic[x.TrianID]?.Value;
                    if (examDic.ContainsKey(x.ID))
                        x.ExamCount = examDic[x.ID].Where(y => y.Code == x.NowTheme).Count() + 1;
                    else
                        x.ExamCount = 1;

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
                    if (Cache_Get_UserList().AsQueryable().AsNoTracking().Where(x => x.Mobile.Equals(model.Mobile) && !string.IsNullOrEmpty(x.CoachID) && !x.CoachID.Equals(model.ID)).Any())
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

                    if (entities.SaveChanges() > 0)
                    {
                        var list = Cache_Get_CoachList();
                        var index = list.FindIndex(x => x.ID.Equals(model.ID));
                        var userList = Cache_Get_UserList();
                        var userIndex = userList.FindIndex(x => x.ID.Equals(user.ID));
                        if (index > -1)
                        {
                            list[index] = oldEntity;
                            userList[userIndex] = user;
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
                else
                    return Result(false, ErrorCode.sys_param_format_error);

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

                var query = Cache_Get_CoachList().Where(x => x.Flag == 0).AsQueryable().AsNoTracking();

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
                    query.Where(x => x.DriverShopID.Equals(driverShopId)).OrderBy(x => x.CreatedTime).ToList().ForEach(x =>
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
        /// 教练员确认
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Confirm_Coach(string id, int code)
        {
            if (!id.IsNotNullOrEmpty() || (code != 2 && code != 3))
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_StudentList();

                var student = entities.Student.Find(id);
                if (student == null)
                    return Result(false, ErrorCode.sys_param_format_error);
                if (code == 2)
                    student.ThemeTwoConfirm = YesOrNoCode.Yes;
                if (code == 3)
                    student.ThemeThreeConfirm = YesOrNoCode.Yes;
                if (entities.SaveChanges() > 0)
                {
                    var index = list.FindIndex(x => x.ID.Equals(id));
                    list[index] = student;
                    return Result(true);
                }
                else
                {
                    return Result(false, ErrorCode.sys_fail);
                }
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
                    if (user != null)
                        user.Flag = x.Flag | (long)GlobalFlag.Removed;
                    x.Flag = x.Flag | (long)GlobalFlag.Removed;
                    var index = list.FindIndex(y => y.ID.Equals(x.ID));
                    var userIndex = -1;
                    if (user != null)
                        userIndex = userList.FindIndex(y => y.ID.Equals(user.ID));
                    if (index > -1)
                    {
                        list[index] = x;
                        if (userIndex != -1)
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
            if (string.IsNullOrEmpty(id)||id=="-1")
                return Get_CoachSalary(time);
            //赋值本月
            if (time == null)
                time = DateTime.Parse(DateTime.Now.ToString("yyyy-MM"));

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
            model.ThemeSalaryModel.List = new List<Tuple<ThemeCode, int, string, int, decimal, decimal>>();
            model.ThemeSalaryModel.OldList = new List<Tuple<ThemeCode, int, string, int, decimal, decimal>>();
            model.ThemeSalaryModel.BasicSalary = coachItem.BasicSalary;

            //科目二是该教练的学员
            var studentList = Cache_Get_StudentList().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0);


            #region 总的通过率
            //科目二学员集合
            var themeTwoStudentList = studentList.Where(x => !string.IsNullOrEmpty(x.ThemeTwoCoachID) && x.ThemeTwoCoachID.Equals(id)).ToList();
            var themeTwoStudentIdList = themeTwoStudentList.Select(x => x.ID).ToList();
            var allThemeTwoList = Cache_Get_ExamList().Where(x => themeTwoStudentIdList.Contains(x.StudentID)).ToList();

            //考试人数 通过人数 通过率
            model.ExamModel.ThemeTwoAllPeopleExamCount = allThemeTwoList.Where(x => x.Code == ThemeCode.Two).Select(x => x.StudentID).Distinct().Count();
            model.ExamModel.ThemeTwoAllExamCount = allThemeTwoList.Where(x => x.Code == ThemeCode.Two).Select(x => x.StudentID).Count();
            model.ExamModel.ThemeTwoAllPassCount = allThemeTwoList.Where(x => x.Result == ExamCode.Pass && x.Code == ThemeCode.Two).Select(x => x.StudentID).Distinct().Count();
            model.ExamModel.ThemeTwoAllPassScaling = model.ExamModel.ThemeTwoAllPassCount != 0 ? (model.ExamModel.ThemeTwoAllPassCount * 100 / model.ExamModel.ThemeTwoAllPeopleExamCount) : 0;

            //科目三学员集合
            var themeThreeStudentList = studentList.Where(x => !string.IsNullOrEmpty(x.ThemeThreeCoachID) && x.ThemeThreeCoachID.Equals(id)).ToList();
            var themeThreeStudentIdList = themeThreeStudentList.Select(x => x.ID).ToList();
            var allThemeThreeList = Cache_Get_ExamList().Where(x => themeThreeStudentIdList.Contains(x.StudentID)).ToList();

            //考试人数 通过人数 通过率
            model.ExamModel.ThemeThreeAllPeopleExamCount = allThemeThreeList.Where(x => x.Code == ThemeCode.Three).Select(x => x.StudentID).Distinct().Count();
            model.ExamModel.ThemeThreeAllExamCount = allThemeThreeList.Where(x => x.Code == ThemeCode.Three).Select(x => x.StudentID).Count();
            model.ExamModel.ThemeThreeAllPassCount = allThemeThreeList.Where(x => x.Result == ExamCode.Pass && x.Code == ThemeCode.Three).Select(x => x.StudentID).Distinct().Count();
            model.ExamModel.ThemeThreeAllPassScaling = model.ExamModel.ThemeThreeAllPassCount != 0 ? (model.ExamModel.ThemeThreeAllPassCount * 100 / model.ExamModel.ThemeThreeAllPeopleExamCount) : 0;

            #endregion

            //考试学员姓名集合
            List<string> studentNameList = new List<string>();
            List<string> studentNameArry = new List<string>();

            #region 科目二考试记录

            //考试记录集合
            var examThemeTwolist = Cache_Get_ExamList().Where(x => themeTwoStudentIdList.Contains(x.StudentID) && x.CreatedTime >= time && x.CreatedTime <= endTime).ToList();

            //考试次数 通过次数
            int examAllCount = 0;
            int passAllCount = 0;

            //科目二考试人数
            examThemeTwolist.Where(x => x.Code == ThemeCode.Two).OrderBy(x => x.CreatedTime).GroupBy(x => x.CreatedTime.Date).ToList().ForEach(x =>
              {
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
                      var passIdList = x.Where(y => y.Result == ExamCode.Pass).Select(y => y.StudentID).ToList();
                      var failedList = new List<string>();
                      //学员姓名
                      themeTwoStudentList.Where(y => idList.Contains(y.ID)).ToList().ForEach(y =>
                      {
                          if (passIdList.Contains(y.ID))
                          {
                              studentNameList.Add(y.Name);
                          }
                          else
                          {
                              failedList.Add(y.Name);
                          }
                          studentNameArry.Add(y.Name);
                      });
                      if (studentNameList != null)
                      {
                          var nameStr =string.Format("<p class=\"am-text-success\">{0}</p><p class=\"am-text-danger\">{1}</p>", string.Join(",", studentNameList), string.Join(",", failedList));

                          //添加到集合
                          model.ExamModel.List.Add(new Tuple<ThemeCode, DateTime, string, int, int, int>(ThemeCode.Two, x.Key, nameStr, examCount, passCount, passCount != 0 ? (passCount * 100 / examCount) : 0));
                      }
                  }
              });

            if (studentNameArry != null)
            {
                model.ExamModel.ThemeTwoMonthPeopleExamCount = studentNameArry.Distinct().Count();
            }
            //得出科目二的考试信息
            model.ExamModel.ThemeTwoMonthExamCount = examAllCount;
            model.ExamModel.ThemeTwoMonthPassCount = passAllCount;
            if (examAllCount == 0)
            {
                model.ExamModel.ThemeTwoMonthPassScaling = 0;
            }
            else
            {
                model.ExamModel.ThemeTwoMonthPassScaling = passAllCount != 0 ? (passAllCount * 100 / examAllCount) : 0;
            }
            if (model.ExamModel.ThemeTwoMonthPeopleExamCount == 0)
            {
                model.ExamModel.ThemeTwoMonthPeoplePassScaling = 0;
            }
            else
            {
                model.ExamModel.ThemeTwoMonthPeoplePassScaling = passAllCount != 0 ? (passAllCount * 100 / model.ExamModel.ThemeTwoMonthPeopleExamCount) : 0;
            }


            #endregion

            examAllCount = 0;
            passAllCount = 0;
            studentNameArry = new List<string>();

            #region 科目三考试记录

            //考试记录集合
            var examThemeThreelist = Cache_Get_ExamList().Where(x => themeThreeStudentIdList.Contains(x.StudentID) && x.CreatedTime >= time && x.CreatedTime <= endTime).ToList();
            //科目三考试人数
            examThemeThreelist.Where(x => x.Code == ThemeCode.Three).OrderBy(x => x.CreatedTime).GroupBy(x => x.CreatedTime.Date).ToList().ForEach(x =>
            {
                if (x != null)
                {
                    studentNameList = new List<string>();
                    int examCount = x.Count();
                    examAllCount += examCount;
                    int passCount = x.Where(y => y.Result == ExamCode.Pass).Count();
                    passAllCount += passCount;
                    var idList = x.Select(y => y.StudentID).ToList();
                    var passIdList = x.Where(y => y.Result == ExamCode.Pass).Select(y => y.StudentID).ToList();
                    var failedList = new List<string>();
                    themeThreeStudentList.Where(y => idList.Contains(y.ID)).ToList().ForEach(y =>
                    {
                        if (passIdList.Contains(y.ID))
                        {
                            studentNameList.Add(y.Name);
                        }
                        else
                        {
                            failedList.Add(y.Name);
                        }
                        studentNameArry.Add(y.Name);
                    });
                    if (studentNameList != null)
                    {
                        var nameStr = string.Format("<p class=\"am-text-success\">{0}</p><p class=\"am-text-danger\">{1}</p>", string.Join(",", studentNameList), string.Join(",", failedList));
                        model.ExamModel.List.Add(new Tuple<ThemeCode, DateTime, string, int, int, int>(ThemeCode.Three, x.Key, nameStr, examCount, passCount, passCount != 0 ? (passCount * 100 / examCount) : 0));
                    }
                }
            });
            if (studentNameArry != null)
            {
                model.ExamModel.ThemeThreeMonthPeopleExamCount = studentNameArry.Distinct().Count();
            }
            //科目三的考试信息
            model.ExamModel.ThemeThreeMonthExamCount = examAllCount;
            model.ExamModel.ThemeThreeMonthPassCount = passAllCount;
            if (examAllCount == 0)
            {
                model.ExamModel.ThemeThreeMonthPassScaling = 0;
            }
            else
            {
                model.ExamModel.ThemeThreeMonthPassScaling = passAllCount != 0 ? (passAllCount * 100 / examAllCount) : 0;
            }
            if (model.ExamModel.ThemeThreeMonthPeopleExamCount == 0)
            {
                model.ExamModel.ThemeThreeMonthPeoplePassScaling = 0;
            }
            else
            {
                model.ExamModel.ThemeThreeMonthPeoplePassScaling = passAllCount != 0 ? (passAllCount * 100 / model.ExamModel.ThemeThreeMonthPeopleExamCount) : 0;
            }


            #endregion


            decimal AllMoney = 0;

            //搜索时间内的科目薪资
            var themeSalaryList = Cache_Get_ThemeSalaryList().Where(x => !x.EndTime.HasValue).ToList();
            var themeSalaryOldList = Cache_Get_ThemeSalaryList().Where(x => (x.EndTime.HasValue && x.EndTime.Value > time && x.EndTime.Value < endTime)).ToList();

            #region 科目二薪资
            //科目二薪资集合
            var themeTwoSalaryDic = themeSalaryList.Where(x => x.Code == ThemeCode.Two).ToDictionary(x => x.Count);
            var themeSalaryOldDic = themeSalaryOldList.Where(x => x.Code == ThemeCode.Two).ToDictionary(x => x.Count);

            //有的次数集合
            List<int> hadCount = new List<int>();



            //该教练有的科目二薪资
            examThemeTwolist.Where(x => x.Result == ExamCode.Pass && x.Code == ThemeCode.Two).GroupBy(x => x.Count).ToList().ForEach(x =>
                  {
                      var count = 0;
                      decimal money = 0;
                      decimal totalMoney = 0;

                      var oldName = "";
                      var oldCount = 0;
                      decimal oldMoney = 0;
                      decimal oldTotalMoney = 0;
                      x.ToList().ForEach(y =>
                      {
                          if (themeSalaryOldDic.ContainsKey(x.Key))
                          {
                              var item = themeSalaryOldDic[x.Key];
                              if (y.CreatedTime < item.EndTime)
                              {
                                  oldName = item.Name + "（" + item.EndTime.Value.ToString("yyyy年MM月dd日") + "后弃用)";
                                  hadCount.Add(x.Key);
                                  oldMoney = themeSalaryOldDic[x.Key].Money;
                                  oldCount += 1;
                                  oldTotalMoney += oldMoney;

                              }
                              else if (themeTwoSalaryDic.ContainsKey(x.Key))
                              {
                                  hadCount.Add(x.Key);
                                  money = themeTwoSalaryDic[x.Key].Money;
                                  count += 1;
                                  totalMoney += money;

                              }
                          }
                          else if (themeTwoSalaryDic.ContainsKey(x.Key))
                          {
                              hadCount.Add(x.Key);
                              money = themeTwoSalaryDic[x.Key].Money;
                              count += 1;
                              totalMoney += money;

                          }
                      });


                      AllMoney += totalMoney;
                      AllMoney += oldTotalMoney;
                      if (oldTotalMoney > 0)
                      {
                          model.ThemeSalaryModel.OldList.Add(new Tuple<ThemeCode, int, string, int, decimal, decimal>(ThemeCode.Two, x.Key, oldName, oldCount, oldMoney, oldTotalMoney));

                          model.ThemeSalaryModel.List.Add(new Tuple<ThemeCode, int, string, int, decimal, decimal>(ThemeCode.Two, x.Key, themeTwoSalaryDic[x.Key].Name, count, money == 0 ? themeTwoSalaryDic[x.Key].Money : money, totalMoney));
                      }
                      else if (totalMoney > 0)
                      {
                          model.ThemeSalaryModel.List.Add(new Tuple<ThemeCode, int, string, int, decimal, decimal>(ThemeCode.Two, x.Key, themeTwoSalaryDic[x.Key].Name, count, money == 0 ? themeTwoSalaryDic[x.Key].Money : money, totalMoney));
                      }
                  });


            //没有的科目薪资
            themeSalaryList.Where(x => !hadCount.Contains(x.Count) && x.Code == ThemeCode.Two).ToList().ForEach(x =>
                {
                    model.ThemeSalaryModel.List.Add(new Tuple<ThemeCode, int, string, int, decimal, decimal>(ThemeCode.Two, x.Count, x.Name, 0, x.Money, 0));
                });
            #endregion

            #region 科目三薪资

            //科目三的人员薪资
            var themeThreeSalaryDic = themeSalaryList.Where(x => x.Code == ThemeCode.Three).ToDictionary(x => x.Count);
            var themeThreeSalaryOldDic = themeSalaryOldList.Where(x => x.Code == ThemeCode.Three).ToDictionary(x => x.Count);

            hadCount = new List<int>();
            examThemeThreelist.Where(x => x.Result == ExamCode.Pass && x.Code == ThemeCode.Three).GroupBy(x => x.Count).ToList().ForEach(x =>
            {


                var count = 0;
                decimal money = 0;
                decimal totalMoney = 0;

                var oldName = "";
                var oldCount = 0;
                decimal oldMoney = 0;
                decimal oldTotalMoney = 0;
                x.ToList().ForEach(y =>
                {
                    if (themeThreeSalaryOldDic.ContainsKey(x.Key))
                    {
                        var item = themeThreeSalaryOldDic[x.Key];
                        if (y.CreatedTime < item.EndTime)
                        {
                            oldName = item.Name + "（" + item.EndTime.Value.ToString("yyyy年MM月dd日") + "后弃用)";
                            hadCount.Add(x.Key);
                            oldMoney = item.Money;
                            oldCount += 1;
                            oldTotalMoney += oldMoney;
                        }
                        else if (themeThreeSalaryDic.ContainsKey(x.Key))
                        {
                            hadCount.Add(x.Key);
                            money = themeThreeSalaryDic[x.Key].Money;
                            count += 1;
                            totalMoney += money;

                        }
                    }
                    else if (themeThreeSalaryDic.ContainsKey(x.Key))
                    {
                        hadCount.Add(x.Key);
                        money = themeThreeSalaryDic[x.Key].Money;
                        count += 1;
                        totalMoney += money;

                    }
                });

                AllMoney += oldTotalMoney;
                AllMoney += totalMoney;
                if (oldTotalMoney > 0)
                {
                    model.ThemeSalaryModel.OldList.Add(new Tuple<ThemeCode, int, string, int, decimal, decimal>(ThemeCode.Three, x.Key, oldName, oldCount, oldMoney, oldTotalMoney));
                    model.ThemeSalaryModel.List.Add(new Tuple<ThemeCode, int, string, int, decimal, decimal>(ThemeCode.Three, x.Key, themeThreeSalaryDic[x.Key].Name, count, money == 0 ? themeThreeSalaryDic[x.Key].Money : money, totalMoney));
                }
                else if(totalMoney > 0)
                {
                    model.ThemeSalaryModel.List.Add(new Tuple<ThemeCode, int, string, int, decimal, decimal>(ThemeCode.Three, x.Key, themeThreeSalaryDic[x.Key].Name, count, money == 0 ? themeThreeSalaryDic[x.Key].Money : money, totalMoney));
                }
            });
            //没有人员的薪资
            themeSalaryList.Where(x => !hadCount.Contains(x.Count) && x.Code == ThemeCode.Three).ToList().ForEach(x =>
            {
                model.ThemeSalaryModel.List.Add(new Tuple<ThemeCode, int, string, int, decimal, decimal>(ThemeCode.Three, x.Count, x.Name, 0, x.Money, 0));
            });

            #endregion
            //总工资
            model.ThemeSalaryModel.TotalMoeny = AllMoney + coachItem.BasicSalary;

            return model;
        }


        /// <summary>
        /// 获取教练员学员考试信息 和工资
        /// </summary>
        /// <param name="time"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public CoachReportModel Get_CoachSalary(DateTime? time)
        {
            //赋值本月
            if (time == null)
                time = DateTime.Parse(DateTime.Now.ToString("yyyy-MM"));
            //本月结束时间
            var endTime = DateTime.Parse(time.Value.AddMonths(1).ToString("yyyy-MM")).AddDays(-1);// && x.Code == ThemeCode.Two
            var coachList = Cache_Get_CoachList().Where(x => x.Flag == 0).ToList();

            var returnModel = new CoachReportModel();
            returnModel.isShowList = true;
            CoachListReportModel model = new CoachListReportModel();
            model.StartTime= returnModel.StartTime = time.Value;
            model.EndTime = endTime;


            var studentList = Cache_Get_StudentList().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0);
            var themeTwoStudentList = studentList.Where(x => !string.IsNullOrEmpty(x.ThemeTwoCoachID)).ToList();
            var themeThreeStudentList = studentList.Where(x => !string.IsNullOrEmpty(x.ThemeThreeCoachID)).ToList();
            var examList = Cache_Get_ExamList().ToList();

            //搜索时间内的科目薪资
            var themeSalaryList = Cache_Get_ThemeSalaryList().Where(x => !x.EndTime.HasValue).OrderBy(x=>x.Code).ThenBy(x=>x.Count).ToList();
            var themeSalaryOldList = Cache_Get_ThemeSalaryList().Where(x => (x.EndTime.HasValue && x.EndTime.Value > time && x.EndTime.Value < endTime)).OrderBy(x => x.Code).ThenBy(x => x.Count).ToList();

            model.SalaryList = themeSalaryList;
            model.OldSalaryList = themeSalaryOldList;

            //科目二薪资集合
            var themeTwoSalaryDic = themeSalaryList.Where(x => x.Code == ThemeCode.Two).ToDictionary(x => x.Count);
            var themeSalaryOldDic = themeSalaryOldList.Where(x => x.Code == ThemeCode.Two).ToDictionary(x => x.Count);

            //科目三的人员薪资
            var themeThreeSalaryDic = themeSalaryList.Where(x => x.Code == ThemeCode.Three).ToDictionary(x => x.Count);
            var themeThreeSalaryOldDic = themeSalaryOldList.Where(x => x.Code == ThemeCode.Three).ToDictionary(x => x.Count);

            List<int> hadCount = new List<int>();
            model.AllDic = new Dictionary<string, ThemeSalaryItemModel>();
            model.AllTrainDic = new Dictionary<string, ThemeTrainItemModel>();
            foreach (var coachItem in coachList)
            {

                var itemModel = new ThemeSalaryItemModel();
                itemModel.CoachName = coachItem.Name;
                itemModel.List = new List<Tuple<ThemeCode, int, string, int, decimal, decimal>>();
                itemModel.OldList = new List<Tuple<ThemeCode, int, string, int, decimal, decimal>>();
                itemModel.BasicSalary = coachItem.BasicSalary;


                //科目二学员集合
                var selectThemeTwoStudentList = themeTwoStudentList.Where(x => x.ThemeTwoCoachID.Equals(coachItem.ID)).ToList();
                var selectThemeTwoStudentIDList = selectThemeTwoStudentList.Select(x => x.ID).ToList();

                //科目三学员集合
                var selectThemeThreeStudentList = themeThreeStudentList.Where(x => x.ThemeThreeCoachID.Equals(coachItem.ID)).ToList();
                var selectThemeThreeStudentIDList = selectThemeThreeStudentList.Select(x => x.ID).ToList();

                //考试记录集合  科目二 科目三
                var examThemeTwolist = examList.Where(x => selectThemeTwoStudentIDList.Contains(x.StudentID)&&x.Code==ThemeCode.Two && x.CreatedTime >= time && x.CreatedTime <= endTime).ToList();
                var examThemeThreelist = examList.Where(x => selectThemeThreeStudentIDList.Contains(x.StudentID) && x.Code == ThemeCode.Three && x.CreatedTime >= time && x.CreatedTime <= endTime).ToList();

                //科目二薪资
                //有的次数集合
                hadCount = new List<int>();
                decimal AllMoney = 0;
                decimal themeMoney = 0;
                //该教练有的科目二薪资
                examThemeTwolist.Where(x => x.Result == ExamCode.Pass).GroupBy(x => x.Count).ToList().ForEach(x =>
                  {
                      var count = 0;
                      decimal money = 0;
                      decimal totalMoney = 0;

                      var oldName = "";
                      var oldCount = 0;
                      decimal oldMoney = 0;
                      decimal oldTotalMoney = 0;
                      x.ToList().ForEach(y =>
                      {
                          if (themeSalaryOldDic.ContainsKey(x.Key))
                          {
                              var item = themeSalaryOldDic[x.Key];
                              if (y.CreatedTime < item.EndTime)
                              {
                                  oldName = item.Name + "（" + item.EndTime.Value.ToString("yyyy年MM月dd日") + "后弃用)";
                                  hadCount.Add(x.Key);
                                  oldMoney = themeSalaryOldDic[x.Key].Money;
                                  oldCount += 1;
                                  oldTotalMoney += oldMoney;

                              }
                              else if (themeTwoSalaryDic.ContainsKey(x.Key))
                              {
                                  hadCount.Add(x.Key);
                                  money = themeTwoSalaryDic[x.Key].Money;
                                  count += 1;
                                  totalMoney += money;

                              }
                          }
                          else if (themeTwoSalaryDic.ContainsKey(x.Key))
                          {
                              hadCount.Add(x.Key);
                              money = themeTwoSalaryDic[x.Key].Money;
                              count += 1;
                              totalMoney += money;

                          }
                      });


                      AllMoney += totalMoney;
                      AllMoney += oldTotalMoney;
                      themeMoney += totalMoney;
                      themeMoney += oldTotalMoney;
                      if (oldTotalMoney > 0)
                      {
                          itemModel.OldList.Add(new Tuple<ThemeCode, int, string, int, decimal, decimal>(ThemeCode.Two, x.Key, oldName, oldCount, oldMoney, oldTotalMoney));

                          itemModel.List.Add(new Tuple<ThemeCode, int, string, int, decimal, decimal>(ThemeCode.Two, x.Key, themeTwoSalaryDic[x.Key].Name, count, money == 0 ? themeTwoSalaryDic[x.Key].Money : money, totalMoney));
                      }
                      if (totalMoney > 0)
                      {
                          itemModel.List.Add(new Tuple<ThemeCode, int, string, int, decimal, decimal>(ThemeCode.Two, x.Key, themeTwoSalaryDic[x.Key].Name, count, money == 0 ? themeTwoSalaryDic[x.Key].Money : money, totalMoney));
                      }
                  });
                itemModel.ThemeTwoMoney = themeMoney;
                themeMoney = 0;
                //没有的科目二薪资
                themeSalaryList.Where(x => !hadCount.Contains(x.Count) && x.Code == ThemeCode.Two).ToList().ForEach(x =>
                {
                        itemModel.List.Add(new Tuple<ThemeCode, int, string, int, decimal, decimal>(ThemeCode.Two, x.Count, x.Name, 0, x.Money, 0));
                });
                //科目三薪资
                hadCount = new List<int>();
                examThemeThreelist.Where(x => x.Result == ExamCode.Pass).GroupBy(x => x.Count).ToList().ForEach(x =>
                {


                    var count = 0;
                    decimal money = 0;
                    decimal totalMoney = 0;

                    var oldName = "";
                    var oldCount = 0;
                    decimal oldMoney = 0;
                    decimal oldTotalMoney = 0;
                    x.ToList().ForEach(y =>
                    {
                        if (themeThreeSalaryOldDic.ContainsKey(x.Key))
                        {
                            var item = themeThreeSalaryOldDic[x.Key];
                            if (y.CreatedTime < item.EndTime)
                            {
                                oldName = item.Name + "（" + item.EndTime.Value.ToString("yyyy年MM月dd日") + "后弃用)";
                                hadCount.Add(x.Key);
                                oldMoney = item.Money;
                                oldCount += 1;
                                oldTotalMoney += oldMoney;
                            }
                            else if (themeThreeSalaryDic.ContainsKey(x.Key))
                            {
                                hadCount.Add(x.Key);
                                money = themeThreeSalaryDic[x.Key].Money;
                                count += 1;
                                totalMoney += money;

                            }
                        }
                        else if (themeThreeSalaryDic.ContainsKey(x.Key))
                        {
                            hadCount.Add(x.Key);
                            money = themeThreeSalaryDic[x.Key].Money;
                            count += 1;
                            totalMoney += money;

                        }
                    });

                    AllMoney += oldTotalMoney;
                    AllMoney += totalMoney;
                    themeMoney += totalMoney;
                    themeMoney += oldTotalMoney;
                    if (oldTotalMoney > 0)
                    {
                        itemModel.OldList.Add(new Tuple<ThemeCode, int, string, int, decimal, decimal>(ThemeCode.Three, x.Key, oldName, oldCount, oldMoney, oldTotalMoney));
                        itemModel.List.Add(new Tuple<ThemeCode, int, string, int, decimal, decimal>(ThemeCode.Three, x.Key, themeThreeSalaryDic[x.Key].Name, count, money == 0 ? themeThreeSalaryDic[x.Key].Money : money, totalMoney));
                    }
                    if (totalMoney > 0)
                    {
                        itemModel.List.Add(new Tuple<ThemeCode, int, string, int, decimal, decimal>(ThemeCode.Three, x.Key, themeThreeSalaryDic[x.Key].Name, count, money == 0 ? themeThreeSalaryDic[x.Key].Money : money, totalMoney));
                    }
                });
                itemModel.ThemeThreeMoney = themeMoney;
                //没有科目二的薪资
                themeSalaryList.Where(x => !hadCount.Contains(x.Count) && x.Code == ThemeCode.Three).ToList().ForEach(x =>
                {
                    itemModel.List.Add(new Tuple<ThemeCode, int, string, int, decimal, decimal>(ThemeCode.Three, x.Count, x.Name, 0, x.Money, 0));
                });
                itemModel.TotalMoeny = itemModel.ThemeTwoMoney + itemModel.ThemeThreeMoney + coachItem.BasicSalary;
                model.AllDic.Add(coachItem.ID, itemModel);

                #region 考试统计

                var trainItem = new ThemeTrainItemModel();
                trainItem.CoachName = coachItem.Name;
                #region 总的通过率
                //科目二学员集合


                //考试人数 通过人数 通过率
                trainItem.ThemeTwoAllPeopleExamCount = examList.Where(x => selectThemeTwoStudentIDList.Contains(x.StudentID) && x.Code == ThemeCode.Two).Select(x => x.StudentID).Distinct().Count();
                trainItem.ThemeTwoAllExamCount = examList.Where(x => selectThemeTwoStudentIDList.Contains(x.StudentID) && x.Code == ThemeCode.Two).Select(x => x.StudentID).Count();
                trainItem.ThemeTwoAllPassCount = examList.Where(x => selectThemeTwoStudentIDList.Contains(x.StudentID) && x.Code == ThemeCode.Two && x.Result == ExamCode.Pass).Select(x => x.StudentID).Count();
                trainItem.ThemeTwoAllPassScaling = trainItem.ThemeTwoAllPassCount != 0 ? (trainItem.ThemeTwoAllPassCount * 100 / trainItem.ThemeTwoAllPeopleExamCount) : 0;


                //考试人数 通过人数 通过率
                trainItem.ThemeThreeAllPeopleExamCount = examList.Where(x => selectThemeThreeStudentIDList.Contains(x.StudentID) && x.Code == ThemeCode.Three).Select(x => x.StudentID).Distinct().Count();
                trainItem.ThemeThreeAllExamCount = examList.Where(x => selectThemeThreeStudentIDList.Contains(x.StudentID) && x.Code == ThemeCode.Three).Select(x => x.StudentID).Count();
                trainItem.ThemeThreeAllPassCount = examList.Where(x => selectThemeThreeStudentIDList.Contains(x.StudentID) && x.Code == ThemeCode.Three&&x.Result==ExamCode.Pass).Select(x => x.StudentID).Count();
                trainItem.ThemeThreeAllPassScaling = trainItem.ThemeThreeAllPassCount != 0 ? (trainItem.ThemeThreeAllPassCount * 100 / trainItem.ThemeThreeAllPeopleExamCount) : 0;

                #endregion

                //考试学员姓名集合
                List<string> studentNameList = new List<string>();
                List<string> studentNameArry = new List<string>();

                #region 科目二考试记录

                //考试次数 通过次数
                int examAllCount = 0;
                int passAllCount = 0;


                //科目二考试人数
                examThemeTwolist.OrderBy(x => x.CreatedTime).GroupBy(x => x.CreatedTime.Date).ToList().ForEach(x =>
                {
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
                            studentNameArry.Add(y.Name);
                        });

                    }
                });

                if (studentNameArry != null)
                {
                    trainItem.ThemeTwoMonthPeopleExamCount = studentNameArry.Distinct().Count();
                }
                //得出科目二的考试信息
                trainItem.ThemeTwoMonthExamCount = examAllCount;
                trainItem.ThemeTwoMonthPassCount = passAllCount;
                if (examAllCount == 0)
                {
                    trainItem.ThemeTwoMonthPassScaling = 0;
                }
                else
                {
                    trainItem.ThemeTwoMonthPassScaling = passAllCount != 0 ? (passAllCount * 100 / examAllCount) : 0;
                }
                if (trainItem.ThemeTwoMonthPeopleExamCount == 0)
                {
                    trainItem.ThemeTwoMonthPeoplePassScaling = 0;
                }
                else
                {
                    trainItem.ThemeTwoMonthPeoplePassScaling = passAllCount != 0 ? (passAllCount * 100 / trainItem.ThemeTwoMonthPeopleExamCount) : 0;
                }

                #endregion

                examAllCount = 0;
                passAllCount = 0;
                studentNameArry = new List<string>();

                #region 科目三考试记录

                //考试记录集合
                //科目三考试人数
                examThemeThreelist.OrderBy(x => x.CreatedTime).GroupBy(x => x.CreatedTime.Date).ToList().ForEach(x =>
                {
                    if (x != null)
                    {
                        studentNameList = new List<string>();
                        int examCount = x.Count();
                        examAllCount += examCount;
                        int passCount = x.Where(y => y.Result == ExamCode.Pass).Count();
                        passAllCount += passCount;
                        var idList = x.Select(y => y.StudentID).ToList();

                        themeThreeStudentList.Where(y => idList.Contains(y.ID)).ToList().ForEach(y =>
                        {
                            studentNameList.Add(y.Name);
                            studentNameArry.Add(y.Name);
                        });
                    }
                });
                if (studentNameArry != null)
                {
                    trainItem.ThemeThreeMonthPeopleExamCount = studentNameArry.Distinct().Count();
                }
                //科目三的考试信息
                trainItem.ThemeThreeMonthExamCount = examAllCount;
                trainItem.ThemeThreeMonthPassCount = passAllCount;
                if (examAllCount == 0)
                {
                    trainItem.ThemeThreeMonthPassScaling = 0;
                }
                else
                {
                    trainItem.ThemeThreeMonthPassScaling = passAllCount != 0 ? (passAllCount * 100 / examAllCount) : 0;
                }
                if (trainItem.ThemeThreeMonthPeopleExamCount == 0)
                {
                    trainItem.ThemeThreeMonthPeoplePassScaling = 0;
                }
                else
                {
                    trainItem.ThemeThreeMonthPeoplePassScaling = passAllCount != 0 ? (passAllCount * 100 / trainItem.ThemeThreeMonthPeopleExamCount) : 0;
                }
                #endregion
                model.AllTrainDic.Add(coachItem.ID, trainItem);

                #endregion
            }
            returnModel.CoachList = model;
            return returnModel;
        }



        public Coach Get_CoachByName(string name)
        {
            return Cache_Get_CoachList().Where(x => x.Name.Equals(name)).FirstOrDefault();
        }
    }
}
