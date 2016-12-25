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
                    List<Exam> list = db.Exam.OrderBy(x => x.Code).ThenBy(x => x.Count).ToList();
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
                var query = entities.Exam.AsQueryable().AsNoTracking();


                if (no.IsNotNullOrEmpty())
                {
                    var studentIdList = Cache_Get_StudentList().Where(x => x.IDCard.Contains(no)).Select(x => x.ID).ToList();
                    query = query.Where(x => studentIdList.Contains(x.StudentID));
                }
                var count = query.Count();
                var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

                if (Cache_Get_ExamList().Where(x => x.Code == model.Code && x.Result == ExamCode.Pass && x.StudentID.Equals(model.StudentID)).Any())
                    return Result(false, ErrorCode.theme_had_pass);
                if (Cache_Get_ExamList().Where(x => x.Code == model.Code && x.Count == model.Count && x.StudentID.Equals(model.StudentID)).Any())
                    return Result(false, ErrorCode.count_had_exit);
                if (Cache_Get_ExamList().Where(x => x.Code == model.Code && x.Count <= model.Count && x.StudentID.Equals(model.StudentID)).Count() !=(model.Count - 1))
                    return Result(false, ErrorCode.exam_had_lose);
                if (Cache_Get_ExamList().Where(x =>x.CreatedTime > model.CreatedTime && x.StudentID.Equals(model.StudentID)).Any())
                    return Result(false, ErrorCode.exam_timer_error);
                var studentList = Cache_Get_StudentList();
                var student = entities.Student.Find(model.StudentID);
                if (student.State == StudentCode.WantDropOut)
                {
                    return Result(false, ErrorCode.student_want_drop);
                }
                if (student == null)
                    return Result(false, ErrorCode.sys_param_format_error);

                if (model.Code == ThemeCode.One)
                {
                    if (model.CreatedTime < student.MakeCardDate)
                    {
                        return Result(false, ErrorCode.theme_one_time_error);
                    }
                    student.ThemeOneDate =model.CreatedTime;
                    if (model.Result == ExamCode.Pass)
                    {
                        student.ThemeOnePass = YesOrNoCode.Yes;
                        student.NowTheme = ThemeCode.Two;
                        student.State = StudentCode.ThemeTwo;
                    }
                }
                else if (model.Code == ThemeCode.Two)
                {
                    if (student.ThemeTwoCoachID.IsNullOrEmpty())
                    {
                        return Result(false, ErrorCode.exam_coache_not_exit);
                    }

                    if (student.ThemeTwoTimeCode!=ThemeTimeCode.Complete)
                    {
                        return Result(false, ErrorCode.themetwo_timecode_not_complete);
                    }


                    student.ThemeTwoDate = model.CreatedTime;
                    if (model.Result == ExamCode.Pass)
                    {
                        student.ThemeTwoPass = YesOrNoCode.Yes;
                        student.NowTheme = ThemeCode.Three;
                        student.ThemeTwoTimeCode = ThemeTimeCode.Complete;
                        student.State = StudentCode.ThemeThree;
                    }
                }
                else if (model.Code == ThemeCode.Three)
                {
                    if (student.ThemeThreeCoachID.IsNullOrEmpty())
                    {
                        return Result(false, ErrorCode.exam_coache_not_exit);
                    }
                    if (student.ThemeThreeTimeCode != ThemeTimeCode.Complete)
                    {
                        return Result(false, ErrorCode.themethree_timecode_not_complete);
                    }

                    student.ThemeThreeDate = model.CreatedTime;
                    if (model.Result == ExamCode.Pass)
                    {
                        student.ThemeThreePass = YesOrNoCode.Yes;
                        student.NowTheme = ThemeCode.Four;
                        student.State = StudentCode.ThemeFour;

                        student.ThemeThreeTimeCode = ThemeTimeCode.Complete;
                    }
                }
                else if (model.Code == ThemeCode.Four)
                {
                    student.ThemeFourDate = model.CreatedTime;
                    if (model.Result == ExamCode.Pass)
                    {
                        student.ThemeFourPass = YesOrNoCode.Yes;
                        student.NowTheme = ThemeCode.Four;
                        student.State = StudentCode.Graduate;
                    }
                }

                entities.Exam.Add(model);
                Add_Log(LogCode.AddExam, student.ID, string.Format("{0}在{1}新增了学员{2}的考试{3}", Client.LoginUser.Name, DateTime.Now.ToString(), student.Name, model.ID), "", "", "");
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_ExamList();
                    list.Add(model);
                    var index = studentList.FindIndex(y => y.ID.Equals(model.StudentID));
                    if (index > -1)
                    {
                        studentList[index] = student;
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
                ErrorCode msg = ErrorCode.sys_success;
                //找到实体
                entities.Exam.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    var student = entities.Student.Find(x.StudentID);
                    if (student.State == StudentCode.WantDropOut)
                    {
                        msg=ErrorCode.student_want_drop;
                    }
                    if (student != null)
                    {
                        if (x.Result == ExamCode.Pass)
                        {
                            if (x.Code == ThemeCode.One)
                            {
                                student.ThemeOnePass = YesOrNoCode.No;
                                student.ThemeOneDate = null;
                                student.State = StudentCode.ThemeOne;
                                student.NowTheme = ThemeCode.One;
                            }
                            else if (x.Code == ThemeCode.Two)
                            {
                                student.ThemeTwoPass = YesOrNoCode.No;
                                student.ThemeTwoDate = null;
                                student.State = StudentCode.ThemeTwo;
                                student.ThemeTwoTimeCode = ThemeTimeCode.Lock;
                                student.NowTheme = ThemeCode.Two;
                            }
                            else if (x.Code == ThemeCode.Three)
                            {
                                student.ThemeThreePass = YesOrNoCode.No;
                                student.ThemeThreeDate = null;
                                student.State = StudentCode.ThemeThree;
                                student.ThemeThreeTimeCode = ThemeTimeCode.Lock;
                                student.NowTheme = ThemeCode.Three;
                            }
                            else if (x.Code == ThemeCode.Four)
                            {
                                student.ThemeFourPass = YesOrNoCode.No;
                                student.ThemeFourDate = null;
                                student.State = StudentCode.ThemeFour;
                                student.NowTheme = ThemeCode.Four;
                            }
                        }
                        Add_Log(LogCode.DeleteExam, student.ID, string.Format("{0}在{1}删除了学员{2}的考试（{3}）考试时间{4}", Client.LoginUser.Name, DateTime.Now.ToString(), student.Name, x.ID, x.CreatedTime), "", "", "");
                        entities.Exam.Remove(x);
                        var index = list.FindIndex(y => y.ID.Equals(x.ID));
                        var studentIndex = studentList.FindIndex(y => y.ID.Equals(x.StudentID));
                        if (index > -1&& studentIndex>-1)
                        {
                            list.RemoveAt(index);
                            studentList[studentIndex] = student;
                        }
                    }
                });
                if (msg != ErrorCode.sys_success)
                {
                    return Result(false, msg);
                }
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
                var query = entities.Exam.AsQueryable().AsNoTracking().Where(x => x.StudentID.Equals(studentId)).OrderBy(x => x.Code);
                var count = query.Count();
                var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var coachDic = Cache_Get_CoachList_Dic();
                var student = Cache_Get_StudentList_Dic()[studentId];
                string themeTwoCoachName = string.Empty;
                string themeThreeCoachName = string.Empty;
                if (student.ThemeTwoCoachID.IsNotNullOrEmpty() && coachDic.ContainsKey(student.ThemeTwoCoachID))
                {
                    themeTwoCoachName = coachDic[student.ThemeTwoCoachID].Name;
                }
                if (student.ThemeThreeCoachID.IsNotNullOrEmpty() && coachDic.ContainsKey(student.ThemeThreeCoachID))
                {
                    themeThreeCoachName = coachDic[student.ThemeThreeCoachID].Name;
                }
                list.ForEach(x =>
                {
                    x.ThemeTwoCoachName = themeTwoCoachName;
                    x.themeThreeCoachName = themeThreeCoachName;
                });
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }
    }
}
