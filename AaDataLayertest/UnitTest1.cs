using System.Data;

namespace AaDataLayertest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestDataTable()
        {
            Guid testGuid = Guid.NewGuid();
            AaTools.DataLayer.ParameterBuilder pb = new();
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
            AaTools.DataLayer.ParameterBuilder pb = new();
            pb.Add("@UUID", testGuid);
            pb.AddOut("@JsonResults", "");
            AaTestJsonOutput results = AaTools.DataLayer.AaDataAccessLayer.ExecuteObjectJson<AaTestJsonOutput>("[dbo].[AaTestJsonOutput]", pb, "@JsonResults", "TestDbConnection");
            if (results.FullUuid == testGuid)
            {
                Assert.IsTrue(true);
                return;
            }
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