namespace DOL.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// ��Ŀ
    /// </summary>
    public partial class Theme : BaseEntity
    {

        /// <summary>
        /// ��Ŀ����
        /// </summary>
        [Display(Name = "��Ŀ����")]
        [MaxLength(32)]
        [Required(ErrorMessage = "��Ŀ����Ϊ��")]
        [Column("Name", TypeName = "varchar")]
        public string Name { get; set; }

        
    }
}
