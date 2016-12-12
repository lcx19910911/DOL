using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOL.Model;
using DOL.Core;
using DOL.Repository;
using System.Data.Entity;

namespace DOL.Repository
{
    public class CacheModel
    {

        /// <summary>
        /// 全局缓存
        /// </summary>
        /// <returns></returns>
        public static List<TEntity> GetList<TEntity>() where TEntity : PKClass
        {
            string key = CacheHelper.RenderKey(Params.Cache_Prefix_Key, typeof(TEntity).Name);

            return CacheHelper.Get<List<TEntity>>(key, () =>
            {
                using (var db = new DbRepository())
                {
                    DbSet<TEntity> dbSet = db.Set<TEntity>();

                    return dbSet.ToList();
                }
            });
        }

        /// <summary>
        /// 全局缓存 dic
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, TEntity> GetDic<TEntity>() where TEntity : PKClass
        {
            return GetList<TEntity>().ToDictionary(x => x.ID);
        }
    }
}
