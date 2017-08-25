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
            //List<string> aa = new List<string>() { "1", "2", "3" };
            //var dd = aa.ToJson();

            //Hashtable ht = new Hashtable();
            //ht["s"] = "1";
            //ht["t"] = "2";
            //string sss = ht.ToJson();
            using (DbRepository entities = new DbRepository())
            {
                var list = entities.DataDictionary.Where(x => x.GroupCode == GroupCode.Area).GroupBy(x => x.Key).ToList();

                list.ForEach(x =>
                {
                    if (x.Count() > 1)
                    {
                        var dd = x.Where(y => y.Value.Contains("旧")).FirstOrDefault();
                        entities.DataDictionary.Remove(dd);
                    }
                });
                int ss=entities.SaveChanges();
            }

        }
    }
}
