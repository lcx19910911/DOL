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
        string studentKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "Student");

        /// <summary>
        /// 站内通知全局缓存
        /// </summary>
        /// <returns></returns>
        private List<Student> Cache_Get_StudentList()
        {

            return CacheHelper.Get<List<Student>>(studentKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<Student> list = db.Student.Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0).OrderByDescending(x => x.CreatedTime).ThenBy(x => x.ID).ToList();
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
        public WebResult<PageList<Student>> Get_StudentPageList(
            int pageIndex,
            int pageSize, 
            string name,
            string referenceId,
            string no,
            string mobile,
            string enteredPointId,
            string makeDriverShopId,
            DateTime? enteredTimeStart, DateTime? enteredTimeEnd,
            DateTime? makedTimeStart, DateTime? makeTimeEnd
            )
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_StudentList().AsQueryable().AsNoTracking().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0);
                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }
                if (no.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.IDCard.Contains(no));
                }
                if (mobile.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Mobile.Contains(mobile));
                }

                if (referenceId.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.ReferenceID.Equals(referenceId));
                }

                if (enteredPointId.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.EnteredPointID.Equals(enteredPointId));
                }
                if (makeDriverShopId.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.MakeDriverShopID.Equals(makeDriverShopId));
                }
                if (enteredTimeStart != null)
                {
                    query = query.Where(x => x.EnteredDate >= enteredTimeStart);
                }
                if (enteredTimeEnd != null)
                {
                    enteredTimeEnd = enteredTimeEnd.Value.AddDays(1);
                    query = query.Where(x => x.EnteredDate < enteredTimeEnd);
                }

                if (makedTimeStart != null)
                {
                    query = query.Where(x => x.MakeCardDate >= makedTimeStart);
                }
                if (makeTimeEnd != null)
                {
                    makeTimeEnd = makeTimeEnd.Value.AddDays(1);
                    query = query.Where(x => x.MakeCardDate < makeTimeEnd);
                }
                var count = query.Count();
                var list = query.OrderByDescending(x => x.EnteredDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var referenceDic = Cache_Get_ReferenceList_Dic();
                var driverShopDic = Cache_Get_DriverShopList_Dic();
                var areaDic = Cache_Get_DataDictionary()[GroupCode.Area];
                var enteredPointDic = Cache_Get_EnteredPoint_Dic();
                var cerDic = Cache_Get_DataDictionary()[GroupCode.Certificate];
                var payMethodDic = Cache_Get_DataDictionary()[GroupCode.PayMethod];
                var trianDic = Cache_Get_DataDictionary()[GroupCode.Train];
                list.ForEach(x =>
                {
                    //报名地
                    if (!string.IsNullOrEmpty(x.EnteredCityCode))
                        x.EnteredCityName = areaDic[x.EnteredCityCode]?.Value;
                    //制卡地
                    if (!string.IsNullOrEmpty(x.MakeCardCityCode))
                        x.MakeCardCityName = areaDic[x.MakeCardCityCode]?.Value;
                    //培训方式
                    if (!string.IsNullOrEmpty(x.TrianID))
                        x.TrianName = trianDic[x.TrianID]?.Value;
                    //制卡驾校
                    if (!string.IsNullOrEmpty(x.MakeDriverShopID))
                        x.MakeDriverShopName = driverShopDic[x.TrianID]?.Name;

                    //推荐人
                    if (!string.IsNullOrEmpty(x.ReferenceID))
                        x.ReferenceName = referenceDic[x.ReferenceID]?.Name;
                    //支付方式
                    if (!string.IsNullOrEmpty(x.PayMethodID))
                        x.PayMethodName = payMethodDic[x.PayMethodID]?.Value;

                    //省
                    if (!string.IsNullOrEmpty(x.ProvinceCode))
                        x.ProvinceName = areaDic[x.ProvinceCode]?.Value;
                    //省
                    if (!string.IsNullOrEmpty(x.CityCode))
                        x.CityName = areaDic[x.CityCode]?.Value;

                    //证书
                    if (!string.IsNullOrEmpty(x.CertificateID))
                        x.CertificateName = cerDic[x.CertificateID]?.Value;

                });

                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_Student(Student model)
        {
            if (model == null
                || !model.Name.IsNotNullOrEmpty()
                )
                return Result(false, ErrorCode.sys_param_format_error);
            using (DbRepository entities = new DbRepository())
            {
                if (entities.Student.AsNoTracking().Where(x => x.Name.Equals(model.Name)).Any())
                    return Result(false, ErrorCode.datadatabase_name_had);
                model.ID = Guid.NewGuid().ToString("N");
                model.CreatedTime = DateTime.Now;
                model.UpdatedTime = DateTime.Now;
                model.Flag = (long)GlobalFlag.Normal;
                entities.Student.Add(model);
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_StudentList();
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
        /// 获取搜索集合
        /// </summary>
        /// <returns></returns>
        public WebResult<StudentIndexModel> Get_SelectItemList()
        {
            var referenceList = Cache_Get_ReferenceList();
            var driverShopList = Cache_Get_DriverShopList();
            var enteredPointList = Cache_Get_EnteredPointList();

            return Result(new StudentIndexModel()
            {
                ReferenceList = referenceList,
                DriverShopList = driverShopList,
                EnteredPointList = enteredPointList
            });
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update_Student(Student model)
        {
            if (model == null
                 || !model.Name.IsNotNullOrEmpty()
                )
                return Result(false, ErrorCode.sys_param_format_error);
            using (DbRepository entities = new DbRepository())
            {
                var oldEntity = entities.Student.Find(model.ID);
                if (oldEntity != null)
                {
                    if (entities.Student.AsNoTracking().Where(x => x.Name.Equals(model.Name) && !x.ID.Equals(model.ID)).Any())
                        return Result(false, ErrorCode.datadatabase_name_had);
                    oldEntity.Name = model.Name;
                    oldEntity.UpdatedTime = DateTime.Now;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_StudentList();
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
        public WebResult<bool> Delete_Student(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_StudentList();
                //找到实体
                entities.Student.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
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
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Student Find_Student(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            using (DbRepository entities = new DbRepository())
            {
                return Cache_Get_StudentList().FirstOrDefault(x => x.ID.Equals(id));
            }
        }


        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="ids">id，多个id用逗号分隔</param>
        /// <returns>影响条数</returns>
        public WebResult<bool> Enable_Student(string ids)
        {
            if (string.IsNullOrEmpty(ids))
                return Result(false, ErrorCode.sys_param_format_error);
            using (DbRepository entities = new DbRepository())
            {
                //按逗号分隔符分隔开得到unid列表
                var unidArray = ids.Split(',');
                var list = Cache_Get_StudentList();
                entities.Student.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
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
        /// 禁用
        /// </summary>
        /// <param name="ids">ids，多个id用逗号分隔</param>
        /// <returns>影响条数</returns>
        public WebResult<bool> Disable_Student(string ids)
        {
            if (string.IsNullOrEmpty(ids))
                return Result(false, ErrorCode.sys_param_format_error);
            using (DbRepository entities = new DbRepository())
            {
                //按逗号分隔符分隔开得到unid列表
                var unidArray = ids.Split(',');
                var list = Cache_Get_StudentList();
                entities.Student.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
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
