﻿using DOL.Core;
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
        string themeSalaryKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "ThemeSalary");

        /// <summary>
        /// 全局缓存
        /// </summary>
        /// <returns></returns>
        private List<ThemeSalary> Cache_Get_ThemeSalaryList()
        {

            return CacheHelper.Get<List<ThemeSalary>>(themeSalaryKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<ThemeSalary> list = db.ThemeSalary.OrderByDescending(x => x.CreatedTime).ThenBy(x => x.ID).ToList();
                    return list;
                }
            });
        }

        /// <summary>
        /// 全局缓存 dic
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, ThemeSalary> Cache_Get_ThemeSalaryList_Dic()
        {
            return Cache_Get_ThemeSalaryList().ToDictionary(x => x.ID);
        }


        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ThemeSalary Find_ThemeSalary(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            return Cache_Get_ThemeSalaryList().AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns></returns>
        public WebResult<PageList<ThemeSalary>> Get_ThemeSalaryPageList(
            int pageIndex,
            int pageSize,
            string name
            )
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_ThemeSalaryList().Where(x=>x.Flag==(long)GlobalFlag.Normal).AsQueryable().AsNoTracking();
                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }
                var count = query.Count();
                var list = query.OrderBy(x => x.Code).ThenBy(x=>x.Count).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }


        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_ThemeSalary(ThemeSalary model)
        {
            using (DbRepository entities = new DbRepository())
            {
                model.ID = Guid.NewGuid().ToString("N");
                if (Cache_Get_ThemeSalaryList().Where(x => x.Code == model.Code && x.Count == model.Count&&!x.EndTime.HasValue).Any())
                    return Result(false, ErrorCode.count_had_exit);
                model.CreatedTime = DateTime.Now;
                model.UpdatedTime = DateTime.Now;
                model.UpdaterID = Client.LoginUser.ID;
                entities.ThemeSalary.Add(model);
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_ThemeSalaryList();
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
        public WebResult<bool> Update_ThemeSalary(ThemeSalary model)
        {
            using (DbRepository entities = new DbRepository())
            {
                if (Cache_Get_ThemeSalaryList().Where(x => x.Code == model.Code && x.Count == model.Count && !x.EndTime.HasValue&&!x.ID.Equals(model.ID)).Any())
                    return Result(false, ErrorCode.count_had_exit);

                var oldEntity = entities.ThemeSalary.Find(model.ID);
                var newEntity = new ThemeSalary();
                if (oldEntity != null)
                {
                    oldEntity.UpdatedTime = DateTime.Now;
                    oldEntity.UpdaterID = Client.LoginUser.ID;
                    oldEntity.Money = model.Money;
                    oldEntity.Count = model.Count;
                    oldEntity.Code = model.Code;
                    oldEntity.Name = model.Name;
                    //如修改金额和次数 把以前数据隐藏 新增数据
                    if (oldEntity.Money != model.Money || oldEntity.Count != model.Count)
                    {
                        oldEntity.Flag = (long)GlobalFlag.Unabled;
                        oldEntity.EndTime = DateTime.Now;
                        newEntity.ID = Guid.NewGuid().ToString("N");
                        newEntity.CreatedTime = DateTime.Now.AddMinutes(1);
                        newEntity.UpdatedTime = DateTime.Now;
                        newEntity.UpdaterID = Client.LoginUser.ID;
                        newEntity.Code = model.Code;
                        newEntity.Name = model.Name;
                        newEntity.Count = model.Count;
                        newEntity.Money = model.Money;
                        newEntity.Flag = (long)GlobalFlag.Normal;
                        entities.ThemeSalary.Add(newEntity);
                    }
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_ThemeSalaryList();
                    var index = list.FindIndex(x => x.ID.Equals(model.ID));
                    if (index > -1)
                    {
                        list[index] = oldEntity;
                    }
                    else
                    {
                        list.Add(oldEntity);
                    }
                    if (newEntity.ID.IsNotNullOrEmpty())
                    {
                        list.Add(newEntity);
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
        /// 删除考试记录
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Delete_ThemeSalary(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_ThemeSalaryList();
                //找到实体
                entities.ThemeSalary.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    entities.ThemeSalary.Remove(x);
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
    }
}
