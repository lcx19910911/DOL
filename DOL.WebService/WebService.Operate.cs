
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
        string operateKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "Operate");

        /// <summary>
        /// 缓存
        /// </summary>
        /// <returns></returns>
        private List<Operate> Cache_Get_OperateList()
        {

            return CacheHelper.Get<List<Operate>>(operateKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<Operate> list = db.Operate.OrderByDescending(x => x.CreatedTime).ThenBy(x => x.ID).ToList();
                    return list;
                }
            });
        }

        /// <summary>
        /// 缓存 dic
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Operate> Cache_Get_OperateList_Dic()
        {
            return Cache_Get_OperateList().ToDictionary(x => x.ID);
        }




        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<Operate>> Get_OperatePageList(int pageIndex, int pageSize, string name, string no)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_OperateList().AsQueryable().AsNoTracking();
                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }

                var count = query.Count();
                var list = query.OrderBy(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                list.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.MenuID))
                        x.MenuName = Cache_Get_MenuList_Dic()[x.MenuID]?.Name;
                });
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }



        /// <summary>
        /// 获取权限地址集合
        /// </summary>
        /// <returns></returns>
        public List<string> Get_OperateUrlByPageUrl(string pageUrl)
        {
            using (DbRepository entities = new DbRepository())
            {
                var menu = Cache_Get_MenuList().FirstOrDefault(x => x.Link.Equals(pageUrl, StringComparison.CurrentCultureIgnoreCase));
                if (menu != null)
                {
                    return Cache_Get_OperateList().AsQueryable().Where(x => x.MenuID.Equals(menu.ID)).Select(x => x.ActionUrl).ToList();
                }
                else
                    return null;
                
            }
        }


        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_Operate(Operate model)
        {
            using (DbRepository entities = new DbRepository())
            {
                var limitFlags = entities.Operate.Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0).Select(x => x.LimitFlag ?? 0).ToList();
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
                entities.Operate.Add(model);
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_OperateList();
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
        public WebResult<bool> Update_Operate(Operate model)
        {
            using (DbRepository entities = new DbRepository())
            {
                var oldEntity = entities.Operate.Find(model.ID);
                if (oldEntity != null)
                {
                    oldEntity.ActionUrl = model.ActionUrl;
                    oldEntity.Name = model.Name;
                    oldEntity.MenuID = model.MenuID;
                    oldEntity.Sort = model.Sort; 
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_OperateList();
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
        public Operate Find_Operate(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            return Cache_Get_OperateList().AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
        }


        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Delete_Operate(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_OperateList();
                //找到实体
                entities.Operate.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    entities.Operate.Remove(x);
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
        /// 获取ZTree子节点
        /// </summary>
        /// <param name="parentId">父级id</param>
        /// <param name="groups">分组数据</param>
        /// <returns></returns>
        private List<ZTreeNode> Get_OperateZTreeChildren(List<IGrouping<string, Operate>> groups)
        {
            List<ZTreeNode> ztreeNodes = new List<ZTreeNode>();
            groups.ForEach(x =>
            {
                ztreeNodes.Add(new ZTreeNode()
                {
                    name = Cache_Get_MenuList().FirstOrDefault(z => z.ID.Equals(x.Key))?.Name,
                    children = GetChidrenOperate(x.Key)
                });
            });
            return ztreeNodes;
        }

        private List<ZTreeNode> GetChidrenOperate(string menuId)
        {
            return Cache_Get_OperateList().Where(x => x.MenuID.Equals(menuId)).Select(x => new ZTreeNode()
            {
                name = x.Name,
                value = x.LimitFlag.ToString(),
            }).ToList();
        }

        /// <summary>
        /// 获取ZTree子节点
        /// </summary>
        /// <param name="parentId">父级id</param>
        /// <param name="groups">分组数据</param>
        /// <returns></returns>
        public List<ZTreeNode> Get_OperateZTreeFlagChildren()
        {
            List<ZTreeNode> ztreeNodes = new List<ZTreeNode>();
            var group = Cache_Get_OperateList().AsQueryable().AsNoTracking().Where(x => (x.Flag | (long)GlobalFlag.Normal) == 0).GroupBy(x => x.MenuID).ToList();
            return Get_OperateZTreeChildren(group);
        }

        /// <summary>
        /// 根据权限Flag值判断是否有权限
        /// </summary>
        /// <param name="oprertorFlag">权限flag值</param>
        /// <param name="url">相对路径</param>
        /// <returns></returns>
        public bool IsHaveAuthority(long oprertorFlag, string url)
        {
            var oprertor = Cache_Get_OperateList().AsQueryable().Where(x => url.Contains(x.ActionUrl)).FirstOrDefault();

            if (oprertor != null)
            {
                if (((long)oprertor.LimitFlag & oprertorFlag) != 0)
                    return true;
                else
                    return false;
            }
            else
                return true;           
        }
    }
}
