
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
        string carKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "Car");

        /// <summary>
        /// 缓存
        /// </summary>
        /// <returns></returns>
        private List<Car> Cache_Get_CarList()
        {

            return CacheHelper.Get<List<Car>>(carKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<Car> list = db.Car.ToList();
                    return list;
                }
            });
        }

        /// <summary>
        /// 缓存 dic
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Car> Cache_Get_CarList_Dic()
        {
            return Cache_Get_CarList().ToDictionary(x => x.ID);
        }


        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<Car>> Get_CarPageList(int pageIndex, int pageSize, string brandName,string model,string modelCode, string engineNumber, string license, string coachId)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_CarList().AsQueryable().AsNoTracking().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0);
                if (brandName.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Brand.Contains(brandName));
                }
                if (model.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Models.Contains(model));
                }
                if (modelCode.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.ModelsCode.Contains(modelCode));
                }
                if (engineNumber.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.EngineNumber.Contains(engineNumber));
                }
                if (license.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.License.Contains(license));
                }
                if (coachId.IsNotNullOrEmpty()&&!coachId.Equals("-1"))
                {
                    query = query.Where(x => x.CoachID.Equals(coachId));
                }
                var coachDic = Cache_Get_CoachList_Dic();
                var departMentDic = Cache_Get_DepartmentList_Dic();
                var count = query.Count();
                var list = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var carIdList = list.Select(x => x.ID).ToList();
                var starTime = DateTime.Now.Date.AddDays(-DateTime.Now.Date.Day+1);
                var wasteList = Cache_Get_WasteList().Where(x=> carIdList.Contains(x.CarID)&&x.CreatedTime> starTime).ToList();
                var oilDic = wasteList.Where(x => x.Code == WasteCode.Oil).GroupBy(x => x.CarID).ToDictionary(x=>x.Key,x=>x.ToList());
                var repairDic = wasteList.Where(x => x.Code == WasteCode.Repair).GroupBy(x => x.CarID).ToDictionary(x => x.Key, x => x.ToList());
                list.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.CoachID) && coachDic.ContainsKey(x.CoachID))
                        x.CoachName = coachDic[x.CoachID].Name;
                    if (!string.IsNullOrEmpty(x.DepartmentID) && departMentDic.ContainsKey(x.DepartmentID))
                        x.DepartmentName = departMentDic[x.DepartmentID].Name;
                    if (oilDic.ContainsKey(x.ID))
                        x.OilMonth = oilDic[x.ID].Sum(y=>y.Money);
                    if (repairDic.ContainsKey(x.ID))
                        x.RepairMonth = repairDic[x.ID].Sum(y => y.Money);
                });

                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_Car(Car model)
        {
            using (DbRepository entities = new DbRepository())
            {
                model.ID = Guid.NewGuid().ToString("N");
                model.CreatedTime = DateTime.Now;
                model.Flag = (long)GlobalFlag.Normal;
                model.UpdatedTime = DateTime.Now;
                model.UpdaterID = Client.LoginUser.ID;
                entities.Car.Add(model);
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_CarList();
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
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update_Car(Car model)
        {
            using (DbRepository entities = new DbRepository())
            {
                var oldEntity = entities.Car.Find(model.ID);
                if (oldEntity != null)
                {
                    oldEntity.Models = model.Models;
                    oldEntity.ModelsCode = model.ModelsCode;
                    oldEntity.Brand = model.Brand;
                    oldEntity.EngineNumber = model.EngineNumber;
                    oldEntity.FrameCode = model.FrameCode;
                    oldEntity.CoachID = model.CoachID;
                    oldEntity.DepartmentID = model.DepartmentID;
                    oldEntity.UpdatedTime = DateTime.Now;
                    oldEntity.BuyTime = model.BuyTime;
                    oldEntity.License = model.License;
                    oldEntity.OnCardTime = model.OnCardTime;
                    oldEntity.UpdaterID = Client.LoginUser.ID;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_CarList();
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
        public Car Find_Car(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
                return Cache_Get_CarList().AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Delete_Car(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_CarList();
                //找到实体
                entities.Car.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    x.Flag = x.Flag | (long)GlobalFlag.Removed;
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
        /// 修改责任人
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update_CarCoach(string id, string coachID)
        {
            if (!id.IsNotNullOrEmpty() || !coachID.IsNotNullOrEmpty()
                )
                return Result(false, ErrorCode.sys_param_format_error);
            using (DbRepository entities = new DbRepository())
            {

                var list = Cache_Get_CarList();
                var oldEntity = entities.Car.Find(id);
                if (oldEntity != null)
                {

                    oldEntity.CoachID = coachID;
                    oldEntity.UpdatedTime = DateTime.Now;
                    oldEntity.UpdaterID = Client.LoginUser.ID;
                    string afterJSon = oldEntity.ToJson();
                   // string info = SearchModifyHelper.CompareProperty<Student, Student>(Cache_Get_StudentList_Dic()[oldEntity.ID], oldEntity);
                   // Add_Log(LogCode.UpdateStudent, oldEntity.ID, string.Format("{0}在{1}编辑{2}的信息,分配教练", Client.LoginUser.Name, DateTime.Now.ToString(), oldEntity.Name), beforeJson, afterJSon, info);
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
    }
}
