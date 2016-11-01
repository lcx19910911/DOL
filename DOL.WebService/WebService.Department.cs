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
        string departmentKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "Department");

        /// <summary>
        /// 全局缓存
        /// </summary>
        /// <returns></returns>
        private List<Department> Cache_Get_DepartmentList()
        {

            return CacheHelper.Get<List<Department>>(departmentKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<Department> list = db.Department.OrderByDescending(x => x.Sort).ThenBy(x => x.ID).ToList();
                    return list;
                }
            });
        }

        /// <summary>
        /// 全局缓存 dic
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Department> Cache_Get_DepartmentList_Dic()
        {
            return Cache_Get_DepartmentList().ToDictionary(x => x.ID);
        }


        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<Department>> Get_DepartmentPageList(int pageIndex, int pageSize, string name, string no)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_DepartmentList().AsQueryable().AsNoTracking().Where(x=>(x.Flag|(long)GlobalFlag.Normal)==0);
                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name .Contains(name));
                }

                var count = query.Count();
                var list = query.OrderByDescending(x => x.Sort).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var deparmentDic = Cache_Get_DepartmentList_Dic();
                list.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.ParentID) && deparmentDic.ContainsKey(x.ParentID))
                    {
                        x.ParentName = deparmentDic[x.ParentID]?.Name;
                    }
                });
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_Department(Department model)
        {
            using (DbRepository entities = new DbRepository())
            {
                model.ID = Guid.NewGuid().ToString("N");
                model.CreatedTime = DateTime.Now;
                model.UpdatedTime = DateTime.Now;
                model.Flag = (long)GlobalFlag.Normal;
                model.UpdaterID = Client.LoginUser.ID;
                entities.Department.Add(model);
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_DepartmentList();
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
        public WebResult<bool> Update_Department(Department model)
        {
            using (DbRepository entities = new DbRepository())
            {
                var oldEntity = entities.Department.Find(model.ID);
                if (oldEntity != null)
                {
                    oldEntity.Name = model.Name;
                    oldEntity.Sort = model.Sort;
                    oldEntity.ParentID = model.ParentID;
                    oldEntity.Remark = model.Remark;
                    oldEntity.No = model.No;
                    oldEntity.Telephone = model.Telephone;
                    oldEntity.UpdaterID = Client.LoginUser.ID;
                    oldEntity.UpdatedTime = DateTime.Now;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_DepartmentList();
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
        public WebResult<bool> Delete_Department(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_DepartmentList();
                //找到实体
                entities.Department.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
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
        /// 获取分类下拉框集合
        /// </summary>
        /// <param name="">门店id</param>
        /// <returns></returns>
        public List<SelectItem> Get_DepartmentSelectItem(string storeId)
        {
            using (DbRepository entities = new DbRepository())
            {
                List<SelectItem> list = new List<SelectItem>();

                var query = Cache_Get_DepartmentList().OrderByDescending(x => x.Sort).AsQueryable().AsNoTracking();

                query.OrderBy(x => x.CreatedTime).ToList().ForEach(x =>
                {
                    list.Add(new SelectItem()
                    {
                        Selected = x.ID.Equals(storeId),
                        Text = x.Name,
                        Value = x.ID
                    });
                });
                return list;

            }
        }


        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Department Find_Department(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;

                return Cache_Get_DepartmentList().AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
        }

        /// <summary>
        /// 获取ZTree子节点
        /// </summary>
        /// <param name="parentId">父级id</param>
        /// <param name="groups">分组数据</param>
        /// <returns></returns>
        private List<ZTreeNode> Get_DepartmentZTreeChildren(string parentId, List<IGrouping<string, Department>> groups)
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
                        children = Get_DepartmentZTreeChildren(x.ID, groups)
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
        public List<ZTreeNode> Get_DepartmentZTreeChildren(string parentId)
        {
            List<ZTreeNode> ztreeNodes = new List<ZTreeNode>();
            var group = Cache_Get_DepartmentList().AsQueryable().AsNoTracking().Where(x=>(x.Flag|(long)GlobalFlag.Normal)==0).OrderByDescending(x => x.Sort).GroupBy(x => x.ParentID).ToList();
            return Get_DepartmentZTreeChildren(parentId, group);
        }

    }
}
