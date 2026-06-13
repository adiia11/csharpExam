using System;
using System.Xml.Linq;
namespace Transport2Exam
{
    /// <summary>
    ///  Проєктування типів (Моделей даних):Пасажир характеризується числовим ідентифікатором (Id), прізвищем (LastName) 
    ///  Passenger: Id,LastName, MaxLimSpent
    ///  та індивідуальним добовим лімітом (максимально дозволеною сумою) витрат на транспорт 
    ///  Маршрут (Транспорт) характеризується числовим ідентифікатором (Id), назвою маршруту (RouteTitle) 
    ///  Passenger: Id,LastName, MaxLimSpent
    /// Route: Id,Title(string), TicketPrice
    ///   /// Infos: Date,PassengerId,RouteId, Count
    ///  та базовою вартістю однієї поїздки
    /// Infos: Date,PassengerId,RouteId, Count
    ///  Журнал валідацій (Поїздок) містить дату та час поїздки (Date), ідентифікатор пасажира (PassengerId), ідентифікатор маршруту (RouteId) 
    ///  та кількість пасажирів, за яких було оплачено одноразово цим пасажиром (Count).
    ///  Інформацію про поїздки подано кількома (не менше 2) окремими XML-файлами,
    ///  які в Main необхідно об'єднати через .Concat().2.
    ///  Отримати та реалізувати:
    ///  (а) Статистика завантаженості маршрутів:Отримати обʼєкт типу XElement,
    ///  у якому для кожного маршруту вказати перелік пасажирів,  eachRoute allPassengers persSum+count (RtTitle,
    ///  які ним користувалися, а також персональну сумарну кількість поїздок (з урахуванням сплачених додаткових пасажирів Count), 
    ///  яку цей пасажир здійснив на цьому маршруті за весь період.Сортування: Перелік маршрутів впорядкувати за назвою/номером в 
    ///  алфавітному порядку. 
    ///  Перелік пасажирів всередині кожного маршруту впорядкувати за спаданням (від найбільшого до найменшого)
    ///  сумарної кількості здійснених поїздок.Вимога до XML: Прізвища пацієнтів/пасажирів та їхню особисту кількість поїздок записати строго
    ///  в XAttribute тегу <Passenger>. Отриманий результат вивести у файл TaskA.xml.(б) Звіт про дні перевищення ліміту витрат:Отримати обʼєкт 
    ///  типу XElement, у якому для кожного пасажира вказати щодобові сумарні витрати на проїзд, але лише у ті дні, коли пасажир перевищив свій 
    ///  особистий добовий ліміт витрат ($> \text{MaxExpenseLimit}$).Математика витрат за день: $\text{Кількість оплачених місць (Count)} \times
    ///  \text{Вартість квитка на маршруті (TicketPrice)}$. Якщо пасажир за один день їздив кількома різними маршрутами, витрати за цей день додаються разом.
    ///  Сортування: Перелік пасажирів впорядкувати за прізвищем у лексико-графічному порядку без повторень. 
    ///  Дні всередині кожного пасажира впорядкувати за датами в хронологічному порядку 
    ///  (від старіших до новіших).Вимога до XML: Дані про дату та сумарні витрачені кошти за цей день записати 
    ///  строго в XAttribute вкладеного тегу <OverspentDay>. Отриманий результат вивести у файл TaskB.xml.
    /// </summary>
    public class TransportLogic {
        //Отримати та реалізувати:
        ///  (а) Статистика завантаженості маршрутів:Отримати обʼєкт типу XElement,
        ///  у якому для кожного маршруту вказати перелік пасажирів,
        ///  eachRoute allPassengers persSum+count (RtTitle,
        ///  які ним користувалися, а також персональну сумарну кількість поїздок (з урахуванням сплачених додаткових пасажирів Count), 
        ///  яку цей пасажир здійснив на цьому маршруті за весь період.
        ///  Сортування: Перелік маршрутів впорядкувати за назвою/номером в 
        ///  алфавітному порядку.  
        ///  Перелік пасажирів всередині кожного маршруту впорядкувати за спаданням (від найбільшого до найменшого)
        ///  сумарної кількості здійснених поїздок.Вимога до XML: Прізвища пацієнтів/пасажирів та їхню особисту кількість поїздок записати строго
        ///  в XAttribute тегу <Passenger>
        ///   Passenger: Id,LastName, MaxLimSpent
        /// Route: Id,Title(string), TicketPrice
        /// Infos: Date,PassengerId,RouteId, Count

