
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
        /// 缓存
        /// </summary>
        /// <returns></returns>
        private List<Role> Cache_Get_RoleList()
        {

            return CacheHelper.Get<List<Role>>(roleKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<Role> list = db.Role.ToList();
                    return list;
                }
            });
        }

        /// <summary>
        /// 缓存 dic
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Role> Cache_Get_RoleList_Dic()
        {
            return Cache_Get_RoleList().ToDictionary(x => x.ID);
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
                var query = Cache_Get_RoleList().AsQueryable().AsNoTracking().Where(x=>(x.Flag&(long)GlobalFlag.Removed)==0);
                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }

                var count = query.Count();
                var list = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
                model.ID = Guid.NewGuid().ToString("N");
                model.CreatedTime = DateTime.Now;
                model.Flag = (long)GlobalFlag.Normal;
                model.UpdatedTime = DateTime.Now;
                model.UpdaterID = Client.LoginUser.ID;
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
        public WebResult<bool> Update_RoleOperate(string ID, long OperateFlag)
        {
            using (DbRepository entities = new DbRepository())
            {
                var oldEntity = entities.Role.Find(ID);
                if (oldEntity != null)
                {
                    var userList = Cache_Get_UserList();
                    oldEntity.OperateFlag = OperateFlag;
                    oldEntity.UpdaterID = Client.LoginUser.ID;
                    oldEntity.UpdatedTime = DateTime.Now;
                    if (oldEntity.ID.Equals("a7acd041b51d4aa0bc226da93957b29c"))
                    {
                        entities.User.Where(x => x.RoleID.Equals(ID)).ToList().ForEach(x =>
                        {
                            x.OperateFlag = OperateFlag;
                            var index = userList.FindIndex(y => y.ID.Equals(x.ID));
                            if (index > -1)
                            {
                                userList[index] = x;
                            }
                            else
                            {
                                userList.Add(x);
                            }
                        });
                    }
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_RoleList();
                    var index = list.FindIndex(x => x.ID.Equals(ID));
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
                    oldEntity.MenuIDStr = model.MenuIDStr;
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
        public Role Find_Role(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
                return Cache_Get_RoleList().AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Delete_Role(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_RoleList();
                //找到实体
                entities.Role.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    //教练员不能删除
                    if (!x.ID.Equals("a7acd041b51d4aa0bc226da93957b29c"))
                    {
                        x.Flag = x.Flag | (long)GlobalFlag.Removed;
                        var index = list.FindIndex(y => y.ID.Equals(x.ID));
                        if (index > -1)
                        {
                            list[index] = x;
                        }
                        else
                        {
                            list.Add(x);
                        }
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
        /// 获取选择项
        /// </summary>
        /// <param name="roleFlag">角色flag值</param>
        /// <returns></returns>
        public List<SelectItem> Get_RoleSelectItem(string id)
        {
            List<SelectItem> list = new List<SelectItem>();

            Cache_Get_RoleList().AsQueryable().AsNoTracking().OrderBy(x => x.CreatedTime).ToList().ForEach(x =>
            {
                list.Add(new SelectItem()
                {
                    Selected = x.ID.Equals(id),
                    Text = x.Name,
                    Value = x.ID
                });
            });
            return list;
        }
    }
}
