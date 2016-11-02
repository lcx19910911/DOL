using DOL.Core;
using DOL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOL.Web.Models
{
    public class CoacheInfoModel
    {
        public CoachReportModel CoachReportModel { get; set; }

        public List<SelectItem> DriverShopList { get; set; }
        public List<SelectItem> CoachList { get; set; }
    }
}