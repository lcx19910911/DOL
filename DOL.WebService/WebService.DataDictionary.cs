﻿
using DOL.Core;
using DOL.Model;
using DOL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DOL.Service
{
    public partial class WebService
    {
        string dictionaryKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "DataDictionary");


        public Dictionary<GroupCode, Dictionary<string, DataDictionary>> Cache_Get_DataDictionary()
        {

            return CacheHelper.Get<Dictionary<GroupCode, Dictionary<string, DataDictionary>>>(dictionaryKey, () =>
            {
                var listss = new List<string>();
                using (var db = new DbRepository())
                {
                    var dic = db.DataDictionary.GroupBy(x => x.GroupCode).ToDictionary(x => x.Key, x => x.ToList());
                    return dic.ToDictionary(x => x.Key,
                        x => {
                            var returnDic = new Dictionary<string, DataDictionary>();
                            x.Value.ForEach(y =>
                            {
                                if (!returnDic.ContainsKey(y.Key))
                                {
                                    returnDic.Add(y.Key, y);
                                }
                            });
                            return returnDic;
                        });
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
        public WebResult<PageList<DataDictionary>> Get_GroupPageList(int pageIndex, int pageSize, string parentKey, GroupCode group, string key, string value)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = entities.DataDictionary.AsNoTracking().Where(x=>x.GroupCode==group);
                if (parentKey.IsNullOrEmpty())
                {
                    query = query.Where(x => string.IsNullOrEmpty(x.ParentKey));
                }
                else
                {
                    query = query.Where(x => x.ParentKey.Equals(parentKey.Trim()));
                }
                if (key.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Key.Contains(key));
                }
                if (value.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Value.Contains(value));
                }
                var count = query.Count();
                var list = query.OrderByDescending(x => x.Sort).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                list.ForEach(x=>{
                    if (!string.IsNullOrEmpty(x.ParentKey))
                    {
                        if (x.GroupCode == GroupCode.Area)
                        {
                            x.ParentName = Cache_Get_DataDictionary()[group][x.ParentKey.Trim()]?.Value;
                        }
                        else if (x.GroupCode == GroupCode.College)
                        {
                            x.ParentName = Cache_Get_DataDictionary()[GroupCode.School][x.ParentKey.Trim()]?.Value;
                        }
                        else if (x.GroupCode == GroupCode.Major)
                        {
                            x.ParentName = Cache_Get_DataDictionary()[GroupCode.College][x.ParentKey.Trim()]?.Value;
                        }
                    }
                });
                return ResultPageList(list, pageIndex, pageSize, count);
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
        public WebResult<PageList<DataDictionary>> Get_DataDictionaryPageList(int pageIndex, int pageSize, GroupCode group, string key, string value)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = entities.DataDictionary.AsNoTracking().Where(x => x.GroupCode == group&&x.ID!= "df84ff2a25ff462aac24543976e56d62");

                if (key.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Key.Contains(key));
                }
                if (value.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Value.Contains(value));
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
        public WebResult<bool> Add_DataDictionary(DOL.Model.DataDictionary model)
        {
            using (DbRepository entities = new DbRepository())
            {
                if(entities.DataDictionary.Where(x=>x.GroupCode==GroupCode.Area&&x.Key.Equals(model.Key)).Any())
                    return Result(false, ErrorCode.sys_param_format_error);

                model.ID = Guid.NewGuid().ToString("N");
                if (model.GroupCode == GroupCode.School || model.GroupCode == GroupCode.School || model.GroupCode == GroupCode.School)
                {
                    if (entities.DataDictionary.Where(x => x.GroupCode == model.GroupCode && x.Value.Equals(model.Value)).Any())
                        return Result(false, ErrorCode.datadatabase_name_had);
                }
                if(string.IsNullOrEmpty(model.Key))
                    model.Key = model.ID;
                entities.DataDictionary.Add(model);
                if (entities.SaveChanges() > 0)
                {
                    CacheHelper.Clear();
                    return Result(true);
                }
                else
                {
                    return Result(false, ErrorCode.sys_fail);
                }
            }

        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Delete_DataDictionary(string ID)
        {
            using (DbRepository entities = new DbRepository())
            {
                var dataDic = entities.DataDictionary.Find(ID);
                if (dataDic != null)
                {
                    entities.DataDictionary.Remove(dataDic);
                    if (entities.SaveChanges() > 0)
                    {
                        CacheHelper.Clear();
                        return Result(true);
                    }
                    else
                    {
                        return Result(false, ErrorCode.sys_fail);
                    }
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);
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
                if (entities.DataDictionary.Where(x => x.GroupCode == GroupCode.Area && x.Key.Equals(model.Key)&&!x.ID.Equals(model.ID)).Any())
                    return Result(false, ErrorCode.sys_param_format_error);
                if (model.GroupCode == GroupCode.School || model.GroupCode == GroupCode.School || model.GroupCode == GroupCode.School)
                {
                    if (entities.DataDictionary.Where(x => x.GroupCode == model.GroupCode && x.Value.Equals(model.Value) && !x.ID.Equals(model.ID)).Any())
                        return Result(false, ErrorCode.datadatabase_name_had);
                }
                var oldEntity = entities.DataDictionary.Find(model.ID);
                if (oldEntity != null)
                {
                    if (string.IsNullOrEmpty(model.Key))
                        oldEntity.Key = model.ID;
                    oldEntity.ParentKey = model.ParentKey;
                    oldEntity.Sort = model.Sort;
                    if(model.Key.IsNotNullOrEmpty())
                        oldEntity.Key = model.Key;
                    oldEntity.Remark = model.Remark;
                    oldEntity.Value = model.Value;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    CacheHelper.Clear();
                    return Result(true);
                }
                else
                {
                    return Result(false, ErrorCode.sys_fail);
                }
            }

        }


        /// <summary>
        /// 根据分组类型和key获取数值
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(GroupCode code, string key)
        {
            var data_Dictionary = Cache_Get_DataDictionary()[code][key];
            if (data_Dictionary != null)
            {
                return data_Dictionary.Value;
            }
            return null;
        }

        /// <summary>
        /// 获取数据字典对象
        /// </summary>
        /// <returns></returns>
        public DataDictionary Find_DataDictionary(string ID)
        {
            using (DbRepository entities = new DbRepository())
            {
                return entities.DataDictionary.Find(ID);
            }
        }

        /// <summary>
        /// 获取地区数据
        /// </summary>
        /// <param name="value">地区编码</param>
        /// <returns></returns>
        public List<SelectItem> Get_AreaList(string value)
        {
            var areas = Cache_Get_DataDictionary()[GroupCode.Area].Values.OrderByDescending(x => x.Sort).ToList().AsQueryable();
            if (!string.IsNullOrEmpty(value)&&!value.Equals("0"))
                areas = areas.Where(x=>!string.IsNullOrEmpty(x.ParentKey)&&x.ParentKey.Trim().Equals(value));
            else
                areas = areas.Where(_ => string.IsNullOrEmpty(_.ParentKey));
            var alist = areas.ToList();
            var list=alist.Select(x => new SelectItem() { Value = x.Key, Text = x.Value }).ToList();
            list.Insert(0, new SelectItem() { Value = "0", Text = "点击选择..." });
            return list;
        }

        /// <summary>
        /// 获取地区数据
        /// </summary>
        /// <param name="value">地区编码</param>
        /// <returns></returns>
        public List<SelectItem> Get_SchoolList(string parentId,GroupCode code)
        {
            var query = Cache_Get_DataDictionary()[code].Values.OrderByDescending(x => x.Sort).ToList().AsQueryable();
            if (!string.IsNullOrEmpty(parentId) && !parentId.Equals("0"))
                query = query.Where(x => !string.IsNullOrEmpty(x.ParentKey) && x.ParentKey.Trim().Equals(parentId));
            else
                query = query.Where(_ => string.IsNullOrEmpty(_.ParentKey));
            var list = query.ToList();
            var itemList = list.Select(x => new SelectItem() { Value = x.Key, Text = x.Value }).ToList();
            itemList.Insert(0, new SelectItem() { Value = "0", Text = "点击选择..." });
            return itemList;
        }

        /// <summary>
        /// 获取地区数据
        /// </summary>
        /// <param name="value">地区编码</param>
        /// <returns></returns>
        public Tuple<string,string> Get_ByCityCode(string cityCode)
        {
            
            var provniceName = GetValue(GroupCode.Area, cityCode.Substring(0, 2) + "0000");
            var cityName = GetValue(GroupCode.Area, cityCode);
            return new Tuple<string, string>(provniceName, cityName);
        }


        /// <summary>
        /// 获取下拉框
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public List<SelectItem> Get_DataDictorySelectItem(GroupCode group, Func<DataDictionary, bool> predicate = null)
        {
            var list = new List<SelectItem>();
            var dataDic = Cache_Get_DataDictionary();
            if (dataDic.Keys.Contains(group))
            {
                var dic = dataDic[group];
                var itemList = dic.Values.Where(x=> x.ID!= "df84ff2a25ff462aac24543976e56d62");
                if(predicate != null)
                {
                   itemList.Where(x=>predicate(x)).OrderByDescending(x=>x.Sort).ToList().ForEach(x =>
                   {
                       list.Add(new SelectItem()
                       {
                           Text = x.Value,
                           Value = x.Key,
                           ParentKey=x.ParentKey
                       });
                   }); 
                }
                else
                {
                    itemList.OrderByDescending(x => x.Sort).ToList().ForEach(x =>
                      {
                          list.Add(new SelectItem()
                          {
                              Text = x.Value,
                              Value = x.Key,
                              ParentKey = x.ParentKey
                          });
                      });
                }
                return list;
            }
            else
                return null;
        }
    }
}
