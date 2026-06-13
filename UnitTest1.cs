using System.Xml.Linq;

namespace TestProject1
{
    public class NutritionFixture
    {
        public IEnumerable<XElement> Patients { get; private set; }
        public IEnumerable<XElement> Products { get; private set; }
        public IEnumerable<XElement> Infos { get; private set; }
        public IEnumerable<XElement> Info1 { get; private set; }
        public IEnumerable<XElement> Info2 { get; private set; }
        public NutritionFixture()
        {
            Patients = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Patients>
	<Patient>
		<Id>1</Id>
		<LastName>Smith</LastName>
		<CarbsNorm>200</CarbsNorm>
		<ProteinsNorm>170</ProteinsNorm>
		<FatsNorm>150</FatsNorm>
	</Patient>
	<Patient>
		<Id>2</Id>
		<LastName>Aria</LastName>
		<CarbsNorm>10</CarbsNorm>
		<ProteinsNorm>170</ProteinsNorm>
		<FatsNorm>100</FatsNorm>
	</Patient>

</Patients>
").Descendants("Patient");
			Products = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Products>
	<Product>
		<Id>1</Id>
		<Name>Nut</Name>
		<CarbsPr>0.30</CarbsPr>
		<ProteinsPr>0.40</ProteinsPr>
		<FatsPr>0.80</FatsPr>
	</Product>
	<Product>
		<Id>2</Id>
		<Name>Avocado</Name>
		<CarbsPr>0.40</CarbsPr>
		<ProteinsPr>0.30</ProteinsPr>
		<FatsPr>0.80</FatsPr>
	</Product>
</Products>").Descendants("Product");
			Info1 = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Infos>
	<Info>
		<Date>2026-06-06</Date>
		<PatientId>1</PatientId>
		<ProductId>2</ProductId>
		<Weight>10</Weight>
	</Info>
	<Info>
		<Date>2026-06-06</Date>
		<PatientId>1</PatientId>
		<ProductId>2</ProductId>
		<Weight>15</Weight>
	</Info>
	<Info>
		<Date>2026-06-06</Date>
		<PatientId>2</PatientId>
		<ProductId>1</ProductId>
		<Weight>20</Weight>
	</Info>
	<Info>
		<Date>2026-06-06</Date>
		<PatientId>2</PatientId>
		<ProductId>2</ProductId>
		<Weight>40</Weight>
	</Info>
</Infos>
").Descendants("Info");
			Info2 = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Infos>
	<Info>
		<Date>2026-06-03</Date>
		<PatientId>1</PatientId>
		<ProductId>2</ProductId>
		<Weight>50</Weight>
	</Info>
	<Info>
		<Date>2026-06-03</Date>
		<PatientId>1</PatientId>
		<ProductId>2</ProductId>
		<Weight>55</Weight>
	</Info>
	<Info>
		<Date>2026-06-03</Date>
		<PatientId>2</PatientId>
		<ProductId>1</ProductId>
		<Weight>20</Weight>
	</Info>
	<Info>
		<Date>2026-06-03</Date>
		<PatientId>2</PatientId>
		<ProductId>2</ProductId>
		<Weight>40</Weight>
	</Info>
</Infos>").Descendants("Info");
			Infos= Info1.Concat(Info2);
                }

    }
    public class NutritionTesting : IClassFixture<NutritionFixture>
    {   
        private readonly NutritionFixture _fixture;
		public NutritionTesting(NutritionFixture fixture)
		{
			_fixture = fixture;
		}

       [Fact]
        public void Test1()
        {
			var exTree = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8""?>
<ProductStatistic>
  <Products>
    <Name>Avocado</Name>
    <Patient PersonalConWeight=""130"">
      <LastName>Smith</LastName>
    </Patient>
    <Patient PersonalConWeight=""80"">
      <LastName>Aria</LastName>
    </Patient>
  </Products>
  <Products>
    <Name>Nut</Name>
    <Patient PersonalConWeight=""40"">
      <LastName>Aria</LastName>
    </Patient>
  </Products>
</ProductStatistic>");
			var acTree = Nutrition.NutritionLogic.GetProductStat(_fixture.Patients, _fixture.Products, _fixture.Infos);
			Assert.True(XNode.DeepEquals(exTree, acTree));
        }
        [Fact]
        public void Test2()
        {
            var exTree = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8""?>
<PatientsStatistic>
  <Patients>
    <LastName>Smith</LastName>
    <Dates Date=""2026-06-03"" ConsumedCarbs=""42.00"" ConsumedProteins=""31.50"" ConsumedFats=""84.00"" />
    <Dates Date=""2026-06-06"" ConsumedCarbs=""10.00"" ConsumedProteins=""7.50"" ConsumedFats=""20.00"" />
  </Patients>
</PatientsStatistic>");
            var acTree = Nutrition.NutritionLogic.GetPatientNutrition(_fixture.Patients, _fixture.Products, _fixture.Infos);
            Assert.True(XNode.DeepEquals(exTree, acTree));
        }
    }
}
