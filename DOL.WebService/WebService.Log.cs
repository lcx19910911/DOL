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
        string logKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "Log");

        /// <summary>
        /// 全局缓存
        /// </summary>
        /// <returns></returns>
        private List<Log> Cache_Get_LogList()
        {

            return CacheHelper.Get<List<Log>>(logKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<Log> list = db.Log.OrderByDescending(x => x.CreatedTime).ThenBy(x => x.ID).ToList();
                    return list;
                }
            });
        }

        /// <summary>
        /// 全局缓存 dic
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Log> Cache_Get_LogList_Dic()
        {
            return Cache_Get_LogList().ToDictionary(x => x.ID);
        }


        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Log Find_Log(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            return Cache_Get_LogList().AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
        }

      /// <summary>
      /// 增加日志
      /// </summary>
      /// <param name="code">日志编码</param>
      /// <param name="studentId">学员Id</param>
      /// <param name="remark">备注</param>
      /// <param name="beforeJson"></param>
      /// <param name="afterJson"></param>
        public void Add_Log(LogCode code,string studentId,string remark,string beforeJson,string afterJson)
        {
            using (DbRepository entities = new DbRepository())
            {
                var model = new Log();
                model.ID = Guid.NewGuid().ToString("N");

                    model.CreatedTime = DateTime.Now;
                model.CreaterID = Client.LoginUser.ID;
                model.Remark = remark;
                model.Code = code;
                model.BeforeJson = beforeJson;
                model.AfterJson = afterJson;
                model.StudentID = studentId;
          
                entities.Log.Add(model);
                           
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_LogList();
                    list.Add(model);
                }
            }

        }
     
        /// 获取学员缴费记录
        /// </summary>
        /// <param name="">门店id</param>
        /// <returns></returns>
        public WebResult<PageList<Log>> Get_LogByStudentId(int pageIndex,
            int pageSize, string studentId)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_LogList().AsQueryable().AsNoTracking().Where(x => x.StudentID.Equals(studentId)).OrderByDescending(x => x.CreatedTime);
                var count = query.Count();
                var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }
    }
}
