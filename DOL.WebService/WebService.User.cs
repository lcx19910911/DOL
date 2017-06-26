
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
        string userKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "User");
        string userMenuKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "UserMenu");
        /// <summary>
        /// 全局缓存
        /// </summary>
        /// <returns></returns>
        private List<User> Cache_Get_UserList()
        {

            return CacheHelper.Get<List<User>>(userKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<User> list = db.User.OrderByDescending(x => x.CreatedTime).ThenBy(x => x.ID).ToList();
                    return list;
                }
            });
        }

        /// <summary>
        /// 全局缓存
        /// </summary>
        /// <returns></returns>
        private string Cache_Get_UserMenu()
        {

            var user = Cache_Get_UserList().Where(x => x.ID.Equals(Client.LoginUser.ID)).FirstOrDefault();
            if (user != null)
            {
                return user.MenuIDStr.IsNotNullOrEmpty() ? user.MenuIDStr : "";
            }
            else
                return "";

        }

        /// <summary>
        /// 全局缓存 dic
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, User> Cache_Get_UserDic()
        {
            return Cache_Get_UserList().ToDictionary(x => x.ID);
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <returns></returns> 
        public WebResult<bool> Login(string loginName, string password)
        {
            try
            {
                if (loginName.IsNullOrEmpty() || password.IsNullOrEmpty())
                {
                    return Result(false, ErrorCode.sys_param_format_error);
                }
                using (var db = new DbRepository())
                {
                    string md5Password = CryptoHelper.MD5_Encrypt(password);
                    var user = Cache_Get_UserList().AsQueryable().AsNoTracking().Where(x => x.Password.Equals(md5Password) && x.Account.Equals(loginName)).FirstOrDefault();
                    if (user == null)
                        return Result(false, ErrorCode.user_login_error);
                    else if ((user.Flag & (long)GlobalFlag.Unabled) != 0)
                    {
                        return Result(false, ErrorCode.user_disabled);
                    }
                    else if ((user.Flag & (long)GlobalFlag.Removed) != 0)
                    {
                        return Result(false, ErrorCode.user_not_exit);
                    }

                    else
                    {
                        if (user.OperateFlag.HasValue)
                            user.OperateList = Get_UserOperateList(user.OperateFlag.Value);
                        if (user.RoleID.IsNotNullOrEmpty() && user.RoleID.Trim() != "1")
                        {
                            var role = db.Role.Find(user.RoleID);
                            if (role == null)
                            {
                                return Result(false, ErrorCode.sys_param_format_error);
                            }
                            user.IsNotShowMoney = role.IsNotShowMoney==YesOrNoCode.Yes;
                        }
                        Client.Session[Params.UserCookieName] = CryptoHelper.AES_Encrypt(user.ToJson(), Params.SecretKey);
                        return Result(true);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteException(ex);
                return Result(false, ErrorCode.sys_fail);
            }
        }

        /// <summary>
        /// 用户修改密码
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <returns></returns> 
        public WebResult<bool> ChangePassword(string oldPassword, string newPassword, string cfmPassword)
        {
            try
            {
                if (oldPassword.IsNullOrEmpty() || newPassword.IsNullOrEmpty() || cfmPassword.IsNullOrEmpty())
                {
                    return Result(false, ErrorCode.sys_param_format_error);
                }
                if (!cfmPassword.Equals(newPassword))
                {
                    return Result(false, ErrorCode.user_password_notequal);

                }
                using (var db = new DbRepository())
                {
                    oldPassword = CryptoHelper.MD5_Encrypt(oldPassword);

                    var user = db.User.Where(x => x.ID.Equals(this.Client.LoginUser.ID)).FirstOrDefault();
                    if (user == null)
                        return Result(false, ErrorCode.user_not_exit);
                    if (!user.Password.Equals(oldPassword))
                        return Result(false, ErrorCode.user_password_nottrue);
                    newPassword = CryptoHelper.MD5_Encrypt(newPassword);
                    user.Password = newPassword;
                    Client.Session[Params.UserCookieName] = CryptoHelper.AES_Encrypt(user.ToJson(), Params.SecretKey);
                    if (db.SaveChanges() > 0)
                    {
                        var list = Cache_Get_UserList();
                        var index = list.FindIndex(x => x.ID.Equals(user.ID));
                        if (index > -1)
                        {
                            list[index] = user;
                        }
                        else
                        {
                            list.Add(user);
                        }
                        return Result(true);
                    }
                    else
                    {
                        return Result(false, ErrorCode.sys_fail);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteException(ex);
                return Result(false, ErrorCode.sys_fail);
            }
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<User>> Get_UserPageList(int pageIndex, int pageSize, string name, string mobile, DateTime? startTimeStart, DateTime? endTimeEnd)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_UserList().AsQueryable().AsNoTracking().AsNoTracking().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0 && !x.ID.Equals(this.Client.LoginUser.ID) && x.ID != "2dad7156a2b644c98cea08e52ab1ddb1");

                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }
                if (mobile.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Mobile.Contains(mobile));
                }
                if (startTimeStart != null)
                {
                    query = query.Where(x => x.CreatedTime >= startTimeStart);
                }
                if (endTimeEnd != null)
                {
                    endTimeEnd = endTimeEnd.Value.AddDays(1);
                    query = query.Where(x => x.CreatedTime < endTimeEnd);
                }

                var count = query.Count();
                var departDic = Cache_Get_DepartmentList_Dic();
                var roleDic = Cache_Get_RoleList_Dic();
                var list = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                list.ForEach(x =>
                {
                    //推荐人
                    if (!string.IsNullOrEmpty(x.RoleID) && roleDic.ContainsKey(x.RoleID))
                        x.RoleName = roleDic[x.RoleID]?.Name;
                    //推荐人
                    if (!string.IsNullOrEmpty(x.DepartmentID) && departDic.ContainsKey(x.DepartmentID))
                        x.DepartmentName = departDic[x.DepartmentID]?.Name;
                    x.State = EnumHelper.GetEnumDescription((GlobalFlag)x.Flag);
                });
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }


        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_User(User model)
        {
            using (DbRepository entities = new DbRepository())
            {
                if (Cache_Get_UserList().AsQueryable().AsNoTracking().Where(x => x.Mobile.Equals(model.Mobile) && x.Flag == (long)GlobalFlag.Normal).Any())
                    return Result(false, ErrorCode.datadatabase_mobile__had);
                if (Cache_Get_UserList().AsQueryable().AsNoTracking().Where(x => x.Account.Equals(model.Account) && x.Flag == (long)GlobalFlag.Normal).Any())
                    return Result(false, ErrorCode.user_name_already_exist);
                var role = Cache_Get_RoleList().Where(x => x.ID.Equals(model.RoleID)).FirstOrDefault();
                if (role == null)
                    return Result(false, ErrorCode.datadatabase_primarykey_not_found);
                model.OperateFlag = role.OperateFlag;
                model.Password = CryptoHelper.MD5_Encrypt(model.ConfirmPassword);
                model.ID = Guid.NewGuid().ToString("N");
                model.CreaterId = Client.LoginUser.ID;
                model.CreatedTime = DateTime.Now;
                model.UpdatedTime = DateTime.Now;
                model.Flag = (long)GlobalFlag.Normal;
                model.UpdaterID = Client.LoginUser.ID;
                string menuIdStr = string.Empty;
                if (Cache_Get_RoleList_Dic().ContainsKey(model.RoleID))
                {
                    model.MenuIDStr = Cache_Get_RoleList_Dic()[model.RoleID].MenuIDStr;
                }
                entities.User.Add(model);
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_UserList();
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
        public WebResult<bool> Update_User(User model)
        {
            using (DbRepository entities = new DbRepository())
            {
                if (Cache_Get_UserList().AsQueryable().AsNoTracking().Where(x => x.Mobile.Equals(model.Mobile) && !x.ID.Equals(model.ID) && x.Flag == (long)GlobalFlag.Normal).Any())
                    return Result(false, ErrorCode.datadatabase_mobile__had);
                var oldEntity = entities.User.Find(model.ID);
                if (oldEntity != null)
                {
                    var role = Cache_Get_RoleList().Where(x => x.ID.Equals(model.RoleID)).FirstOrDefault();
                    if (role == null)
                        return Result(false, ErrorCode.datadatabase_primarykey_not_found);
                    oldEntity.OperateFlag = role.OperateFlag;
                    oldEntity.Mobile = model.Mobile;
                    oldEntity.Name = model.Name;
                    oldEntity.Remark = model.Remark;
                    oldEntity.UpdaterID = Client.LoginUser.ID;
                    oldEntity.UpdatedTime = DateTime.Now;
                    oldEntity.IsStoreAdmin = model.IsStoreAdmin;
                    string menuIdStr = string.Empty;
                    if (Cache_Get_RoleList_Dic().ContainsKey(model.RoleID))
                    {
                        oldEntity.MenuIDStr = Cache_Get_RoleList_Dic()[model.RoleID].MenuIDStr;
                    }
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_UserList();
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
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update_UserOperate(string ID, long OperateFlag)
        {
            using (DbRepository entities = new DbRepository())
            {
                var oldEntity = entities.User.Find(ID);
                if (oldEntity != null)
                {
                    oldEntity.OperateFlag = OperateFlag;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_UserList();
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
        public WebResult<bool> Update_MenuIDStr(string ID, string menuIDStr)
        {
            using (DbRepository entities = new DbRepository())
            {
                var oldEntity = entities.User.Find(ID);
                if (oldEntity != null)
                {
                    oldEntity.MenuIDStr = menuIDStr;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_UserList();
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
        public WebResult<bool> Update_UserEnteredPointIDStr(string ID, string enteredPointIDStr)
        {
            using (DbRepository entities = new DbRepository())
            {
                var oldEntity = entities.User.Find(ID);
                if (oldEntity != null)
                {
                    oldEntity.EnteredPointIDStr = enteredPointIDStr;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_UserList();
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
        /// 删除分类
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Delete_User(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_UserList();
                var coacheList = Cache_Get_CoachList();
                //找到实体
                entities.User.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    if (x.CoachID.IsNotNullOrEmpty())
                    {
                        var coach = entities.Coach.Find(x.CoachID);
                        if (coach != null)
                        {
                            coach.Flag = coach.Flag | (long)GlobalFlag.Removed;
                            var coachIndex = coacheList.FindIndex(y => y.ID.Equals(x.CoachID));
                            if (coachIndex > -1)
                            {
                                coacheList[coachIndex] = coach;
                            }
                        }
                    }
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
                });
                return entities.SaveChanges() > 0 ? Result(true) : Result(false, ErrorCode.sys_fail);
            }
        }


        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User Find_User(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            using (DbRepository entities = new DbRepository())
            {
                var entity = Cache_Get_UserList().AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
                return entity;
            }
        }


        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="ids">id，多个id用逗号分隔</param>
        /// <returns>影响条数</returns>
        public WebResult<bool> Enable_User(string ids)
        {
            if (string.IsNullOrEmpty(ids))
                return Result(false, ErrorCode.sys_param_format_error);
            using (DbRepository entities = new DbRepository())
            {
                //按逗号分隔符分隔开得到unid列表
                var unidArray = ids.Split(',');
                var list = Cache_Get_UserList();

                entities.User.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    x.Flag = x.Flag & ~(long)GlobalFlag.Unabled;
                    var index = list.FindIndex(y => y.ID.Equals(x.ID));
                    if (index > -1)
                    {
                        list[index] = x;
                    }
                    else
                    {
                        list.Add(x);
                    }
                });

                return entities.SaveChanges() > 0 ? Result(true) : Result(false, ErrorCode.sys_fail);
            }
        }


        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="ids">ids，多个id用逗号分隔</param>
        /// <returns>影响条数</returns>
        public WebResult<bool> Disable_User(string ids)
        {
            if (string.IsNullOrEmpty(ids))
                return Result(false, ErrorCode.sys_param_format_error);
            using (DbRepository entities = new DbRepository())
            {
                //按逗号分隔符分隔开得到unid列表
                var unidArray = ids.Split(',');
                var list = Cache_Get_UserList();
                entities.User.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    x.Flag = x.Flag | (long)GlobalFlag.Unabled;
                    var index = list.FindIndex(y => y.ID.Equals(x.ID));
                    if (index > -1)
                    {
                        list[index] = x;
                    }
                    else
                    {
                        list.Add(x);
                    }
                });

                return entities.SaveChanges() > 0 ? Result(true) : Result(false, ErrorCode.sys_fail);
            }
        }


        /// <summary>
        /// 下拉框集合
        /// </summary>
        /// <param name="">门店id</param>
        /// <returns></returns>
        public List<SelectItem> Get_UserSelectItem()
        {
            using (DbRepository entities = new DbRepository())
            {
                List<SelectItem> list = new List<SelectItem>();

                var query = Cache_Get_UserList().AsQueryable().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0).AsNoTracking();
                query.OrderBy(x => x.CreatedTime).ToList().ForEach(x =>
                {
                    list.Add(new SelectItem()
                    {
                        Text = x.Name,
                        Value = x.ID
                    });
                });

                return list;

            }
        }

    }
}
