
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
        string roleKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "Role");

        /// <summary>
        /// 站内通知全局缓存
        /// </summary>
        /// <returns></returns>
        private List<Role> Cache_Get_RoleList()
        {

            return CacheHelper.Get<List<Role>>(roleKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<Role> list = db.Role.OrderByDescending(x => x.LimitFlag).ThenBy(x => x.ID).ToList();
                    return list;
                }
            });
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<Role>> Get_RolePageList(int pageIndex, int pageSize, string name, string no)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_RoleList().AsQueryable().AsNoTracking();
                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }

                var count = query.Count();
                var list = query.OrderByDescending(x => x.LimitFlag).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_Role(Role model)
        {
            using (DbRepository entities = new DbRepository())
            {
                var roleFlags = entities.Role.Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0).Select(x => x.RoleFlag ?? 0).ToList();
                var roleFlagAll = 0L;
                // 获取所有角色位值并集
                roleFlags.ForEach(x => roleFlagAll |= x);
                var roleFlag = 0L;
                // 从低位遍历是否为空
                for (var i = 0; i < 64; i++)
                {
                    if ((roleFlagAll & (1 << i)) == 0)
                    {
                        roleFlag = 1 << i;
                        break;
                    }
                }
                model.RoleFlag = roleFlag;
                model.ID = Guid.NewGuid().ToString("N");
                model.CreatedTime = DateTime.Now;
                model.Flag = (long)GlobalFlag.Normal;
                model.UpdatedTime = DateTime.Now;
                entities.Role.Add(model);
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_RoleList();
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
        public WebResult<bool> Update_Role(Role model)
        {
            using (DbRepository entities = new DbRepository())
            {
                var oldEntity = entities.Role.Find(model.ID);
                if (oldEntity != null)
                {
                    oldEntity.Remark = model.Remark;
                    oldEntity.UpdatedTime = DateTime.Now;
                    oldEntity.Name = model.Name;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_RoleList();
                    var index = list.FindIndex(x => x.ID.Equals(model.ID));
                    if (index > -1)
                    {
                        list[index] = oldEntity;
                    }
                    return Result(true);
                }
                else
                {
                    return Result(false, ErrorCode.sys_fail);
                }
            }

        }

   
        ///// <summary>
        ///// 获取选择项
        ///// </summary>
        ///// <param name="roleFlag">角色flag值</param>
        ///// <returns></returns>
        //public List<SelectItem> GetSelectItem(long roleFlag)
        //{
        //    return FlagHelper.GetSelectItem(roleFlag, typeof(Role), "RoleFlag", "Name");
        //}
    }
}
