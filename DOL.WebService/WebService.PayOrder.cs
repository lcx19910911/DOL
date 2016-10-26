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
        string payOrderKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "PayOrder");

        /// <summary>
        /// 全局缓存
        /// </summary>
        /// <returns></returns>
        private List<PayOrder> Cache_Get_PayOrderList()
        {

            return CacheHelper.Get<List<PayOrder>>(payOrderKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<PayOrder> list = db.PayOrder.OrderByDescending(x => x.CreatedTime).ThenBy(x => x.ID).ToList();
                    return list;
                }
            });
        }

        /// <summary>
        /// 全局缓存 dic
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, PayOrder> Cache_Get_PayOrderList_Dic()
        {
            return Cache_Get_PayOrderList().ToDictionary(x => x.ID);
        }


        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PayOrder Find_PayOrder(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            return Cache_Get_PayOrderList().AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
        }



        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_PayOrder(PayOrder model)
        {
            using (DbRepository entities = new DbRepository())
            {
                model.ID = Guid.NewGuid().ToString("N");
                model.UpdatedTime = DateTime.Now;
                model.CreatedTime = DateTime.Now;
                model.Flag = (long)GlobalFlag.Normal;
                entities.PayOrder.Add(model);
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_PayOrderList();
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
        public WebResult<bool> Update_PayOrder(PayOrder model)
        {
            using (DbRepository entities = new DbRepository())
            {
                var oldEntity = entities.PayOrder.Find(model.ID);
                if (oldEntity != null)
                {
                    oldEntity.PayMoney = model.PayMoney;
                    oldEntity.PayTime = model.PayTime;
                    oldEntity.PayTypeID = model.PayTypeID;
                    oldEntity.VoucherNO = model.VoucherNO; ;
                    oldEntity.VoucherThum = model.VoucherThum;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_PayOrderList();
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
        public WebResult<bool> Delete_PayOrder(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_PayOrderList();
                //找到实体
                entities.PayOrder.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    x.Flag = x.Flag | (long)GlobalFlag.Removed;
                    var index = list.FindIndex(y => y.ID.Equals(x.ID));
                    if (index > -1)
                    {
                        list[index] = x;
                    }
                    else
                    {
                        list.Add(list[index]);
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
        /// 获取学员缴费记录
        /// </summary>
        /// <param name="">门店id</param>
        /// <returns></returns>
        public WebResult<PageList<PayOrder>> Get_PayOrderByStudentId(int pageIndex,
            int pageSize, string studentId)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_PayOrderList().AsQueryable().AsNoTracking().Where(x => x.StudentID.Equals(studentId) && x.Flag == (long)GlobalFlag.Normal).OrderBy(x => x.CreatedTime);
                var count = query.Count();
                var list=query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var accountList = Get_DataDictorySelectItem(GroupCode.Account);
                var payTypeList = Get_DataDictorySelectItem(GroupCode.PayType);
                list.ForEach(x => {
                    if (!string.IsNullOrEmpty(x.AccountID))
                        x.AccountName = accountList.FirstOrDefault(y => y.Value.Equals(x.AccountID))?.Text;
                    if (!string.IsNullOrEmpty(x.PayTypeID))
                        x.PayTypeName = payTypeList.FirstOrDefault(y => y.Value.Equals(x.PayTypeID))?.Text;
                });

                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }
    }
}
