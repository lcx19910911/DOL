
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
        string rechargeKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "Recharge");



        /// <summary>
        /// 缓存
        /// </summary>
        /// <returns></returns>
        private List<Recharge> Cache_Get_RechargeList()
        {

            return CacheHelper.Get<List<Recharge>>(rechargeKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<Recharge> list = db.Recharge.ToList();
                    return list;
                }
            });
        }


        /// <summary>
        /// 缓存 dic
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Recharge> Cache_Get_RechargeList_Dic()
        {
            return Cache_Get_RechargeList().ToDictionary(x => x.ID);
        }

        

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<Recharge>> Get_RechargePageList(int pageIndex, int pageSize, string oilId, string userId)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_RechargeList().AsQueryable().AsNoTracking();
                if (oilId.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.OilID.Equals(oilId));
                }
                if (userId.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.CreatedUserID.Equals(userId));
                }
                var count = query.Count();
                var list = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var userDic = Cache_Get_UserDic();
                var oilDic = Cache_Get_OilCardList_Dic();
                list.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.CreatedUserID) && userDic.ContainsKey(x.CreatedUserID))
                        x.CreatedUserName = userDic[x.CreatedUserID].Name;
                    if (!string.IsNullOrEmpty(x.OilID) && oilDic.ContainsKey(x.OilID))
                        x.OilName = oilDic[x.OilID].CardNO;
                    
                });

                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }


        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_Recharge(Recharge model)
        {
            using (DbRepository entities = new DbRepository())
            {
                model.ID = Guid.NewGuid().ToString("N");
                model.CreatedUserID = Client.LoginUser.ID;
                entities.Recharge.Add(model);
                var oilModel = entities.OilCard.Find(model.OilID);
                if(oilModel==null)
                    return Result(false, ErrorCode.sys_param_format_error);
                oilModel.Money += model.Money;
                oilModel.Balance += model.Money;
                if (entities.SaveChanges() > 0)
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
                    var list = Cache_Get_RechargeList();
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
        /// 删除分类
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Delete_Recharge(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_RechargeList();

                var oilCardList = Cache_Get_OilCardList();
                //找到实体
                entities.Recharge.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    entities.Recharge.Remove(x);

                    var oildCard = entities.OilCard.Find(x.OilID);
                    if (oildCard != null)
                    {
                        oildCard.Money -= x.Money;
                        oildCard.Balance -= x.Money;
                        var oilCardIndex = oilCardList.FindIndex(y => y.ID.Equals(x.OilID));
                        if (oilCardIndex > -1)
                        {
                            oilCardList[oilCardIndex] = oildCard;
                        }
                        else
                        {
                            oilCardList.Add(oildCard);
                        }
                    }
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
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Recharge Find_Recharge(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
                return Cache_Get_RechargeList().AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
        }
              
    }
}