        public static XElement GetRouteStat(IEnumerable<XElement>passengers, IEnumerable<XElement> routes, IEnumerable<XElement> infos) {
            var flatData = from i in infos
                           join p in passengers on (int)i.Element("PassengerId")! equals (int)p.Element("Id")!
                         //  where (string)p.Element("LastName")! == "Verbova"|| (string)p.Element("LastName")! == "Smith"
                           join r in routes on (int)i.Element("RouteId")! equals (int)r.Element("Id")!
                           select new
                           {
                               RouteTitle = (string)r.Element("Title")!,
                               PasLastName = (string)p.Element("LastName")!,
                               PasCount = (int)i.Element("Count")!,
                          
                           };
            var eachRoute = from f in flatData
                            group f by new { f.RouteTitle, f.PasLastName } into RouteGr
                           
                            let TotalPassCount = RouteGr.Sum(x => x.PasCount)

                            select new
                            {
                                PassLastName = RouteGr.Key.PasLastName,
                                RouteTitle = RouteGr.Key.RouteTitle,
                                TotalPasCount = TotalPassCount

                            };
            var finalMerge = from e in eachRoute
                             group e by e.RouteTitle into RouteGroup
                             orderby RouteGroup.Key ascending
                             select new XElement("Routes",
                             new XElement("RouteTitle", RouteGroup.Key),
                             from r in RouteGroup

                             orderby r.TotalPasCount descending
                             select new XElement("Passengers",
                             new XElement("PassengerLastName", r.PassLastName),
                             new XAttribute("TotalCount", r.TotalPasCount)
                             ));

            return new XElement("TravelStat", finalMerge);
        }
        ///  типу XElement, у якому для кожного пасажира вказати щодобові сумарні витрати на проїзд,
        /// passLastName
        /// але лише у ті дні, коли пасажир перевищив свій 
        ///  особистий добовий ліміт витрат ($> \text{MaxExpenseLimit}$).Математика витрат за день: $\text{Кількість оплачених місць (Count)} \times
        ///  \text{Вартість квитка на маршруті (TicketPrice)}$. Якщо пасажир за один день їздив кількома різними маршрутами, витрати за цей день додаються разом.
        ///  Сортування: Перелік пасажирів впорядкувати за прізвищем у лексико-графічному порядку без повторень. 
        ///  Дні всередині кожного пасажира впорядкувати за датами в хронологічному порядку 
        ///  (від старіших до новіших).Вимога до XML: Дані про дату та сумарні витрачені кошти за цей день записати 
        ///  строго в XAttribute вкладеного тегу <OverspentDay>. Отриманий результат вивести у файл TaskB.xml.
        ///   Passenger: Id,LastName, MaxLimSpent
        /// Route: Id,Title(string), TicketPrice
        ///   /// Infos: Date,PassengerId,RouteId, Count
        ///  passLastName where MaxLimSpent< DaySpent, Date
        public static XElement OverLimitCommuting(IEnumerable<XElement> passengers, IEnumerable<XElement>routes, IEnumerable<XElement> infos)
        {

            var flatData = from i in infos
                           join p in passengers on (int)i.Element("PassengerId")! equals (int)p.Element("Id")!
                           join r in routes on (int)i.Element("RouteId")! equals (int)r.Element("Id")!
                           select new
                           {
                               PassLastName = (string)p.Element("LastName")!,
                               MaxLimSpent = (int)p.Element("MaxLimSpent")!,
                               Date = (DateTime)i.Element("Date")!,
                               AllTicketPrice=(int)r.Element("TicketPrice")! * (int)i.Element("Count")!,
                               CountTic= (int)r.Element("TicketPrice")!

                           };
            var eachPass = from f in flatData
                           group f by new { f.Date, f.PassLastName } into passGr
                           let DaySpentM = passGr.Sum(x => x.AllTicketPrice)
                           let pas=passGr.First()
                           let ticketCount= passGr.Count()
                           where DaySpentM>pas.MaxLimSpent
                           select new
                           {
                               PasLastName = passGr.Key.PassLastName,
                               DateTr = passGr.Key.Date,
                               SpentMoney = DaySpentM,
                               CountTick=ticketCount

                              
                           };
            var finalMerge = from e in eachPass
                             group e by e.PasLastName into PsGr
                             orderby PsGr.Key ascending
                             select new XElement("Passengers",
                             new XElement("Passenger", PsGr.Key),
                             from g in PsGr
                             orderby g.DateTr ascending
                             select new XElement("OverspentDay",
                             new XAttribute("Date", g.DateTr.ToString("yyyy-MM-dd")),
                             new XAttribute("DayTicketCount",g.CountTick),
                             new XAttribute("SpentMoney", g.SpentMoney)

                                 ));



            return new XElement("OverLimStat", finalMerge);
        }
}

    public class TrProgram
    {
        public static void Main(string[] args)
        {

            var passengers = XDocument.Load("passengers.xml").Descendants("Passenger");
            var routes = XDocument.Load("routes.xml").Descendants("Route");
            var info1 = XDocument.Load("info1.xml").Descendants("Info");
            var info2 = XDocument.Load("info2.xml").Descendants("Info");
            var infos = info1.Concat(info2);
            string pathA = "D:\\Users\\LEGEND\\source\\repos\\Transport2Exam\\Transport2Exam\\TaskA.xml";
            string pathB = "D:\\Users\\LEGEND\\source\\repos\\Transport2Exam\\Transport2Exam\\TaskB.xml";
            var TaskA = TransportLogic.GetRouteStat(passengers, routes, infos);
            var TaskB= TransportLogic.OverLimitCommuting(passengers, routes, infos);
            TaskA.Save(pathA);
            Console.WriteLine(TaskA.ToString());
            TaskB.Save(pathB);
            Console.WriteLine(TaskB.ToString());
        }


    }
}