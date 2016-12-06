
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
        public WebResult<PageList<Recharge>> Get_RechargePageList(string id,int pageIndex, int pageSize)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_RechargeList().AsQueryable().AsNoTracking().Where(x=>x.OilID.Equals(id));
                var count = query.Count();
                var list = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var userDic = Cache_Get_UserDic();
                list.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.CreatedUserID) && userDic.ContainsKey(x.CreatedUserID))
                        x.CreatedUserName = userDic[x.CreatedUserID].Name;
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
                model.CreatedTime = DateTime.Now;
                model.CreatedUserID = Client.LoginUser.ID;
                entities.Recharge.Add(model);
                var oilModel = entities.OilCard.Find(model.OilID);
                if(oilModel==null)
                    return Result(false, ErrorCode.sys_param_format_error);
                oilModel.Money += model.Money;
                if (entities.SaveChanges() > 0)
                {
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
