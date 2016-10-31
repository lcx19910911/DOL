
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
                var list = query.OrderByDescending(x => x.EnteredDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
                    if (Cache_Get_StudentList().AsQueryable().AsNoTracking().Where(x => x.Mobile.Equals(model.Mobile) && !x.ID.Equals(model.ID)).Any())
                        return Result(false, ErrorCode.datadatabase_mobile__had);
                    oldEntity.IDCard = model.IDCard;
                    oldEntity.Mobile = model.Mobile;
                    oldEntity.Name = model.Name;
                    oldEntity.GenderCode = model.GenderCode;
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
    }
}
