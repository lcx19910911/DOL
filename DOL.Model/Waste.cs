
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Model
{
    /// <summary>
    /// 耗损
    /// </summary>
    [Table("Waste")]
    public class Waste : BaseEntity
    {

        /// <summary>
        /// 教练员
        /// </summary>
        [Required(ErrorMessage = "教练员不能为空")]
        [MaxLength(32)]
        [Column("CoachID", TypeName = "char")]
        public string CoachID { get; set; }

        [NotMapped]
        public string CoachName { get; set; }

        /// <summary>
        /// 消费类别
        /// </summary>
        public WasteCode Code { get; set; }

        /// <summary>
        /// 车辆id
        /// </summary>
        [MaxLength(32)]
        [Column("CarID", TypeName = "char")]
        public string CarID { get; set; }
        [NotMapped]
        public string CarName { get; set; }

        /// <summary>
        /// 油卡id
        /// </summary>
        [MaxLength(32)]
        [Column("OilID", TypeName = "char")]
        public string OilID { get; set; }

        [NotMapped]
        public string OilName { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Money { get; set; }


        /// <summary>
        /// 耗损项目
        /// </summary>
        [Required(ErrorMessage = "耗损项目不能为空")]
        [MaxLength(32)]
        [Column("ThingID", TypeName = "char")]
        public string ThingID { get; set; }

        [NotMapped]
        public string ThingName { get; set; }


        /// <summary>
        /// 目标id
        /// </summary>
        [Required(ErrorMessage = "目标id不能为空")]
        [MaxLength(32)]
        [Column("TargetID", TypeName = "char")]
        public string TargetID { get; set; }

        [NotMapped]
        public string TargetName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(128)]
        [Column("Remark", TypeName = "varchar")]
        public string Remark { get; set; }
    }
}
