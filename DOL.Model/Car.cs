using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Model
{
    [Table("Car")]
    public class Car : BaseEntity
    {
        /// <summary>
        /// 车型
        /// </summary>
        [MaxLength(32)]
        [Column("Models", TypeName = "varchar")]
        public string Models { get; set; }
        /// <summary>
        /// 车型代码
        /// </summary>
        [MaxLength(32)]
        [Column("ModelsCode", TypeName = "varchar")]
        public string ModelsCode { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        [MaxLength(32)]
        [Column("Brand", TypeName = "varchar")]
        public string Brand { get; set; }

        /// <summary>
        /// 车牌
        /// </summary>
        [MaxLength(32)]
        [Column("License", TypeName = "varchar")]
        public string License { get; set; }


        /// <summary>
        /// 发动机号
        /// </summary>
        [MaxLength(32)]
        [Column("EngineNumber", TypeName = "varchar")]
        public string EngineNumber { get; set; }

        /// <summary>
        /// 车架代码
        /// </summary>
        [MaxLength(32)]
        [Column("FrameCode", TypeName = "varchar")]
        public string FrameCode { get; set; }
        



        /// <summary>
        /// 责任人
        /// </summary>
        [MaxLength(32)]
        [Column("CoachID", TypeName = "char")]
        public string CoachID { get; set; }

        [NotMapped]
        public string CoachName { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        [Required(ErrorMessage = "部门ID不能为空")]
        [Column("DepartmentID", TypeName = "char"), MaxLength(32)]
        public string DepartmentID { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [NotMapped]
        public string DepartmentName { get; set; }


        /// <summary>
        /// 购置日期
        /// </summary>
        [Display(Name = "购置日期")]
        [Required]
        public System.DateTime BuyTime { get; set; }


        /// <summary>
        /// 上牌日期
        /// </summary>
        [Display(Name = "上牌日期")]
        [Required]
        public System.DateTime? OnCardTime { get; set; }

        [NotMapped]
        public decimal RepairMonth { get; set; }


        [NotMapped]
        public decimal OilMonth { get; set; }
    }
}
