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
        string examKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "Exam");

        /// <summary>
        /// 全局缓存
        /// </summary>
        /// <returns></returns>
        private List<Exam> Cache_Get_ExamList()
        {

            return CacheHelper.Get<List<Exam>>(examKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<Exam> list = db.Exam.OrderByDescending(x => x.CreatedTime).ThenBy(x => x.ID).ToList();
                    return list;
                }
            });
        }

        /// <summary>
        /// 全局缓存 dic
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Exam> Cache_Get_ExamList_Dic()
        {
            return Cache_Get_ExamList().ToDictionary(x => x.ID);
        }


        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Exam Find_Exam(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            return Cache_Get_ExamList().AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns></returns>
        public WebResult<PageList<Exam>> Get_ExamPageList(
            int pageIndex,
            int pageSize,
            string no,
            int state
            )
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_ExamList().AsQueryable().AsNoTracking();


                if (no.IsNotNullOrEmpty())
                {
                    var studentIdList = Cache_Get_StudentList().Where(x => x.IDCard.Contains(no)).Select(x => x.ID).ToList();
                    query = query.Where(x => studentIdList.Contains(x.StudentID));
                }
                var count = query.Count();
                var list = query.OrderByDescending(x => x.Code).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }


        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_Exam(Exam model)
        {
            using (DbRepository entities = new DbRepository())
            {
                model.ID = Guid.NewGuid().ToString("N");

                entities.Exam.Add(model);
                if(Cache_Get_ExamList().Where(x=>x.Code==model.Code&&x.Result==ExamCode.Pass&&x.StudentID.Equals(model.StudentID)).Any())
                    return Result(false, ErrorCode.theme_had_pass);
                if (Cache_Get_ExamList().Where(x => x.Code == model.Code && x.Count == model.Count && x.StudentID.Equals(model.StudentID)).Any())
                    return Result(false, ErrorCode.count_had_exit);
                var studentList = Cache_Get_StudentList();
                var student = entities.Student.Find(model.StudentID);
                if (student==null)
                    return Result(false, ErrorCode.sys_param_format_error);
                if (model.Code == ThemeCode.One&&model.Result==ExamCode.Pass)
                {
                    student.ThemeOnePass = YesOrNoCode.Yes;
                    student.NowTheme = ThemeCode.Two;
                }
                else if (model.Code == ThemeCode.Two && model.Result == ExamCode.Pass)
                {
                    student.ThemeTwoPass = YesOrNoCode.Yes;
                    student.NowTheme = ThemeCode.Three;
                }
                else if (model.Code == ThemeCode.Three && model.Result == ExamCode.Pass)
                {
                    student.ThemeThreePass = YesOrNoCode.Yes;
                    student.NowTheme = ThemeCode.Four;
                }
                else if (model.Code == ThemeCode.Four && model.Result == ExamCode.Pass)
                {
                    student.ThemeFourPass = YesOrNoCode.Yes;
                    student.NowTheme = ThemeCode.Four;
                }
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_ExamList();
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
        /// 删除考试记录
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Delete_Exam(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_ExamList();
                var studentList = Cache_Get_StudentList();
                //找到实体
                entities.Exam.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    var student = entities.Student.Find(x.StudentID);
                    if (student!=null)
                    {
                        if (x.Result == ExamCode.Pass)
                        {
                            if (x.Code == ThemeCode.One)
                            {
                                student.ThemeOnePass = YesOrNoCode.No;
                            }
                            else if (x.Code == ThemeCode.Two)
                            {
                                student.ThemeTwoPass = YesOrNoCode.No;
                            }
                            else if (x.Code == ThemeCode.Three)
                            {
                                student.ThemeThreePass = YesOrNoCode.No;
                            }
                            else if (x.Code == ThemeCode.Four)
                            {
                                student.ThemeFourPass = YesOrNoCode.No;
                            }
                        }
                        entities.Exam.Remove(x);
                        var index = list.FindIndex(y => y.ID.Equals(x.ID));
                        var studentIndex = studentList.FindIndex(y => y.ID.Equals(x.StudentID));
                        if (index > -1)
                        {
                            studentList.RemoveAt(studentIndex);
                            list.RemoveAt(index);
                        }
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
        /// 获取学员缴费记录
        /// </summary>
        /// <param name="">门店id</param>
        /// <returns></returns>
        public WebResult<PageList<Exam>> Get_ExamByStudentId(int pageIndex,
            int pageSize, string studentId)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_ExamList().AsQueryable().AsNoTracking().Where(x => x.StudentID.Equals(studentId)).OrderBy(x => x.Code);
                var count = query.Count();
                var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }
    }
}
