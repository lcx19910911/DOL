
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
        string oilCardKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "OilCard");

        /// <summary>
        /// 缓存
        /// </summary>
        /// <returns></returns>
        private List<OilCard> Cache_Get_OilCardList()
        {

            return CacheHelper.Get<List<OilCard>>(oilCardKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<OilCard> list = db.OilCard.ToList();
                    return list;
                }
            });
        }



        /// <summary>
        /// 缓存 dic
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, OilCard> Cache_Get_OilCardList_Dic()
        {
            return Cache_Get_OilCardList().ToDictionary(x => x.ID);
        }


        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<OilCard>> Get_OilCardPageList(int pageIndex, int pageSize, string companyName, string no,string coachId)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_OilCardList().AsQueryable().AsNoTracking().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0);
                if (companyName.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Company.Contains(companyName));
                }

                if (no.IsNotNullOrEmpty() )
                {
                    query = query.Where(x => x.CardNO.Contains(no));
                }
                if (coachId.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.CoachID.Equals(coachId));
                }
                var count = query.Count();
                var list = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var coachDic = Cache_Get_CoachList_Dic();
                var userDic = Cache_Get_UserDic();
                var starTime = DateTime.Now.Date.AddDays(-DateTime.Now.Date.Day + 1);
                var oilIdList = list.Select(x => x.ID).ToList();
                var oilDic = Cache_Get_WasteList().Where(x =>x.Code==WasteCode.Oil&&oilIdList.Contains(x.OilID) && x.CreatedTime > starTime).ToList().GroupBy(x => x.OilID).ToDictionary(x => x.Key, x => x.ToList());
                list.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.CoachID) && coachDic.ContainsKey(x.CoachID))
                        x.CoachName = coachDic[x.CoachID].Name;
                    if (!string.IsNullOrEmpty(x.CreatedUserID) && userDic.ContainsKey(x.CreatedUserID))
                        x.CreatedUserName = userDic[x.CreatedUserID].Name;
                    if (oilDic.ContainsKey(x.ID))
                        x.OilMonth = oilDic[x.ID].Sum(y => y.Money);
                });

                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }



        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_OilCard(OilCard model)
        {
            using (DbRepository entities = new DbRepository())
            {
                if (Cache_Get_OilCardList().Where(x => x.CardNO == model.CardNO && (x.Flag&(long)GlobalFlag.Removed)!=0).Any())
                    return Result(false, ErrorCode.datadatabase_name_had);
                model.ID = Guid.NewGuid().ToString("N");
                model.CreatedTime = DateTime.Now;
                model.Flag = (long)GlobalFlag.Normal;
                model.UpdatedTime = DateTime.Now;
                model.UpdaterID=model.CreatedUserID = Client.LoginUser.ID;
                entities.OilCard.Add(model);
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_OilCardList();
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
        public WebResult<bool> Update_OilCard(OilCard model)
        {
            using (DbRepository entities = new DbRepository())
            {
                if (Cache_Get_OilCardList().Where(x => x.CardNO == model.CardNO && (x.Flag & (long)GlobalFlag.Removed) != 0 && !model.ID.Equals(x.ID)).Any())
                    return Result(false, ErrorCode.datadatabase_name_had);
                var oldEntity = entities.OilCard.Find(model.ID);
                if (oldEntity != null)
                {
                    oldEntity.Company = model.Company;
                    oldEntity.CardNO = model.CardNO;
                    oldEntity.CoachID = model.CoachID;
                    oldEntity.CreatedUserID = model.CreatedUserID;
                    oldEntity.UpdatedTime = DateTime.Now;
                    oldEntity.UpdaterID = Client.LoginUser.ID;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_OilCardList();
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
        public OilCard Find_OilCard(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
                return Cache_Get_OilCardList().AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Delete_OilCard(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_OilCardList();
                //找到实体
                entities.OilCard.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
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

                var wastList = Cache_Get_WasteList();
                entities.Waste.Where(x => ids.Contains(x.TargetID)&&x.Code==WasteCode.Oil).ToList().ForEach(x =>
                {
                    x.Flag = x.Flag | (long)GlobalFlag.Removed;

                    var index = wastList.FindIndex(y => y.ID.Equals(x.ID));
                    if (index > -1)
                    {
                        wastList[index] = x;
                    }
                    else
                    {
                        wastList.Add(x);
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
        /// <param name="oilCardFlag">角色flag值</param>
        /// <returns></returns>
        public List<SelectItem> Get_OilCardSelectItem(string id)
        {
            List<SelectItem> list = new List<SelectItem>();

            Cache_Get_OilCardList().Where(x=>x.Flag==(long)GlobalFlag.Normal).AsQueryable().AsNoTracking().OrderBy(x => x.CreatedTime).ToList().ForEach(x =>
            {
                list.Add(new SelectItem()
                {
                    Selected = x.ID.Equals(id),
                    Text = x.CardNO,
                    Value = x.ID
                });
            });
            return list;
        }

        /// <summary>
        /// 获取ZTree节点
        /// </summary>
        /// <returns></returns>
        public List<ZTreeNode> Get_OilCardZTreeStr()
        {
            List<ZTreeNode> ztreeNodes = Cache_Get_OilCardList().Select(
                    x => new ZTreeNode()
                    {
                        name = x.Company,
                        value=x.ID
                    }).ToList();
            return ztreeNodes;
        }



        /// <summary>
        /// 修改责任人
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update_OilCardCoach(string id, string coachID)
        {
            if (!id.IsNotNullOrEmpty() || !coachID.IsNotNullOrEmpty()
                )
                return Result(false, ErrorCode.sys_param_format_error);
            using (DbRepository entities = new DbRepository())
            {

                var list = Cache_Get_OilCardList();
                var oldEntity = entities.OilCard.Find(id);
                if (oldEntity != null)
                {

                    oldEntity.CoachID = coachID;
                    oldEntity.UpdatedTime = DateTime.Now;
                    oldEntity.UpdaterID = Client.LoginUser.ID;
                    string afterJSon = oldEntity.ToJson();
                    // string info = SearchModifyHelper.CompareProperty<Student, Student>(Cache_Get_StudentList_Dic()[oldEntity.ID], oldEntity);
                    // Add_Log(LogCode.UpdateStudent, oldEntity.ID, string.Format("{0}在{1}编辑{2}的信息,分配教练", Client.LoginUser.Name, DateTime.Now.ToString(), oldEntity.Name), beforeJson, afterJSon, info);
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var index = list.FindIndex(x => x.ID.Equals(id));
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
    }
}
