using DOL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Core
{
    /// <summary>
    /// 菜单
    /// </summary>
    public class StudentIndexModel
    {
        /// <summary>
        /// 报名点
        /// </summary>
        public List<EnteredPoint> EnteredPointList { get; set; }

        /// <summary>
        /// 推荐人
        /// </summary>
        public List<Reference> ReferenceList { get; set; }

        /// <summary>
        /// 驾校
        /// </summary>
        public List<DriverShop> DriverShopList { get; set; }

        /// <summary>
        /// 培训班别
        /// </summary>
        public List<SelectItem> TrainList { get; set; }

        /// <summary>
        /// 证书类型
        /// </summary>
        public List<SelectItem> CertificateList { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public List<SelectItem> PayMethodList { get; set; }

        /// <summary>
        /// 支付渠道
        /// </summary>
        public List<SelectItem> PayTypeList { get; set; }


        /// <summary>
        /// 收款账号
        /// </summary>
        public List<SelectItem> AccountList { get; set; }

        /// <summary>
        /// 教练员
        /// </summary>
        public List<Coach> CoachList { get; set; }


        /// <summary>
        /// 学校
        /// </summary>
        public List<SelectItem> SchoolList { get; set; }

        /// <summary>
        /// 学院
        /// </summary>
        public List<SelectItem> CollegeList { get; set; }


        /// <summary>
        /// 专业
        /// </summary>
        public List<SelectItem> MajorList { get; set; }

    }
}
