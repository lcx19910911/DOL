
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
        /// 缓存
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
        /// 缓存 dic
        /// </summary>
        /// <returns></returns>
        private Dictionary<string,Menu> Cache_Get_MenuList_Dic()
        {
            return Cache_Get_MenuList().ToDictionary(x => x.ID);
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
                list.ForEach(x =>
                {
                    if(!string.IsNullOrEmpty(x.ParentID))
                        x.ParentName = Cache_Get_MenuList_Dic()[x.ParentID]?.Name;
                });
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
                model.UpdaterID = Client.LoginUser.ID;
                entities.Menu.Add(model);
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_MenuList();
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
                    oldEntity.UpdaterID = Client.LoginUser.ID;
                    oldEntity.UpdatedTime = DateTime.Now;
                    oldEntity.Sort = model.Sort;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_MenuList();
                    var index = list.FindIndex(x => x.ID.Equals(model.ID));
                    if (index>-1)
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
        public Menu Find_Menu(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
             return Cache_Get_MenuList().AsQueryable().AsNoTracking().FirstOrDefault(x=>x.ID.Equals(id));
        }


        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Delete_Menu(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_MenuList();
                //找到实体
                entities.Menu.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    entities.Menu.Remove(x);
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
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        public List<DOL.Model.MenuItem> Get_UserMenu(string parentId)
        {
            List<DOL.Model.MenuItem> menuList = new List<DOL.Model.MenuItem>();
            var group = Cache_Get_MenuList().AsQueryable().AsNoTracking().OrderByDescending(x => x.Sort).GroupBy(x => x.ParentID).ToList();
            return Get_UserMenu(parentId, group);
        }


        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        private List<DOL.Model.MenuItem> Get_UserMenu(string parentId, List<IGrouping<string, Menu>> groups)
        {
            List<DOL.Model.MenuItem> menuList = new List<DOL.Model.MenuItem>();
            var group = groups.FirstOrDefault(x => x.Key == parentId);
            if (group != null)
            {
                menuList = group.OrderBy(x=>x.Sort).Select(
                    x =>
                {
                    if (this.Client.LoginUser.MenuFlag == -1 || (this.Client.LoginUser.MenuFlag & x.LimitFlag) != 1)
                    {
                        return new DOL.Model.MenuItem()
                        {
                            ClassName = x.ClassName,
                            Name = x.Name,
                            Link = x.Link,
                            Children = Get_UserMenu(x.ID, groups)
                        };
                    }
                    else
                        return null;
                }).ToList();
            }
            return menuList;
        }



        /// <summary>
        /// 获取ZTree子节点
        /// </summary>
        /// <param name="parentId">父级id</param>
        /// <param name="groups">分组数据</param>
        /// <returns></returns>
        private List<ZTreeNode> Get_MenuZTreeChildren(string parentId, List<IGrouping<string, Menu>> groups)
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
                        children = Get_MenuZTreeChildren(x.ID, groups)
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
        public List<ZTreeNode> Get_MenuZTreeChildren(string parentId)
        {
            List<ZTreeNode> ztreeNodes = new List<ZTreeNode>();
            var group = Cache_Get_MenuList().AsQueryable().AsNoTracking().OrderByDescending(x => x.Sort).GroupBy(x=>x.ParentID).ToList();
            return Get_MenuZTreeChildren(parentId, group);
        }

        /// <summary>
        /// 获取ZTree子节点
        /// </summary>
        /// <param name="parentId">父级id</param>
        /// <param name="groups">分组数据</param>
        /// <returns></returns>
        public List<ZTreeNode> Get_MenuZTreeFlagChildren(string parentId)
        {
            List<ZTreeNode> ztreeNodes = new List<ZTreeNode>();
            var group = Cache_Get_MenuList().AsQueryable().AsNoTracking().OrderByDescending(x => x.Sort).GroupBy(x => x.ParentID).ToList();
            return Get_MenuZTreeFlagChildren(parentId, group);
        }

        /// <summary>
        /// 获取ZTree子节点
        /// </summary>
        /// <param name="parentId">父级id</param>
        /// <param name="groups">分组数据</param>
        /// <returns></returns>
        private List<ZTreeNode> Get_MenuZTreeFlagChildren(string parentId, List<IGrouping<string, Menu>> groups)
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
                        children = Get_MenuZTreeFlagChildren(x.ID, groups)
                    }).ToList();
            }
            return ztreeNodes;
        }
    }
}
