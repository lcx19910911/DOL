
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
        string referenceKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "Reference");

        /// <summary>
        /// 缓存
        /// </summary>
        /// <returns></returns>
        public List<Reference> Cache_Get_ReferenceList()
        {

            return CacheHelper.Get<List<Reference>>(referenceKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<Reference> list = db.Reference.OrderByDescending(x => x.CreatedTime).ThenBy(x => x.ID).ToList();
                    return list;
                }
            });
        }

        /// <summary>
        /// 缓存 dic
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,Reference> Cache_Get_ReferenceList_Dic()
        {
            return Cache_Get_ReferenceList().ToDictionary(x => x.ID);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<Reference>> Get_ReferencePageList(int pageIndex, int pageSize, string name)
        {
            using (DbRepository entities = new DbRepository())
            {
               var query = Cache_Get_ReferenceList().AsQueryable().AsNoTracking();
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
        public WebResult<bool> Add_Reference(Reference model)
        {
            using (DbRepository entities = new DbRepository())
            {
                model.ID = Guid.NewGuid().ToString("N");
                model.CreatedTime = DateTime.Now;
                model.Flag = (long)GlobalFlag.Normal;
                model.UpdatedTime = DateTime.Now;
                model.UpdaterID = Client.LoginUser.ID;
                entities.Reference.Add(model);
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_ReferenceList();
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
        public WebResult<bool> Update_Reference(Reference model)
        {
            using (DbRepository entities = new DbRepository())
            {
                var oldEntity = entities.Reference.Find(model.ID);
                if (oldEntity != null)
                {
                    oldEntity.Mobile = model.Mobile;
                    oldEntity.Name = model.Name;
                    oldEntity.EnteredPointIDStr = model.EnteredPointIDStr;
                    oldEntity.GenderCode = model.GenderCode;
                    oldEntity.UpdaterID = Client.LoginUser.ID;
                    oldEntity.UpdatedTime = DateTime.Now;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_ReferenceList();
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
        public Reference Find_Reference(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
             return Cache_Get_ReferenceList().AsQueryable().AsNoTracking().FirstOrDefault(x=>x.ID.Equals(id));
        }


        /// <summary>
        /// 下拉框集合
        /// </summary>
        /// <param name="">门店id</param>
        /// <returns></returns>
        public List<SelectItem> Get_ReferenceSelectItem(string enteredPointId)
        {
            using (DbRepository entities = new DbRepository())
            {
                List<SelectItem> list = new List<SelectItem>();

                var query = Cache_Get_ReferenceList().AsQueryable().Where(x=>(x.Flag&(long)GlobalFlag.Removed)==0).AsNoTracking();

                if (string.IsNullOrEmpty(enteredPointId))
                {
                    query.OrderBy(x => x.CreatedTime).ToList().ForEach(x =>
                    {
                        list.Add(new SelectItem()
                        {
                            Text = x.Name,
                            Value = x.ID
                        });
                    });
                }
                else
                {
                    query.Where(x=> x.EnteredPointIDStr.IsNotNullOrEmpty()&&x.EnteredPointIDStr.Contains(enteredPointId)&& (x.Flag & (long)GlobalFlag.Removed) == 0).OrderBy(x => x.CreatedTime).ToList().ForEach(x =>
                    {
                        list.Add(new SelectItem()
                        {
                            Text = x.Name,
                            Value = x.ID
                        });
                    });
                }
                return list;

            }
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Delete_Reference(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_ReferenceList();
                //找到实体
                entities.Reference.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    entities.Reference.Remove(x);
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


        public Reference Get_ReferenceByName(string name)
        {
            return Cache_Get_ReferenceList().Where(x => x.Name.Equals(name)).FirstOrDefault();
        }
    }
}
