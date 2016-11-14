
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
        string driverShopKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "DriverShop");

        /// <summary>
        /// 缓存
        /// </summary>
        /// <returns></returns>
        private List<DriverShop> Cache_Get_DriverShopList()
        {

            return CacheHelper.Get<List<DriverShop>>(driverShopKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<DriverShop> list = db.DriverShop.ToList();
                    return list;
                }
            });
        }

        /// <summary>
        /// 缓存 dic
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, DriverShop> Cache_Get_DriverShopList_Dic()
        {
            return Cache_Get_DriverShopList().ToDictionary(x => x.ID);
        }


        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<DriverShop>> Get_DriverShopPageList(int pageIndex, int pageSize, string name,string provinceCode,string cityCode, string districtCode, string no)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_DriverShopList().AsQueryable().AsNoTracking().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0);
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
        public WebResult<bool> Add_DriverShop(DriverShop model)
        {
            using (DbRepository entities = new DbRepository())
            {
                model.ID = Guid.NewGuid().ToString("N");
                model.CreatedTime = DateTime.Now;
                model.Flag = (long)GlobalFlag.Normal;
                model.UpdatedTime = DateTime.Now;
                model.UpdaterID = Client.LoginUser.ID;
                entities.DriverShop.Add(model);
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_DriverShopList();
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
        public WebResult<bool> Update_DriverShop(DriverShop model)
        {
            using (DbRepository entities = new DbRepository())
            {
                var oldEntity = entities.DriverShop.Find(model.ID);
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
                    var list = Cache_Get_DriverShopList();
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
        public DriverShop Find_DriverShop(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
                return Cache_Get_DriverShopList().AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Delete_DriverShop(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_DriverShopList();
                //找到实体
                entities.DriverShop.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
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
        /// <param name="driverShopFlag">角色flag值</param>
        /// <returns></returns>
        public List<SelectItem> Get_DriverShopSelectItem(string id)
        {
            List<SelectItem> list = new List<SelectItem>();

            Cache_Get_DriverShopList().AsQueryable().AsNoTracking().OrderBy(x => x.CreatedTime).ToList().ForEach(x =>
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

        /// <summary>
        /// 获取ZTree节点
        /// </summary>
        /// <returns></returns>
        public List<ZTreeNode> Get_DriverShopZTreeStr()
        {
            List<ZTreeNode> ztreeNodes = Cache_Get_DriverShopList().Select(
                    x => new ZTreeNode()
                    {
                        name = x.Name,
                        value=x.ID
                    }).ToList();
            return ztreeNodes;
        }

        public DriverShop Get_DriverShopByName(string name)
        {
            return Cache_Get_DriverShopList().Where(x => x.Name.Equals(name)).FirstOrDefault();
        }
    }
}
