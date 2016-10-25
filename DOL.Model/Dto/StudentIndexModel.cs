using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Model
{
    /// <summary>
    /// 菜单
    /// </summary>
    public class StudentIndexModel
    {
        public List<EnteredPoint> EnteredPointList { get; set; }

        public List<Reference> ReferenceList { get; set; }

        public List<DriverShop> DriverShopList { get; set; }
    }
}
