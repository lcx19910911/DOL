
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
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="source">实体</param>
        /// <returns>影响条数</returns>
        public void Add(Dto.Admin.Role.Add source)
        {
            using (var db = new DbRepository())
            {
                var roleFlags = entities.Role.Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0).Select(x => x.RoleFlag ?? 0).ToList();
                var roleFlagAll = 0L;
                // 获取所有角色位值并集
                roleFlags.ForEach(x => roleFlagAll |= x);
                var roleFlag = 0L;
                // 从低位遍历是否为空
                for (var i = 0; i < 64; i++)
                {
                    if ((roleFlagAll & (1 << i)) == 0)
                    {
                        roleFlag = 1 << i;
                        break;
                    }
                }
                source.RoleFlag = roleFlag;
                base.Add<Dto.Admin.Role.Add, Role>(source);
            }
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <returns></returns>
        public PageList<Role> GetPageList(int pageIndex, int pageSize, string name)
        {
            using (var db = new DbRepository())
            {
                var query = entities.Role.AsQueryable();

                if (name != null)
                {
                    query = query.Where(x => x.Name.Contains(name));
                }
                return CreatePageList(query.OrderByDescending(x => x.CreatedTime), pageIndex, pageSize);
            }
        }

        /// <summary>
        /// 获取选择项
        /// </summary>
        /// <param name="roleFlag">角色flag值</param>
        /// <returns></returns>
        public List<SelectItem> GetSelectItem(long roleFlag)
        {
            return Helper.FlagHelper.GetSelectItem(roleFlag, typeof(Role), "RoleFlag", "Name");
        }
    }
}
