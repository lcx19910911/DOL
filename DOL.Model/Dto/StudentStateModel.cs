using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Model.Dto
{
    public class StudentStateModel
    {
        public Dictionary<string, Dictionary<StudentCode, int>> dsd { get; set; }
    }
}
