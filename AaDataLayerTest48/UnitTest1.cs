using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AaDataLayerTest48
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestDataTable()
        {
            Guid testGuid = Guid.NewGuid();
            AaTools.DataLayer.ParameterBuilder pb = new AaTools.DataLayer.ParameterBuilder();
            pb.Add("@UUID", testGuid);
            DataTable dt = AaTools.DataLayer.AaDataAccessLayer.ExecuteDatatable("[dbo].[AaTestDataset]", pb, "TestDbConnection");
            foreach (DataRow r in dt.Rows)
            {
                if (r["FullUuid"].ToString() == testGuid.ToString())
                {
                    Assert.IsTrue(true);
                    return;
                }
            }
        }

        [TestMethod]
        public void TestJson()
        {
            Guid testGuid = Guid.NewGuid();
            AaTools.DataLayer.ParameterBuilder pb = new AaTools.DataLayer.ParameterBuilder();
            pb.Add("@UUID", testGuid);
            pb.AddOut("@JsonResults", "");
            AaTestJsonOutput results = AaTools.DataLayer.AaDataAccessLayer.ExecuteObjectJson<AaTestJsonOutput>("[dbo].[AaTestJsonOutput]", pb, "@JsonResults", "TestDbConnection");
            if (results.FullUuid == testGuid)
            {
                Assert.IsTrue(true);
                return;
            }
        }

        [TestMethod]
        public void TestErrorIgnored()
        {
            
            AaTools.DataLayer.ParameterBuilder pb = new AaTools.DataLayer.ParameterBuilder();
            pb.Add("@UUID", 15);
            AaTools.DataLayer.AaDataAccessLayer.ExecuteDatatable("[dbo].[AaTestDataset]", pb, "TestDbConnection",throwError:false);
            Assert.IsTrue(true);
        }


        [TestMethod]
        public void TestErrorThrown()
        {
            try
            {
                AaTools.DataLayer.ParameterBuilder pb = new AaTools.DataLayer.ParameterBuilder();
                pb.Add("@UUID", 15);
                DataTable dt = AaTools.DataLayer.AaDataAccessLayer.ExecuteDatatable("[dbo].[AaTestDataset]", pb, "TestDbConnection", throwError: true);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                AaTools.AaSerilog.GetInstance().Error(ex, "TestErrorThrown");
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void TestDataTableAsync()
        {

            Guid testGuid = Guid.NewGuid();
            AaTools.DataLayer.ParameterBuilder pb = new AaTools.DataLayer.ParameterBuilder();
            pb.Add("@UUID", testGuid);

            DataTable dt =  AaTools.DataLayer.AaDataAccessLayer.ExecuteDatatableAsync("[dbo].[AaTestDataset]", pb, "TestDbConnection").Result;
            foreach (DataRow r in dt.Rows)
            {
                if (r["FullUuid"].ToString() == testGuid.ToString())
                {
                    Assert.IsTrue(true);

                }
            }
        
            Assert.IsTrue(true);
        }
    }


  

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class AaTestJsonOutput
    {
        public Guid FullUuid { get; set; }
        public string FullUuidString { get; set; }
        public string Part1 { get; set; }
        public string Part2 { get; set; }
        public string Part3 { get; set; }
        public string Part4 { get; set; }
        public string Part5 { get; set; }
    }

    public class AaTestJsonOutputs : List<AaTestJsonOutput>
    {
    }
}
