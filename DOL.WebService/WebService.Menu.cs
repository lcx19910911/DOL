using Core;
using Model;
using Repository;
using Service.Admin.Impl.Base;
using Service.Admin.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extension;
using Enum.Logic;
using Dto.Common;

namespace DOL.Service
{
    public partial class WebService
    {

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="source">实体</param>
        /// <returns>影响条数</returns>
        public void Add(Dto.Admin.Menu.Add source)
        {
            using (var db = new DbRepository())
            {
                var limitFlags = entities.Menu.Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0).Select(x => x.LimitFlag ?? 0).ToList();
                var limitFlagAll = 0L;
                // 获取所有角色位值并集
                limitFlags.ForEach(x => limitFlagAll |= x);
                var limitFlag = 0L;
                // 从低位遍历是否为空
                for (var i = 0; i < 64; i++)
                {
                    if ((limitFlagAll & (1 << i)) == 0)
                    {
                        limitFlag = 1 << i;
                        break;
                    }
                }
                source.LimitFlag = limitFlag;
                base.Add<Dto.Admin.Menu.Add, Menu>(source);
            }
        }


        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <returns></returns>
        public PageList<Menu> GetPageList(int pageIndex, int pageSize, string name)
        {
            using (var db = new DbRepository())
            {
                var query = entities.Menu.AsQueryable();

                if (name != null)
                {
                    query = query.Where(x => x.Name.Contains(name));
                }
                return CreatePageList(query.OrderByDescending(x => x.CreatedTime), pageIndex, pageSize);
            }
        }

        /// <summary>
        /// 获取ZTree子节点
        /// </summary>
        /// <param name="parentId">父级id</param>
        /// <param name="groups">分组数据</param>
        /// <returns></returns>
        public List<ZTreeNode> GetZTreeChildren(string parentId, List<IGrouping<string, Menu>> groups)
        {
            List<ZTreeNode> ztreeNodes = new List<ZTreeNode>();
            var group = groups.FirstOrDefault(x => x.Key == parentId);
            if (group != null)
            {
                ztreeNodes = group.Select(
                    x => new ZTreeNode()
                    {
                        name = x.Name,
                        value = x.UNID,
                        children = GetZTreeChildren(x.UNID, groups)
                    }).ToList();
            }
            return ztreeNodes;
        }

        /// <summary>
        /// 获取ZTree子节点
        /// </summary>
        /// <param name="parentId">父级id</param>
        /// <param name="groups">分组数据</param>
        /// <returns></returns>
        public List<ZTreeNode> GetZTreeOperateChildren(string parentId, List<IGrouping<string, Menu>> groups, List<Operate> allOperateList, List<Operate> roleToOperateList)
        {
            List<ZTreeNode> ztreeNodes = new List<ZTreeNode>();
            var group = groups.FirstOrDefault(x => x.Key == parentId);
            if (group != null)
            {
                ztreeNodes = group.Select(
                    x => new ZTreeNode()
                    {
                        name = x.Name,
                        value = x.UNID,
                        children = GetZTreeOperateChildren(x.UNID, groups, allOperateList, roleToOperateList),
                        nocheck = true
                    }).ToList();
            }
            else
            {
                ztreeNodes = allOperateList.Where(x => x.MenuID == parentId).Select(
                    x => new ZTreeNode()
                    {
                        name = x.Name,
                        value = x.UNID,
                        ischeck = roleToOperateList.Select(item => item.UNID).Contains(x.UNID)
                    }
                    ).ToList();
            }
            return ztreeNodes;
        }

        /// <summary>
        /// 获取ZTree子节点
        /// </summary>
        /// <param name="parentId">父级id</param>
        /// <param name="groups">分组数据</param>
        /// <returns></returns>
        public List<ZTreeNode> GetZTreeFlagChildren(string parentId, List<IGrouping<string, Menu>> groups)
        {
            List<ZTreeNode> ztreeNodes = new List<ZTreeNode>();
            var group = groups.FirstOrDefault(x => x.Key == parentId);
            if (group != null)
            {
                ztreeNodes = group.Select(
                    x => new ZTreeNode()
                    {
                        name = x.Name,
                        value = x.LimitFlag.ToString(),
                        children = GetZTreeFlagChildren(x.UNID, groups)
                    }).ToList();
            }
            return ztreeNodes;
        }
    }
}
