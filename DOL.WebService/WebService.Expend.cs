
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
        string ExpendKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "Expend");



        /// <summary>
        /// 缓存
        /// </summary>
        /// <returns></returns>
        private List<Expend> Cache_Get_ExpendList()
        {

            return CacheHelper.Get<List<Expend>>(ExpendKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<Expend> list = db.Expend.ToList();
                    return list;
                }
            });
        }


        /// <summary>
        /// 缓存 dic
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Expend> Cache_Get_ExpendList_Dic()
        {
            return Cache_Get_ExpendList().ToDictionary(x => x.ID);
        }



        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<Expend>> Get_ExpendPageList(int pageIndex, int pageSize, string thingId, string no,
            DateTime? createTimeStart, DateTime? createTimeEnd)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = entities.Expend.AsQueryable().AsNoTracking();

                if (thingId.IsNotNullOrEmpty()&&thingId!="-1")
                {
                    query = query.Where(x => x.ThingID.Equals(thingId));
                }
                if (no.IsNotNullOrEmpty() && no != "-1")
                {
                    query = query.Where(x => x.NO.Contains(no));
                }
                if (createTimeStart != null)
                {
                    query = query.Where(x => x.AddDate >= createTimeStart);
                }
                if (createTimeEnd != null)
                {
                    createTimeEnd = createTimeEnd.Value.AddDays(1);
                    query = query.Where(x => x.AddDate < createTimeEnd);
                }

                var count = query.Count();
                var list = query.OrderByDescending(x => x.NO).ThenBy(x=>x.AddDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var thingDic = new Dictionary<string, DataDictionary>();
                if (Cache_Get_DataDictionary().ContainsKey(GroupCode.ExpendThing))
                {
                    thingDic = Cache_Get_DataDictionary()[GroupCode.ExpendThing];
                }
                list.ForEach(x =>
                {
                    
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
        public WebResult<bool> Add_BatachExpend(List<Expend> model)
        {
            using (DbRepository entities = new DbRepository())
            {
                if (model == null || model.Count == 0)
                {
                    return Result(false, ErrorCode.sys_param_format_error);
                }
                
                var wasteList = new List<Expend>();
                model.ForEach(x =>
                {
                    x.ID = Guid.NewGuid().ToString("N");
                    x.UpdatedTime = x.CreatedTime = DateTime.Now;
                    x.UpdaterID = Client.LoginUser.ID;
                    x.Flag = (long)GlobalFlag.Normal;
                    wasteList.Add(x);
                    entities.Expend.Add(x);
                });
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_ExpendList();
                    list.AddRange(wasteList);
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
        public WebResult<bool> Delete_Expend(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_ExpendList();

                var oilCardList = Cache_Get_OilCardList();
                //找到实体
                entities.Expend.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    entities.Expend.Remove(x);
                    var index = list.FindIndex(y => y.ID.Equals(x.ID));
                    if (index > -1)
                    {
                        list.RemoveAt(index);
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
        /// 获取维修点参数
        /// </summary>
        /// <returns></returns>
        public Tuple<List<SelectItem>, List<SelectItem>> GetExpendSelectItem()
        {
            var carList = Cache_Get_ExpendList().Where(x => x.Flag == 0).Select(x => x.NO).Distinct().Select(x=>new SelectItem() {
                Text=x,
                Value=x
            }).ToList();
            var thingList = Get_DataDictorySelectItem(GroupCode.ExpendThing);
            return new Tuple<List<SelectItem>, List<SelectItem>>(carList, thingList);
        }

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Expend Find_Expend(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            using (DbRepository entities = new DbRepository())
            {
                return entities.Expend.AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update_Expend(Expend model)
        {
            using (DbRepository entities = new DbRepository())
            {

                var oldEntity = entities.Expend.Find(model.ID);
                if (oldEntity != null)
                {
                    oldEntity.ThingID = model.ThingID;
                    oldEntity.Money = model.Money;
                    oldEntity.AddDate = model.AddDate;
                    oldEntity.Remark = model.Remark;
                    oldEntity.NO = model.NO ;
                    oldEntity.UpdatedTime = DateTime.Now;
                    oldEntity.UpdaterID = Client.LoginUser.ID;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_ExpendList();
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
        /// 获取教练员学员考试信息 和工资
        /// </summary>
        /// <param name="time"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public WebResult<ReportModel> Get_ExpendReport(DateTime? searchTime)
        {
            //赋值本月
            if (searchTime == null)
                searchTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM"));
            //表单model
            var model = new ReportModel();
            model.x = new List<string>();
            model.expendSeries = new List<Series>();

            //本月结束时间
            var endTime = DateTime.Parse(searchTime.Value.AddMonths(1).ToString("yyyy-MM")).AddDays(-1);// && x.Code == ThemeCode.Two

            var query = Cache_Get_CarList().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0).AsQueryable();


            var expendThingList = Get_DataDictorySelectItem(GroupCode.ExpendThing);
            var list = query.ToList();
            var expendDic = Cache_Get_ExpendList().Where(x => x.CreatedTime > searchTime && x.CreatedTime < endTime && (x.Flag & (long)GlobalFlag.Removed) == 0).GroupBy(x=>x.ThingID).ToDictionary(x=>x.Key,x=>x.Sum(y=>y.Money));
            var expendSeries = new Series();
            expendSeries.data = new List<decimal>();
            expendSeries.name = "支出统计";
            Get_DataDictorySelectItem(GroupCode.ExpendThing).ForEach(item =>
            {
                model.x.Add(item.Text);
                if (expendDic.ContainsKey(item.Value))
                {
                    expendSeries.data.Add(expendDic[item.Value]);
                }
            });
            model.expendSeries.Add(expendSeries);
            return Result(model);
        }
    }
}
