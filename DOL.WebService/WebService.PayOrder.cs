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
        string payOrderKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "PayOrder");

        /// <summary>
        /// 全局缓存
        /// </summary>
        /// <returns></returns>
        private List<PayOrder> Cache_Get_PayOrderList()
        {

            return CacheHelper.Get<List<PayOrder>>(payOrderKey, () =>
            {
                using (var db = new DbRepository())
                {
                    List<PayOrder> list = db.PayOrder.OrderByDescending(x => x.CreatedTime).ThenBy(x => x.ID).ToList();
                    return list;
                }
            });
        }

        /// <summary>
        /// 全局缓存 dic
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, PayOrder> Cache_Get_PayOrderList_Dic()
        {
            return Cache_Get_PayOrderList().ToDictionary(x => x.ID);
        }


        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PayOrder Find_PayOrder(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            return Cache_Get_PayOrderList().AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
        }



        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns></returns>
        public WebResult<PageList<PayOrder>> Get_PayOrderPageList(
            int pageIndex,
            int pageSize,
            string no,
            int state
            )
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_PayOrderList().AsQueryable().AsNoTracking().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0 && x.IsDrop == YesOrNoCode.No);


                if (no.IsNotNullOrEmpty())
                {
                    var studentIdList = Cache_Get_StudentList().Where(x => x.IDCard.Contains(no)).Select(x => x.ID).ToList();
                    query = query.Where(x => studentIdList.Contains(x.StudentID));
                }

                if (state != -1)
                {
                    query = query.Where(x => (int)x.IsConfirm == state);
                }

                var count = query.Count();
                var list = query.OrderByDescending(x => x.PayTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var studentDic = Cache_Get_StudentList_Dic();
                var referenceDic = Cache_Get_ReferenceList_Dic();
                var driverShopDic = Cache_Get_DriverShopList_Dic();
                var cerDic = Cache_Get_DataDictionary()[GroupCode.Certificate];
                var userDic = Cache_Get_UserDic();
                list.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.StudentID))
                    {
                        if (studentDic.ContainsKey(x.StudentID))
                        {
                            var student = studentDic[x.StudentID];
                            x.EnteredDate = student.EnteredDate;
                            x.MakeCardDate = student.MakeCardDate;
                            x.Mobile = student.Mobile;
                            x.Money = student.Money;
                            x.HadPayMoney = student.HadPayMoney;
                            x.Name = student.Name;
                            x.IDCard = student.IDCard;
                            //制卡驾校
                            if (!string.IsNullOrEmpty(student.MakeDriverShopID) && driverShopDic.ContainsKey(student.MakeDriverShopID))
                                x.MakeDriverShopName = driverShopDic[student.MakeDriverShopID]?.Name;

                            //推荐人
                            if (!string.IsNullOrEmpty(student.ReferenceID) && referenceDic.ContainsKey(student.ReferenceID))
                                x.ReferenceName = referenceDic[student.ReferenceID]?.Name;

                            //证书
                            if (!string.IsNullOrEmpty(student.CertificateID) && cerDic.ContainsKey(student.CertificateID))
                                x.CertificateName = cerDic[student.CertificateID]?.Value;
                        }
                    }
                    //登记人
                    if (!string.IsNullOrEmpty(x.CreaterID) && userDic.ContainsKey(x.CreaterID))
                        x.CreaterName = userDic[x.CreaterID]?.Name;
                    //确认人
                    if (!string.IsNullOrEmpty(x.ConfirmUserID) && userDic.ContainsKey(x.ConfirmUserID))
                        x.ConfirmUserName = userDic[x.ConfirmUserID]?.Name;

                });

                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }


        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns></returns>
        public WebResult<PageList<PayOrder>> Get_WantDropPayOrderPageList(
            int pageIndex,
            int pageSize,
            string no,
            int state
            )
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_PayOrderList().AsQueryable().AsNoTracking().Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0 && x.IsDrop == YesOrNoCode.Yes);


                    
                    
                 var studentQuery=Cache_Get_StudentList().Where(x => x.State == StudentCode.WantDropOut || x.State == StudentCode.HadDropOut);

                if (no.IsNotNullOrEmpty())
                {
                    studentQuery = studentQuery.Where(x =>x.IDCard.Contains(no));
                }
                var studentIdList = studentQuery.Select(x=>x.ID).ToList();

                query = query.Where(x => studentIdList.Contains(x.StudentID));

                if (state != -1)
                {
                    query = query.Where(x => (int)x.IsConfirm == state);
                }

                var count = query.Count();
                var list = query.OrderByDescending(x => x.PayTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var studentDic = Cache_Get_StudentList_Dic();
                var referenceDic = Cache_Get_ReferenceList_Dic();
                var enteredDic = Cache_Get_EnteredPoint_Dic();
                var cerDic = Cache_Get_DataDictionary()[GroupCode.Certificate];
                var userDic = Cache_Get_UserDic();
                list.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.StudentID))
                    {
                        if (studentDic.ContainsKey(x.StudentID))
                        {
                            var student = studentDic[x.StudentID];
                            x.EnteredDate = student.EnteredDate;
                            x.MakeCardDate = student.MakeCardDate;
                            x.Mobile = student.Mobile;
                            x.Money = student.Money;
                            x.HadPayMoney = student.HadPayMoney;
                            x.Name = student.Name;
                            x.IDCard = student.IDCard;

                            //推荐人
                            if (!string.IsNullOrEmpty(student.ReferenceID) && referenceDic.ContainsKey(student.ReferenceID))
                                x.ReferenceName = referenceDic[student.ReferenceID]?.Name;

                            //证书
                            if (!string.IsNullOrEmpty(student.CertificateID) && cerDic.ContainsKey(student.CertificateID))
                                x.CertificateName = cerDic[student.CertificateID]?.Value;
                            //报名点
                            if (!string.IsNullOrEmpty(student.EnteredPointID) && enteredDic.ContainsKey(student.EnteredPointID))
                                x.EnteredPointName = enteredDic[student.EnteredPointID]?.Name;
                        }
                    }
                    //登记人
                    if (!string.IsNullOrEmpty(x.CreaterID) && userDic.ContainsKey(x.CreaterID))
                        x.CreaterName = userDic[x.CreaterID]?.Name;
                    //确认人
                    if (!string.IsNullOrEmpty(x.ConfirmUserID) && userDic.ContainsKey(x.ConfirmUserID))
                        x.ConfirmUserName = userDic[x.ConfirmUserID]?.Name;

                });

                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }



        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_PayOrder(PayOrder model)
        {
            using (DbRepository entities = new DbRepository())
            {
                model.ID = Guid.NewGuid().ToString("N");
                model.UpdatedTime = DateTime.Now;
                model.CreatedTime = DateTime.Now;
                model.Flag = (long)GlobalFlag.Normal;
                model.IsConfirm = YesOrNoCode.No;
                model.IsDrop = YesOrNoCode.No;
                entities.PayOrder.Add(model);
                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_PayOrderList();
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
        /// 确认收款 学员状态为培训中
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Confirm_PayOrder(string ID)
        {
            if (!ID.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_PayOrderList();
                var studentList = Cache_Get_StudentList();
                var oldEntity = entities.PayOrder.Find(ID);

                if (oldEntity == null || oldEntity.IsConfirm == YesOrNoCode.Yes)
                    return Result(false, ErrorCode.sys_param_format_error);

                oldEntity.IsConfirm = YesOrNoCode.Yes;
                oldEntity.ConfirmUserID = Client.LoginUser.ID;
                oldEntity.ConfirmDate = DateTime.Now;

                var student = studentList.FirstOrDefault(x => x.ID.Equals(oldEntity.StudentID));

                if (student == null)
                    return Result(false, ErrorCode.sys_param_format_error);
                student.State = StudentCode.Training;
                list.AsQueryable().Where(x => x.StudentID.Equals(student.ID) && x.IsConfirm == YesOrNoCode.Yes).ToList().ForEach(x =>
                {
                    student.HadPayMoney += x.PayMoney;
                });
                student.HadPayMoney += oldEntity.PayMoney;

                if (student.HadPayMoney >= student.Money)
                {
                    student.MoneyIsFull = YesOrNoCode.Yes;
                }

                if (entities.SaveChanges() > 0)
                {

                    var index = list.FindIndex(x => x.ID.Equals(ID));
                    if (index > -1)
                    {
                        list[index] = oldEntity;
                    }
                    else
                    {
                        list.Add(oldEntity);
                    }
                    var studentIndex = studentList.FindIndex(x => x.ID.Equals(student.ID));
                    if (studentIndex > -1)
                    {
                        studentList[studentIndex] = student;
                    }
                    else
                    {
                        studentList.Add(student);
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
        /// 确认退款
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Confirm_DropPayOrder(PayOrder model)
        {
            if (!model.ID.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_PayOrderList();
                var studentList = Cache_Get_StudentList();
                var oldEntity = entities.PayOrder.Find(model.ID);

                if (oldEntity == null || oldEntity.IsConfirm == YesOrNoCode.Yes)
                    return Result(false, ErrorCode.sys_param_format_error);

                oldEntity.IsConfirm = YesOrNoCode.Yes;
                oldEntity.ConfirmUserID = Client.LoginUser.ID;
                oldEntity.ConfirmDate = DateTime.Now;
                oldEntity.PayMoney = model.PayMoney;
                oldEntity.PayTime = model.PayTime;
                oldEntity.AccountNO = model.AccountNO;
                oldEntity.VoucherNO = model.VoucherNO;
                oldEntity.VoucherThum = model.VoucherThum;
                oldEntity.PayTypeID = model.PayTypeID;

                var student = studentList.FirstOrDefault(x => x.ID.Equals(oldEntity.StudentID));

                if (student == null)
                    return Result(false, ErrorCode.sys_param_format_error);

                student.State = StudentCode.HadDropOut;
                student.DropOutDate = DateTime.Now;

                if (entities.SaveChanges() > 0)
                {

                    var index = list.FindIndex(x => x.ID.Equals(model.ID));
                    if (index > -1)
                    {
                        list[index] = oldEntity;
                    }
                    else
                    {
                        list.Add(oldEntity);
                    }
                    var studentIndex = studentList.FindIndex(x => x.ID.Equals(student.ID));
                    if (studentIndex > -1)
                    {
                        studentList[studentIndex] = student;
                    }
                    else
                    {
                        studentList.Add(student);
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
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update_PayOrder(PayOrder model)
        {
            using (DbRepository entities = new DbRepository())
            {
                var oldEntity = entities.PayOrder.Find(model.ID);
                if (oldEntity != null)
                {
                    oldEntity.PayMoney = model.PayMoney;
                    oldEntity.PayTime = model.PayTime;
                    oldEntity.PayTypeID = model.PayTypeID;
                    oldEntity.VoucherNO = model.VoucherNO; ;
                    oldEntity.VoucherThum = model.VoucherThum;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (entities.SaveChanges() > 0)
                {
                    var list = Cache_Get_PayOrderList();
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
        public WebResult<bool> Delete_PayOrder(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                var list = Cache_Get_PayOrderList();
                //找到实体
                entities.PayOrder.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    x.Flag = x.Flag | (long)GlobalFlag.Removed;
                    var index = list.FindIndex(y => y.ID.Equals(x.ID));
                    if (index > -1)
                    {
                        list[index] = x;
                    }
                    else
                    {
                        list.Add(list[index]);
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
        public WebResult<PageList<PayOrder>> Get_PayOrderByStudentId(int pageIndex,
            int pageSize, string studentId)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = Cache_Get_PayOrderList().AsQueryable().AsNoTracking().Where(x => x.StudentID.Equals(studentId) && x.Flag == (long)GlobalFlag.Normal&&x.IsDrop==YesOrNoCode.No).OrderBy(x => x.CreatedTime);
                var count = query.Count();
                var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var accountList = Get_DataDictorySelectItem(GroupCode.Account);
                var payTypeList = Get_DataDictorySelectItem(GroupCode.PayType);
                list.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.AccountID))
                        x.AccountName = accountList.FirstOrDefault(y => y.Value.Equals(x.AccountID))?.Text;
                    if (!string.IsNullOrEmpty(x.PayTypeID))
                        x.PayTypeName = payTypeList.FirstOrDefault(y => y.Value.Equals(x.PayTypeID))?.Text;
                });

                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }
    }
}
