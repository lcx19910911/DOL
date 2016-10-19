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
        string dataDictionaryKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "DataDictionary");

        /// <summary>
        /// 站内通知全局缓存
        /// </summary>
        /// <returns></returns>
        private List<DataDictionary> Cache_Get_DataDictionaryList()
        {

            return CacheHelper.Get<List<DataDictionary>>(dataDictionaryKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<DataDictionary> list = db.DataDictionary.OrderByDescending(x => x.Sort).ThenBy(x => x.ID).ToList();
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
        public WebResult<PageList<DataDictionary>> Get_DataDictionaryPageList(int pageIndex, int pageSize, string name, string no)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_DataDictionaryList().AsQueryable().AsNoTracking();
                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Key .Contains(name));
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
        public WebResult<bool> Add_DataDictionary(DataDictionary model)
        {
            using (DbRepository entities = new DbRepository())
            {
                model.ID = Guid.NewGuid().ToString("N");
                entities.DataDictionary.Add(model);
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_DataDictionaryList();
                    list.Add(model);
                    CacheHelper.Insert<List<DataDictionary>>(dataDictionaryKey, list);
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
        public WebResult<bool> Update_DataDictionary(DataDictionary model)
        {
            using (DbRepository entities = new DbRepository())
            {
                var oldEntity = entities.DataDictionary.Find(model.ID);
                if (oldEntity != null)
                {
                    oldEntity.Key = model.Key;
                    oldEntity.Value = model.Value;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_DataDictionaryList();
                    var cachItem = list.FirstOrDefault(x => x.ID.Equals(model.ID));
                    if (cachItem != null)
                    {
                        cachItem = oldEntity;
                        CacheHelper.Insert<List<DataDictionary>>(dataDictionaryKey, list);
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
        public WebResult<bool> Delete_DataDictionary(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_DataDictionaryList();
                //找到实体
                entities.DataDictionary.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    entities.DataDictionary.Remove(x);
                    list.Remove(x);
                });
                if (entities.SaveChanges() > 0)
                {
                    CacheHelper.Insert<List<DataDictionary>>(dataDictionaryKey, list);
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
        public DataDictionary Find_DataDictionary(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            using (DbRepository entities = new DbRepository())
            {
                return Cache_Get_DataDictionaryList().FirstOrDefault(x => x.ID.Equals(id));
            }
        }
        

    }
}
