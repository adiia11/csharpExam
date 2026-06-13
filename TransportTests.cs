using System.ComponentModel;
using System.Xml.Linq;

namespace TestProject1
{
	public class TransportFixture
	{
		public IEnumerable<XElement> Passengers { get; private set; }
		public IEnumerable<XElement> Routes { get; private set; }
		public IEnumerable<XElement> Info1 { get; private set; }
		public IEnumerable<XElement> Info2 { get; private set; }
		public IEnumerable<XElement> Infos { get; private set; }
		public TransportFixture()
		{
			Passengers = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Passengers>
	<Passenger>
		<Id>1</Id>
		<LastName>Berva</LastName>
		<MaxLimSpent>50</MaxLimSpent>
	</Passenger>
	<Passenger>
		<Id>2</Id>
		<LastName>Aerbova</LastName>
		<MaxLimSpent>40</MaxLimSpent>
	</Passenger>
<Passenger>
	<Id>3</Id>
	<LastName>Smith</LastName>
	<MaxLimSpent>40</MaxLimSpent>
</Passenger>
</Passengers>").Descendants("Passenger");
			Routes = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Routes>
	<Route>
		<Id>1</Id>
		<Title>Troley</Title>
		<TicketPrice>25</TicketPrice>
	</Route>
	<Route>
		<Id>2</Id>
		<Title>Bus</Title>
		<TicketPrice>35</TicketPrice>
	</Route>
</Routes>").Descendants("Route");
			Info1 = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Infos>
	<Info>
		<Date>2026-06-02</Date>
		<PassengerId>1</PassengerId>
		<RouteId>2</RouteId>
		<Count>4</Count>
	</Info>
	<Info>
		<Date>2026-06-02</Date>
		<PassengerId>2</PassengerId>
		<RouteId>1</RouteId>
		<Count>4</Count>
	</Info>
	<Info>
		<Date>2026-06-02</Date>
		<PassengerId>2</PassengerId>
		<RouteId>1</RouteId>
		<Count>10</Count>
	</Info>
	<Info>
		<Date>2026-06-02</Date>
		<PassengerId>3</PassengerId>
		<RouteId>1</RouteId>
		<Count>16</Count>
	</Info>
</Infos>").Descendants("Info");
			Info2 = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Infos>
	<Info>
		<Date>2026-06-06</Date>
		<PassengerId>1</PassengerId>
		<RouteId>1</RouteId>
		<Count>2</Count>
	</Info>
	<Info>
		<Date>2026-06-06</Date>
		<PassengerId>2</PassengerId>
		<RouteId>1</RouteId>
		<Count>4</Count>
	</Info>
	<Info>
		<Date>2026-06-06</Date>
		<PassengerId>2</PassengerId>
		<RouteId>2</RouteId>
		<Count>10</Count>
	</Info>
	<Info>
		<Date>2026-06-06</Date>
		<PassengerId>3</PassengerId>
		<RouteId>2</RouteId>
		<Count>15</Count>
	</Info>
</Infos>").Descendants("Info");
			Infos = Info1.Concat(Info2);


		}
	}

		public class TransportTesting : IClassFixture<TransportFixture>
		{
		private readonly TransportFixture _fixture;
		public TransportTesting(TransportFixture fixture)
		{
			_fixture = fixture;
        }


			[Fact]
			public void Test1()
			{
			var exTree = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8""?>
<TravelStat>
  <Routes>
    <RouteTitle>Bus</RouteTitle>
    <Passengers TotalCount=""15"">
      <PassengerLastName>Smith</PassengerLastName>
    </Passengers>
    <Passengers TotalCount=""10"">
      <PassengerLastName>Aerbova</PassengerLastName>
    </Passengers>
    <Passengers TotalCount=""4"">
      <PassengerLastName>Berva</PassengerLastName>
    </Passengers>
  </Routes>
  <Routes>
    <RouteTitle>Troley</RouteTitle>
    <Passengers TotalCount=""18"">
      <PassengerLastName>Aerbova</PassengerLastName>
    </Passengers>
    <Passengers TotalCount=""16"">
      <PassengerLastName>Smith</PassengerLastName>
    </Passengers>
    <Passengers TotalCount=""2"">
      <PassengerLastName>Berva</PassengerLastName>
    </Passengers>
  </Routes>
</TravelStat>");
			var acTree = Transport2Exam.TransportLogic.GetRouteStat(_fixture.Passengers, _fixture.Routes, _fixture.Infos);
			Assert.True(XNode.DeepEquals(exTree, acTree));


            }
        [Fact]
        public void Test2()
        {
            var exTree = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8""?>
<OverLimStat>
  <Passengers>
    <Passenger>Aerbova</Passenger>
    <OverspentDay Date=""2026-06-02"" DayTicketCount=""2"" SpentMoney=""350"" />
    <OverspentDay Date=""2026-06-06"" DayTicketCount=""2"" SpentMoney=""450"" />
  </Passengers>
  <Passengers>
    <Passenger>Berva</Passenger>
    <OverspentDay Date=""2026-06-02"" DayTicketCount=""1"" SpentMoney=""140"" />
  </Passengers>
  <Passengers>
    <Passenger>Smith</Passenger>
    <OverspentDay Date=""2026-06-02"" DayTicketCount=""1"" SpentMoney=""400"" />
    <OverspentDay Date=""2026-06-06"" DayTicketCount=""1"" SpentMoney=""525"" />
  </Passengers>
</OverLimStat>");
            var acTree = Transport2Exam.TransportLogic.OverLimitCommuting(_fixture.Passengers, _fixture.Routes, _fixture.Infos);
            Assert.True(XNode.DeepEquals(exTree, acTree));


        }
    }
}
