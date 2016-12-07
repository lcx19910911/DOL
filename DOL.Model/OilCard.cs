using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Model
{
    [Table("OilCard")]
    public class OilCard:BaseEntity
    {
        /// <summary>
        /// 所属公司
        /// </summary>
        [Required(ErrorMessage = "所属公司不能为空")]
        [MaxLength(32)]
        [Column("Company", TypeName = "varchar")]
        public string Company { get; set; }


        /// <summary>
        /// 余额
        /// </summary>
        public decimal Balance { get; set; }


        /// <summary>
        /// 金额
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        [Required(ErrorMessage = "卡号不能为空")]
        [MaxLength(32)]
        [Column("CardNO", TypeName = "varchar")]
        public string CardNO { get; set; }

        /// <summary>
        /// 办卡人
        /// </summary>
        [Required(ErrorMessage = "办卡人不能为空")]
        [MaxLength(32)]
        [Column("CreatedUserID", TypeName = "char")]
        public string CreatedUserID { get; set; }
        [NotMapped]
        public string CreatedUserName { get; set; }
        [NotMapped]
        public decimal OilMonth { get; set; }

        /// <summary>
        /// 教练人
        /// </summary>
        [MaxLength(32)]
        [Column("CoachID", TypeName = "char")]
        public string CoachID { get; set; }

        [NotMapped]
        public string CoachName { get; set; }
        
    }
}
