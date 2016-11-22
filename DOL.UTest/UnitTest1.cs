using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DOL.Repository;
using System.Collections.Generic;
using DOL.Model;
using System.Linq;
using DOL.Core;
using System.IO;
using System.Text;
using System.Collections;

namespace DOL.UTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            List<string> aa = new List<string>() { "1", "2", "3" };
            var dd = aa.ToJson();

            Hashtable ht = new Hashtable();
            ht["s"] = "1";
            ht["t"] = "2";
            string sss = ht.ToJson();
            using (DbRepository entities = new DbRepository())
            {

                List<DataDictionary> list = entities.DataDictionary.Where(x=>x.GroupCode==GroupCode.Area).ToList();

                var dic = list.ToDictionary(x => x.ID);

                StreamReader sr = new StreamReader(@"D:\area.txt", Encoding.Default);
                String line;
                List<string> sqlList = new List<string>();
                List<string> keyList = new List<string>();
                while ((line = sr.ReadLine()) != null)
                {
                    var str = line.ToString();
                    if (str.Contains("[") && str.Contains("]"))
                    {
                        var strArray = str.Split('[');
                        var code = strArray[0].Trim();
                        var areaDic = new DataDictionary();
                        areaDic.Key = code;
                        areaDic.Value = strArray[1].Replace("]", "") + "(旧)";
                        areaDic.GroupCode = GroupCode.Area;
                        areaDic.ID = Guid.NewGuid().ToString("N");
                        //市
                        if (code.Contains("00"))
                        {
                            areaDic.ParentKey = code.Substring(0, 2) + "0000";
                        }
                        else
                        {
                            areaDic.ParentKey = code.Substring(0, 4) + "00";
                        }

                        entities.DataDictionary.Add(areaDic);

                        if (!dic.ContainsKey(areaDic.Key))
                        {
                            keyList.Add(code);
                            sqlList.Add(string.Format("insert into [DataDictionary](ID,[ParentKey],[GroupCode],[Key],[Value],[Remark]) values('{0}','{1}',{2},'{4}','{5}','{6}');\r\n", areaDic.ID, areaDic.ParentKey, 1, areaDic.Key, code, areaDic.Value, "旧版地区"));
                        }
                    }
                }
                string ggg = string.Join(",", keyList);
                string sdssss = string.Join("", sqlList);
            }
        }
    }
}
