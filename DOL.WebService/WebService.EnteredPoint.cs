
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
        string enteredPointKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "EnteredPoint");

        /// <summary>
        /// 缓存
        /// </summary>
        /// <returns></returns>
        private List<EnteredPoint> Cache_Get_EnteredPointList()
        {

            return CacheHelper.Get<List<EnteredPoint>>(enteredPointKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<EnteredPoint> list = db.EnteredPoint.ToList();
                    return list;
                }
            });
        }

        /// <summary>
        /// 缓存 dic
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, EnteredPoint> Cache_Get_EnteredPoint_Dic()
        {
            return Cache_Get_EnteredPointList().ToDictionary(x => x.ID);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<EnteredPoint>> Get_EnteredPointPageList(int pageIndex, int pageSize, string name,string provinceCode,string cityCode, string districtCode, string no)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_EnteredPointList().AsQueryable().AsNoTracking();
                if (Client.LoginUser.MenuFlag != -1)
                    query = query.Where(x => Client.LoginUser.EnteredPointIDStr.Contains(x.ID));
                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }
                if (provinceCode.IsNotNullOrEmpty()&&!provinceCode.Equals("0"))
                {
                    query = query.Where(x => x.ProvinceCode.Equals(provinceCode));
                }
                if (cityCode.IsNotNullOrEmpty() && !cityCode.Equals("0"))
                {
                    query = query.Where(x => x.CityCode.Equals(cityCode));
                }
                if (districtCode.IsNotNullOrEmpty() && !districtCode.Equals("0"))
                {
                    query = query.Where(x => x.DistrictCode.Equals(districtCode));
                }
                
                var count = query.Count();
                var list = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                list.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.ProvinceCode))
                        x.ProvinceName = GetValue(GroupCode.Area, x.ProvinceCode);
                    if (!string.IsNullOrEmpty(x.CityCode))
                        x.CityName = GetValue(GroupCode.Area, x.CityCode);
                    if (!string.IsNullOrEmpty(x.DistrictCode))
                        x.DistrictName = GetValue(GroupCode.Area, x.DistrictCode);
                });

                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_EnteredPoint(EnteredPoint model)
        {
            using (DbRepository entities = new DbRepository())
            {
                model.ID = Guid.NewGuid().ToString("N");
                model.CreatedTime = DateTime.Now;
                model.Flag = (long)GlobalFlag.Normal;
                model.UpdatedTime = DateTime.Now;
                model.UpdaterID = Client.LoginUser.ID;
                entities.EnteredPoint.Add(model);
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_EnteredPointList();
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
        public WebResult<bool> Update_EnteredPoint(EnteredPoint model)
        {
            using (DbRepository entities = new DbRepository())
            {
                var oldEntity = entities.EnteredPoint.Find(model.ID);
                if (oldEntity != null)
                {
                    oldEntity.ProvinceCode = model.ProvinceCode;
                    oldEntity.CityCode = model.CityCode;
                    oldEntity.DistrictCode = model.DistrictCode;
                    oldEntity.Address = model.Address;
                    oldEntity.ConnactPeople = model.ConnactPeople;
                    oldEntity.Mobile = model.Mobile;
                    oldEntity.Telephone = model.Telephone;
                    oldEntity.Sort = model.Sort;
                    oldEntity.UpdatedTime = DateTime.Now;
                    oldEntity.Name = model.Name;
                    oldEntity.UpdaterID = Client.LoginUser.ID;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_EnteredPointList();
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
        public EnteredPoint Find_EnteredPoint(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
                return Cache_Get_EnteredPointList().AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Delete_EnteredPoint(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_EnteredPointList();
                //找到实体
                entities.EnteredPoint.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
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
        /// <param name="enteredPointFlag">角色flag值</param>
        /// <returns></returns>
        public List<SelectItem> Get_EnteredPointSelectItem(string cityCode)
        {
            List<SelectItem> list = new List<SelectItem>();
            if (!string.IsNullOrEmpty(cityCode))
            {
                //是否管理员
                if (Client.LoginUser.MenuFlag == -1)
                {
                    Cache_Get_EnteredPointList().AsQueryable().AsNoTracking().Where(x => x.CityCode.Equals(cityCode)).OrderBy(x => x.CreatedTime).ToList().ForEach(x =>
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

                    Cache_Get_EnteredPointList().AsQueryable().AsNoTracking().Where(x => x.CityCode.Equals(cityCode)&&Client.LoginUser.EnteredPointIDStr.Contains(x.ID)).OrderBy(x => x.CreatedTime).ToList().ForEach(x =>
                    {
                        list.Add(new SelectItem()
                        {
                            Text = x.Name,
                            Value = x.ID
                        });
                    });
                }
            }
            else
            {
                if (Client.LoginUser.MenuFlag == -1)
                {
                    Cache_Get_EnteredPointList().AsQueryable().AsNoTracking().OrderBy(x => x.CreatedTime).ToList().ForEach(x =>
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

                    Cache_Get_EnteredPointList().AsQueryable().AsNoTracking().Where(x => Client.LoginUser.EnteredPointIDStr.Contains(x.ID)).OrderBy(x => x.CreatedTime).ToList().ForEach(x =>
                    {
                        list.Add(new SelectItem()
                        {
                            Text = x.Name,
                            Value = x.ID
                        });
                    });
                }
            }
            return list;
        }

        /// <summary>
        /// 获取ZTree节点
        /// </summary>
        /// <returns></returns>
        public List<ZTreeNode> Get_EnteredPointZTreeStr()
        {
            List<ZTreeNode> ztreeNodes = new List<ZTreeNode>();
            var list = Cache_Get_EnteredPointList().Where(x => Client.LoginUser.MenuFlag != -1 ? (Client.LoginUser.EnteredPointIDStr.Contains(x.ID)) : 1 == 1).ToList();
            list.GroupBy(x => x.CityCode).ToList().ForEach(x =>
            {
                ztreeNodes.Add(new ZTreeNode()
                {
                    //地区名
                    name = GetValue(GroupCode.Area, x.Key),
                    children = Get_CityEnteredPointList(x.Key, list)
                });
            });

            return ztreeNodes;
        }

        /// <summary>
        /// 获取城市的ztree集合
        /// </summary>
        /// <param name="cityCode"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<ZTreeNode> Get_CityEnteredPointList(string cityCode, List<EnteredPoint> list)
        {
            List<ZTreeNode> ztreeNodes = new List<ZTreeNode>();
            list.Where(x => x.CityCode.Equals(cityCode)).ToList().ForEach(x =>
            {
                ztreeNodes.Add(new ZTreeNode()
                {
                    //地区名
                    name = x.Name,
                    value=x.ID
                });
            });
            return ztreeNodes;
        }


        /// <summary>
        /// 获取报名点的统计报表
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public EnteredPointReportModel Get_EnteredPointReport(DateTime? time,string EnteredPointID)
        {
            //赋值本月
            if (time == null)
                time = DateTime.Parse(DateTime.Now.ToString("yyyy-MM"));

            //本月结束时间
            var endTime = DateTime.Parse(time.Value.AddMonths(1).ToString("yyyy-MM")).AddDays(-1);// && x.Code == ThemeCode.Two

            EnteredPointReportModel model = new EnteredPointReportModel();
            model.StartTime = time.Value;
            model.EndDay = endTime.Day;

            model.List = new List<Tuple<string, string, Dictionary<int, int>, int>>();

            model.TotalDic = new Dictionary<int, int>();


            //学员报名记录集合
            var studenLtist = Cache_Get_StudentList().Where(x => x.EnteredDate >= time && x.EnteredDate <= endTime&&x.Flag== (long)GlobalFlag.Normal).ToList();

            //报名点集合
            var enteredPointIDList = new List<string>();
            if (!string.IsNullOrEmpty(EnteredPointID))
            {
                enteredPointIDList.Add(EnteredPointID);
            }
            else
            {
                if (this.Client.LoginUser.MenuFlag == -1)
                    enteredPointIDList = Cache_Get_EnteredPointList().Where(x => x.Flag == (long)GlobalFlag.Normal).Select(x => x.ID).ToList();
                else
                    enteredPointIDList = Cache_Get_EnteredPointList().Where(x => x.Flag == (long)GlobalFlag.Normal && Client.LoginUser.EnteredPointIDStr.Contains(x.ID)).Select(x => x.ID).ToList();
            }
            model.TotalDic = new Dictionary<int, int>();
            //月份天数
            var dayCount = (endTime - time.Value).Days+1;
            model.TotalDic.Add(0,0);
            //遍历报名点
            foreach (var item in enteredPointIDList)
            {
                //报名点
                var enteredModel = Cache_Get_EnteredPoint_Dic()[item];


                    //推荐人集合
                    var referenceList = Cache_Get_ReferenceList().Where(x => x.Flag == (long)GlobalFlag.Normal &&!string.IsNullOrEmpty(x.EnteredPointIDStr)&&x.EnteredPointIDStr.Contains(item)).ToList();
                    referenceList.ForEach(x => {
                        //日期集合
                        var dic = new Dictionary<int, int>();
                        dic.Add(0, 0);
                        //该推荐人总数
                        var totalCount = 0;
                        //根据日期groupby 
                        var referenceStudentDayDic = studenLtist.Where(y => y.EnteredPointID.Equals(item) && y.ReferenceID.Equals(x.ID)).GroupBy(y=>y.CreatedTime.Day).ToDictionary(y=>y.Key) ;
                        //遍历日期
                        for (var i = 1; i <= dayCount; i++)
                        {
                            //如果该推荐人当天有新报名学生
                            if (referenceStudentDayDic.ContainsKey(i))
                            {
                                //判断该天统计是否存在
                                if (model.TotalDic.ContainsKey(i))
                                {
                                    model.TotalDic[i] = model.TotalDic[i] + referenceStudentDayDic[i].Count();
                                }
                                else
                                    model.TotalDic.Add(i, referenceStudentDayDic[i].Count());
                                //总计人数
                                model.TotalCount += referenceStudentDayDic[i].Count();
                                //该推荐人总数
                                totalCount += referenceStudentDayDic[i].Count();
                                dic.Add(i, referenceStudentDayDic[i].Count());
                            }
                            else
                                dic.Add(i, 0);
                        }
                        //添加集合
                        model.List.Add(new Tuple<string, string, Dictionary<int, int>, int>(enteredModel.Name, x.Name, dic, totalCount));                        
                    });
       
            }
            return model;
        }
    }
}
