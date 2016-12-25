
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
        string wasteKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "Waste");



        /// <summary>
        /// 缓存
        /// </summary>
        /// <returns></returns>
        private List<Waste> Cache_Get_WasteList()
        {

            return CacheHelper.Get<List<Waste>>(wasteKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<Waste> list = db.Waste.ToList();
                    return list;
                }
            });
        }


        /// <summary>
        /// 缓存 dic
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Waste> Cache_Get_WasteList_Dic()
        {
            return Cache_Get_WasteList().ToDictionary(x => x.ID);
        }



        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<Waste>> Get_WastePageList(int pageIndex, int pageSize, WasteCode code, string oilId, string carId, string userId, string license)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = entities.Waste.AsQueryable().AsNoTracking();
                if (code != WasteCode.None)
                {
                    query = query.Where(x => x.Code.Equals(code));
                }
                if (license.IsNotNullOrEmpty())
                {
                    var carIdList = Cache_Get_CarList().AsQueryable().Where(x => x.License.Contains(license)).Select(x => x.ID).ToList();
                    query = query.Where(x => carIdList.Contains(x.CarID));
                }
                //不是管理员只能看到自己的
                //if (!Client.LoginUser.IsAdmin)
                //{
                //    query = query.Where(x => x.CreatedUserID.Equals(Client.LoginUser.ID));
                //}
                if (oilId.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.OilID.Equals(oilId));
                }
                if (carId.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.CarID.Equals(carId));
                }
                if (userId.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.CreatedUserID.Equals(userId));
                }
                var count = query.Count();
                var list = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var userDic = Cache_Get_UserDic();
                var carDic = Cache_Get_CarList_Dic();
                var thingDic = Cache_Get_DataDictionary()[GroupCode.Thing];
                var refuelingDic = Cache_Get_DataDictionary()[GroupCode.RefuelingPoint];
                var repairDic = Cache_Get_DataDictionary()[GroupCode.RepairingPoint];
                var oilDic = Cache_Get_OilCardList_Dic();
                list.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.CreatedUserID) && userDic.ContainsKey(x.CreatedUserID))
                        x.CreatedUserName = userDic[x.CreatedUserID].Name;
                    if (!string.IsNullOrEmpty(x.CarID) && carDic.ContainsKey(x.CarID))
                        x.License = carDic[x.CarID].License;
                    if (x.Code == WasteCode.Oil)
                    {
                        if (!string.IsNullOrEmpty(x.TargetID) && refuelingDic.ContainsKey(x.TargetID))
                            x.TargetName = refuelingDic[x.TargetID].Value;
                        if (!string.IsNullOrEmpty(x.OilID) && oilDic.ContainsKey(x.OilID))
                            x.OilName = oilDic[x.OilID].CardNO;
                    }
                    else if (x.Code == WasteCode.Repair)
                    {
                        if (!string.IsNullOrEmpty(x.TargetID) && repairDic.ContainsKey(x.TargetID))
                            x.TargetName = repairDic[x.TargetID].Value;
                    }
                    if (!string.IsNullOrEmpty(x.ThingID) && thingDic.ContainsKey(x.ThingID))
                        x.ThingName = thingDic[x.ThingID].Value;
                });


                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }


        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_Waste(Waste model)
        {
            using (DbRepository entities = new DbRepository())
            {
                var oilModel = new OilCard();
                model.ID = Guid.NewGuid().ToString("N");
                model.CreatedTime = model.UpdatedTime = DateTime.Now;
                model.CreatedUserID = model.UpdaterID = Client.LoginUser.ID;
                model.Flag = (long)GlobalFlag.Normal;
                entities.Waste.Add(model);
                if (model.Code == WasteCode.Oil)
                {
                    oilModel = entities.OilCard.Find(model.OilID);
                    if (oilModel == null)
                        return Result(false, ErrorCode.sys_param_format_error);
                    oilModel.Balance -= model.Money;
                }
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_WasteList();
                    if (model.Code == WasteCode.Oil)
                    {
                        var oilCardList = Cache_Get_OilCardList();
                        var index = oilCardList.FindIndex(x => x.ID.Equals(model.OilID));
                        if (index > -1)
                        {
                            oilCardList[index] = oilModel;
                        }
                        else
                        {
                            oilCardList.Add(oilModel);
                        }
                    }

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
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Waste Find_Waste(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            using (DbRepository entities = new DbRepository())
            {
                return entities.Waste.AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
            }
        }



        /// <summary>
        /// 获取教练员学员考试信息 和工资
        /// </summary>
        /// <param name="time"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public WebResult<ReportModel> Get_WasteReport(string coachId, string carId, DateTime? searchTime)
        {
            //赋值本月
            if (searchTime == null)
                searchTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM"));
            //表单model
            var model = new ReportModel();
            model.x = new List<string>();
            model.oilSeries = new List<Series>();
            model.repairSeries = new List<Series>();

            //本月结束时间
            var endTime = DateTime.Parse(searchTime.Value.AddMonths(1).ToString("yyyy-MM")).AddDays(-1);// && x.Code == ThemeCode.Two

            var query = Cache_Get_CarList().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0).AsQueryable();
            if (coachId.IsNullOrEmpty() || coachId.Equals("-1") || coachId.Equals("null"))
            {
                coachId = string.Empty;
            }
            if (coachId.IsNotNullOrEmpty())
            {
                query = query.Where(x => x.CoachID.Equals(coachId));
            }
            if (carId.IsNullOrEmpty() || carId.Equals("-1") || carId.Equals("null"))
            {
                carId = string.Empty;
            }
            if (carId.IsNotNullOrEmpty())
            {
                query = query.Where(x => x.ID.Equals(carId));
            }

            var list = query.ToList();
            var cardIdList = list.Select(x => x.ID).ToList();
            var wasteList = Cache_Get_WasteList().Where(x => cardIdList.Contains(x.CarID) && x.CreatedTime > searchTime && x.CreatedTime < endTime).ToList();

            //如果没选择教练和车辆和时间 显示教练的平均车损油耗
            if (coachId.IsNullOrEmpty() && carId.IsNullOrEmpty())
            {

                var oilSeries = new Series();
                var repairSeries = new Series();
                oilSeries.data = new List<decimal>();
                repairSeries.data = new List<decimal>();
                oilSeries.name = "平均油耗";
                repairSeries.name = "平均车损";

                Cache_Get_CoachList().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0).ToList().ForEach(item =>
                {
                    model.x.Add(item.Name);
                    var carIdList = list.Where(x => x.CoachID.Equals(item.ID)).Select(x => x.ID).ToList();

                    var repairWasteMoney = wasteList.Where(x => carIdList.Contains(x.CarID) && x.Code == WasteCode.Repair).Sum(x => x.Money);
                    var oilWasteMoney = wasteList.Where(x => carIdList.Contains(x.CarID) && x.Code == WasteCode.Oil).Sum(x => x.Money);


                    repairSeries.data.Add(repairWasteMoney);
                    oilSeries.data.Add(oilWasteMoney);

                });

                model.oilSeries.Add(oilSeries);
                model.repairSeries.Add(repairSeries);
            }
            //显示某个教练的车损油耗
            else if (coachId.IsNotNullOrEmpty() && carId.IsNullOrEmpty())
            {
                var selectCarList = list.Where(x => x.CoachID.Equals(coachId)).ToList();
                if (selectCarList != null && selectCarList.Count > 0)
                {
                    for (var i = searchTime.Value.Date.Day; i <= endTime.Date.Day; i++)
                    {
                        model.x.Add(i.ToString());
                    }
                    selectCarList.ForEach(item =>
                    {

                        var repairWasteDic = wasteList.Where(x => x.CarID.Equals(item.ID) && x.Code == WasteCode.Repair).GroupBy(x => x.CreatedTime.Date.Day).ToDictionary(x => x.Key, x => x.ToList());
                        var oilWasteDic = wasteList.Where(x => x.CarID.Equals(item.ID) && x.Code == WasteCode.Oil).GroupBy(x => x.CreatedTime.Date.Day).ToDictionary(x => x.Key, x => x.ToList());

                        var oilSeries = new Series();
                        var repairSeries = new Series();
                        oilSeries.data = new List<decimal>();
                        repairSeries.data = new List<decimal>();
                        oilSeries.name = repairSeries.name = item.License;
                        for (var i = searchTime.Value.Date.Day; i <= endTime.Date.Day; i++)
                        {
                            if (repairWasteDic.ContainsKey(i))
                            {
                                repairSeries.data.Add(repairWasteDic[i].Sum(x => x.Money));
                            }
                            else
                            {
                                repairSeries.data.Add(0);
                            }
                            if (oilWasteDic.ContainsKey(i))
                            {
                                oilSeries.data.Add(oilWasteDic[i].Sum(x => x.Money));
                            }
                            else
                            {
                                oilSeries.data.Add(0);
                            }
                        }
                        model.oilSeries.Add(oilSeries);
                        model.repairSeries.Add(repairSeries);
                    });
                }
            }
            //显示车辆车损油耗
            else if (carId.IsNotNullOrEmpty())
            {
                var selectCar = list.FirstOrDefault(x => x.ID.Equals(carId));
                if (selectCar != null)
                {
                    var repairWasteDic = wasteList.Where(x => x.CarID.Equals(carId) && x.Code == WasteCode.Repair).GroupBy(x => x.CreatedTime.Date.Day).ToDictionary(x => x.Key, x => x.ToList());
                    var oilWasteDic = wasteList.Where(x => x.CarID.Equals(carId) && x.Code == WasteCode.Oil).GroupBy(x => x.CreatedTime.Date.Day).ToDictionary(x => x.Key, x => x.ToList());

                    var oilSeries = new Series();
                    var repairSeries = new Series();
                    oilSeries.data = new List<decimal>();
                    repairSeries.data = new List<decimal>();

                    oilSeries.name = repairSeries.name = selectCar.License;
                    for (var i = searchTime.Value.Date.Day; i <= endTime.Date.Day; i++)
                    {
                        model.x.Add(i.ToString());
                        if (repairWasteDic.ContainsKey(i))
                        {
                            repairSeries.data.Add(repairWasteDic[i].Sum(x => x.Money));
                        }
                        else
                        {
                            repairSeries.data.Add(0);
                        }
                        if (oilWasteDic.ContainsKey(i))
                        {
                            oilSeries.data.Add(oilWasteDic[i].Sum(x => x.Money));
                        }
                        else
                        {
                            oilSeries.data.Add(0);
                        }
                    }
                    model.oilSeries.Add(oilSeries);
                    model.repairSeries.Add(repairSeries);
                }
            }
            return Result(model);
        }

    }
}
