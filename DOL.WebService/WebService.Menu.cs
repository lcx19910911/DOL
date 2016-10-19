
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
        string menuKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "Menu");

        /// <summary>
        /// 站内通知全局缓存
        /// </summary>
        /// <returns></returns>
        private List<Menu> Cache_Get_MenuList()
        {

            return CacheHelper.Get<List<Menu>>(menuKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<Menu> list = db.Menu.OrderByDescending(x => x.Sort).ThenBy(x => x.ID).ToList();
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
        public WebResult<PageList<Menu>> Get_MenuPageList(int pageIndex, int pageSize, string name, string no)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_MenuList().AsQueryable().AsNoTracking();
                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }

                var count = query.Count();
                var list = query.OrderByDescending(x => x.Sort).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_Menu(Menu model)
        {
            using (DbRepository entities = new DbRepository())
            {
                var limitFlags = entities.Menu.Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0).Select(x => x.LimitFlag ?? 0).ToList();
                var limitFlagAll = 0L;
                // 获取所有角色位值并集
                limitFlags.ForEach(x => limitFlagAll |= x);
                var limitFlag = 0L;
                // 从低位遍历是否为空
                for (var i = 0; i < 64; i++)
                {
                    if ((limitFlagAll & (1 << i)) == 0)
                    {
                        limitFlag = 1 << i;
                        break;
                    }
                }
                model.LimitFlag = limitFlag;
                model.ID = Guid.NewGuid().ToString("N");
                model.CreatedTime = DateTime.Now;
                model.Flag = (long)GlobalFlag.Normal;
                model.UpdatedTime = DateTime.Now;
                entities.Menu.Add(model);
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_MenuList();
                    list.Add(model);
                    CacheHelper.Insert<List<Menu>>(menuKey, list);
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
        public WebResult<bool> Update_Menu(Menu model)
        {
            using (DbRepository entities = new DbRepository())
            {
                var oldEntity = entities.Menu.Find(model.ID);
                if (oldEntity != null)
                {
                    oldEntity.ParentID = model.ParentID;
                    oldEntity.Name = model.Name;
                    oldEntity.ClassName = model.ClassName;
                    oldEntity.Link = model.Link;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_MenuList();
                    var cachItem = list.FirstOrDefault(x => x.ID.Equals(model.ID));
                    if (cachItem != null)
                    {
                        cachItem = oldEntity;
                        CacheHelper.Insert<List<Menu>>(menuKey, list);
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
        /// 获取ZTree子节点
        /// </summary>
        /// <param name="parentId">父级id</param>
        /// <param name="groups">分组数据</param>
        /// <returns></returns>
        public List<ZTreeNode> GetZTreeChildren(string parentId, List<IGrouping<string, Menu>> groups)
        {
            List<ZTreeNode> ztreeNodes = new List<ZTreeNode>();
            var group = groups.FirstOrDefault(x => x.Key == parentId);
            if (group != null)
            {
                ztreeNodes = group.Select(
                    x => new ZTreeNode()
                    {
                        name = x.Name,
                        value = x.ID,
                        children = GetZTreeChildren(x.ID, groups)
                    }).ToList();
            }
            return ztreeNodes;
        }

        /// <summary>
        /// 获取ZTree子节点
        /// </summary>
        /// <param name="parentId">父级id</param>
        /// <param name="groups">分组数据</param>
        /// <returns></returns>
        public List<ZTreeNode> GetZTreeFlagChildren(string parentId, List<IGrouping<string, Menu>> groups)
        {
            List<ZTreeNode> ztreeNodes = new List<ZTreeNode>();
            var group = groups.FirstOrDefault(x => x.Key == parentId);
            if (group != null)
            {
                ztreeNodes = group.Select(
                    x => new ZTreeNode()
                    {
                        name = x.Name,
                        value = x.LimitFlag.ToString(),
                        children = GetZTreeFlagChildren(x.ID, groups)
                    }).ToList();
            }
            return ztreeNodes;
        }
    }
}
