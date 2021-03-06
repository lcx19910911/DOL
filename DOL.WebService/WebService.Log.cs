﻿using DOL.Core;
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

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Log Find_Log(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            using (DbRepository entities = new DbRepository())
            {
                return entities.Log.Find(id);
            }
        }

      /// <summary>
      /// 增加日志
      /// </summary>
      /// <param name="code">日志编码</param>
      /// <param name="studentId">学员Id</param>
      /// <param name="remark">备注</param>
      /// <param name="beforeJson"></param>
      /// <param name="afterJson"></param>
        public void Add_Log(LogCode code,string studentId,string remark,string beforeJson,string afterJson,string info)
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
                model.UpdateInfo = info;
                entities.Log.Add(model);

                entities.SaveChanges();
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
                var query = entities.Log.AsQueryable().AsNoTracking().Where(x => x.StudentID.Equals(studentId)).OrderByDescending(x => x.CreatedTime);
                var count = query.Count();
                var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }
    }
}
