
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
        public WebResult<PageList<Waste>> Get_WastePageList(int pageIndex, int pageSize, WasteCode code, string oilId,string carId,string coachId)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_WasteList().AsQueryable().AsNoTracking();
                if (code != WasteCode.None)
                {
                    query = query.Where(x => x.Code.Equals(code));
                }
                if (oilId.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.OilID.Equals(oilId));
                }
                if (carId.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.CarID.Equals(carId));
                }
                if (coachId.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.CoachID.Equals(coachId));
                }
                var count = query.Count();
                var list = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var userDic = Cache_Get_UserDic();
                var coachDic = Cache_Get_CoachList_Dic();
                var thingDic = Cache_Get_DataDictionary()[GroupCode.Thing];

                list.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.CoachID) && coachDic.ContainsKey(x.CoachID))
                        x.CoachName = coachDic[x.CoachID].Name;
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
                model.ID = Guid.NewGuid().ToString("N");
                model.CreatedTime=model.UpdatedTime = DateTime.Now;
                model.Flag = (long)GlobalFlag.Normal;
                entities.Waste.Add(model);
                var oilModel = entities.OilCard.Find(model.OilID);
                if(oilModel==null)
                    return Result(false, ErrorCode.sys_param_format_error);
                oilModel.Money += model.Money;
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_WasteList();
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
                return Cache_Get_WasteList().AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
        }
              
    }
}
