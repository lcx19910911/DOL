
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
                    var user = db.User.Where(x => x.Password.Equals(md5Password) && x.Account.Equals(loginName)).FirstOrDefault();
                    if (user == null)
                        return Result(false, ErrorCode.user_login_error);
                    else if (user.ExpireTime < DateTime.Now)
                    {
                        user.Flag = user.Flag & (long)GlobalFlag.Unabled;
                        db.SaveChanges();
                        return Result(false, ErrorCode.user_expire);
                    }
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
                        CookieHelper.CreateCookie(user);
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

                    CookieHelper.CreateCookie(user);
                    return db.SaveChanges() > 0 ? Result(true) : Result(false, ErrorCode.sys_fail); ;
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
        public WebResult<PageList<User>> Get_UserPageList(int pageIndex, int pageSize, string name, string account, string phone, bool isShowAdmin, DateTime? exprieTimeStart, DateTime? exprieTimeEnd)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = entities.User.AsQueryable().AsNoTracking().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0 && !x.ID.Equals(this.Client.LoginUser.ID));

                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }
                if (account.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Account.Contains(account));
                }

                if (phone.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Mobile.Contains(phone));
                }
                if (exprieTimeStart != null)
                {
                    query = query.Where(x => x.ExpireTime >= exprieTimeStart);
                }
                if (exprieTimeEnd != null)
                {
                    exprieTimeEnd = exprieTimeEnd.Value.AddDays(1);
                    query = query.Where(x => x.ExpireTime < exprieTimeEnd);
                }
                
                var count = query.Count();
                var list = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                list.ForEach(x =>
                {
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
                var query = entities.User.AsQueryable();

                if (entities.User.AsNoTracking().Where(x => x.Account.Equals(model.Account)).Any())
                    return Result(false, ErrorCode.user_name_already_exist);
                model.Password = CryptoHelper.MD5_Encrypt(model.ConfirmPassword);
                model.ID = Guid.NewGuid().ToString("N");
                model.CreaterId = Client.LoginUser.ID;
                model.CreatedTime = DateTime.Now;
                model.UpdatedTime = DateTime.Now;
                model.Flag = (long)GlobalFlag.Normal;
                if (Client.LoginUser.IsAdmin == YesOrNoCode.Yes)
                    model.IsAdmin = YesOrNoCode.Yes;
                else
                    model.IsAdmin = YesOrNoCode.No;
                entities.User.Add(model);
                return entities.SaveChanges() > 0 ? Result(true) : Result(false, ErrorCode.sys_fail);
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
                var oldEntity = entities.User.Find(model.ID);
                if (oldEntity != null)
                {
                    oldEntity.Mobile = model.Mobile;
                    oldEntity.Name = model.Name;
                    oldEntity.MenuFlag = model.MenuFlag;
                    oldEntity.Remark = model.Remark;
                    oldEntity.UpdatedTime = DateTime.Now;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                return entities.SaveChanges() > 0 ? Result(true) : Result(false, ErrorCode.sys_fail);
            }

        }


        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_Admin(User model)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = entities.User.AsQueryable();

                if (entities.User.AsNoTracking().Where(x => x.Account.Equals(model.Account)).Any())
                    return Result(false, ErrorCode.user_name_already_exist);
                model.ID = Guid.NewGuid().ToString("N");
                model.CreaterId = Client.LoginUser.ID;
                model.CreatedTime = DateTime.Now;
                model.UpdatedTime = DateTime.Now;
                model.Flag = (long)GlobalFlag.Normal;
                model.IsAdmin = YesOrNoCode.No;
                model.Password = CryptoHelper.MD5_Encrypt(model.ConfirmPassword);
                model.MenuFlag = -1;
                entities.User.Add(model);
                return entities.SaveChanges() > 0 ? Result(true) : Result(false, ErrorCode.sys_fail);
            }

        }


        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update_Admin(User model)
        {
            using (DbRepository entities = new DbRepository())
            {
                var oldEntity = entities.User.Find(model.ID);
                if (oldEntity != null)
                {
                    oldEntity.Mobile = model.Mobile;
                    oldEntity.Remark = model.Remark;
                    oldEntity.ExpireTime = model.ExpireTime;
                    oldEntity.UpdatedTime = DateTime.Now;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                return entities.SaveChanges() > 0 ? Result(true) : Result(false, ErrorCode.sys_fail);
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
                //找到实体
                entities.User.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    x.Flag = x.Flag | (long)GlobalFlag.Removed;
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
                var entity = entities.User.Find(id);
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

                entities.User.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    x.Flag = x.Flag & ~(long)GlobalFlag.Unabled;
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

                entities.User.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    x.Flag = x.Flag | (long)GlobalFlag.Unabled;
                });

                return entities.SaveChanges() > 0 ? Result(true) : Result(false, ErrorCode.sys_fail);
            }
        }

    }
}
