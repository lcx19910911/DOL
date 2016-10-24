
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
    /// 驾校
    /// </summary>
    [Table("DriverShop")]
    public class DriverShop : BaseEntity
    {
        /// <summary>
        /// 驾校名称
        /// </summary>
        [Required(ErrorMessage = "驾校名称不能为空")]
        [MaxLength(32)]
        [Display(Name= "驾校名称")]
        [Column("Name", TypeName = "varchar")]
        public string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? Sort { get; set; }

        /// <summary>
        /// 省份编码
        /// </summary>
        [MaxLength(6)]
        [Column("ProvinceCode", TypeName = "varchar")]
        public string ProvinceCode { get; set; }

        /// <summary>
        /// 城市编码
        /// </summary>
        [MaxLength(6)]
        [Column("CityCode", TypeName = "varchar")]
        public string CityCode { get; set; }

        /// <summary>
        /// 县区编码
        /// </summary>
        [MaxLength(6)]
        [Column("DistrictCode", TypeName = "varchar")]
        public string DistrictCode { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [MaxLength(128)]
        [Display(Name = "地址")]
        [Column("Address", TypeName = "varchar")]
        public string Address { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        [MaxLength(32)]
        [Display(Name = "联系人")]
        [Column("ConnactPeople", TypeName = "varchar")]
        public string ConnactPeople { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Display(Name = "手机号")]
        [MaxLength(11)]
        [RegularExpression(@"((\d{11})$)", ErrorMessage = "手机格式不正确")]
        public string Mobile { get; set; }

        /// <summary>
        /// 座机号码
        /// </summary>
        [Display(Name = "座机号码")]
        [MaxLength(16)]
        [RegularExpression(@"((\d{3,4}-\d{6,8})$)", ErrorMessage = "座机号码不正确")]
        public string Telephone { get; set; }


        /// <summary>
        /// 省份名称
        /// </summary>
        [NotMapped]
        public string ProvinceName { get; set; }

        /// <summary>
        /// 城市称
        /// </summary>
        [NotMapped]
        public string CityName { get; set; }

        /// <summary>
        /// 县区名称
        /// </summary>
        [NotMapped]
        public string DistrictName { get; set; }
    }
}
