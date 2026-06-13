using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace Pidgot_1
{
	public class HotLogic
	{
		public static XElement TaskA(IEnumerable<XElement> datas, IEnumerable<XElement> hotels, IEnumerable<XElement> guests, IEnumerable<XElement> categories, string city)
		{
			//xml - файл, де для заданого міста(передається як параметр статичного методу)
			//подано інформацію про історію бронювань.Звіт повинен мати ієрархічну структуру:
			//прізвище гостя(впорядковано за алфавітом) -> назва готелю(впорядковано за алфавітом у лексико-графічному порядку)
			//-> назва категорії номеру.Категорію та готель включати до звіту лише у випадку,
			//якщо вказаний гість дійсно бронював цей номер у цьому готелі заданого міста.
			//Якщо гість бронював одну й ту саму категорію в одному готелі декілька разів,
			//у звіті вона має відображатися без повторень. 

			var data = from d in datas
					   join c in categories on (int)d.Element("C_id") equals (int)c.Element("C_id")
					   join h in hotels on (int)c.Element("H_id") equals (int)h.Element("H_id")
					   where (string)h.Element("H_city") == city
					   join g in guests on (int)d.Element("G_id") equals (int)g.Element("G_id")
					   select new
					   {
						   Sur = (string)g.Element("Surname"),
						   Hotel = (string)h.Element("H_name"),
						   Category = (string)c.Element("C_name")
					   };
			return new XElement("TaskA",
				new XAttribute("City", city),
				from d in data
				group d by d.Sur into g
				orderby g.Key ascending
				select new XElement("Client",
					new XAttribute("Surname", g.Key),
					from ng in g
					group ng by ng.Hotel into gg
					orderby gg.Key ascending
					select new XElement("Hotel",
						new XAttribute("Title", gg.Key),
						from lg in gg
						group lg by lg.Category into c
						select new XElement("Category",
							new XAttribute("Name", c.Key))
					)
				)
			);

		}

		public static XElement TaskB(IEnumerable<XElement> datas, IEnumerable<XElement> hotels, IEnumerable<XElement> categories, DateTime start, DateTime end, double minDoh)
		{
			//xml - файл зі статистикою доходів по готелях за заданий період дат заїздів.
			//Статичний метод приймає два параметри типу DateTime(початок та кінець періоду заїздів)
			//та числове значення мінімального доходу.Для кожного готелю порахувати загальну кількість заброньованих ночей
			//та сумарний дохід(з урахуванням можливих знижок) лише за ті бронювання,
			//дата заїзду яких потрапляє у вказаний проміжок часу(включно).Звіт відсортувати за сумарним доходом у спадному порядку.
			//До звіту включати лише ті готелі, сумарний дохід яких дорівнює або перевищує задане значення мінімального доходу. 
			var data = from d in datas
					   join c in categories on (int)d.Element("C_id") equals (int)c.Element("C_id")
					   join h in hotels on (int)c.Element("H_id") equals (int)h.Element("H_id")
					   let tPrice = (int)d.Element("Nights") * (int)c.Element("Price")
					   let znPrice = (((DateTime)d.Element("Z_date") - (DateTime)d.Element("B_date")).TotalDays > 14) ? tPrice * 1.2 : tPrice
					   where ((DateTime)d.Element("Z_date") >= start) && ((DateTime)d.Element("Z_date") <= end)
					   select new
					   {
						   hotel = (string)h.Element("H_name"),
						   nights = (int)d.Element("Nights"),
						   price = znPrice
					   };
			return new XElement("TaskB",
				new XAttribute("From", start),
				new XAttribute("To", end),
				new XAttribute("minimal_dohid", minDoh),
				from d in data
				group d by d.hotel into hh
				where hh.Sum(x => x.price) > minDoh
				orderby hh.Sum(x => x.nights) descending
				select new XElement("Hotel",
					new XAttribute("Name", hh.Key),
					new XAttribute("Nights", hh.Sum(x => x.nights)),
					new XAttribute("Total", hh.Sum(x => x.price))
				)
			);

		}

		public static XElement TaskC(IEnumerable<XElement> datas, IEnumerable<XElement> categories)
		{
			//xml - файл, де для кожної категорії номерів вказано загальний дохід,
			//який вона принесла за весь час(з урахуванням усіх знижок по кожному конкретному бронюванню).
			//Перелік впорядкувати за назвою категорії у лексико-графічному порядку.

			var data = from d in datas
					   join c in categories on (int)d.Element("C_id") equals (int)c.Element("C_id")
					   let tPrice = (int)d.Element("Nights") * (int)c.Element("Price")
					   let znPrice = (((DateTime)d.Element("Z_date") - (DateTime)d.Element("B_date")).TotalDays > 14) ? tPrice * 1.2 : tPrice
					   select new
					   {
						   category = (string)c.Element("C_name"),
						   price = znPrice
					   };
			return new XElement("TaskC",
				from d in data
				group d by d.category into g
				orderby g.Key ascending
				select new XElement("Category",
					new XAttribute("Name", g.Key),
					new XAttribute("Total", g.Sum(x => x.price))
				)
			);
		}

		public static XElement TaskD(IEnumerable<XElement> datas, IEnumerable<XElement> hotels, IEnumerable<XElement> guests, IEnumerable<XElement> categories)
		{
			//xml - файл, де для кожного готелю визначено гостя(або гостей),
			//який приніс цьому готелю найбільший сумарний дохід(максимальна сума з урахуванням знижок серед усіх гостей,
			//що зупинялися в цьому готелі).До результату включити назву готелю,
			//цю максимальну суму та прізвища знайдених гостей.
			//Перелік впорядкувати за назвою готелю у лексико - графічному порядку.
			var data = from d in datas
					   join c in categories on (int)d.Element("C_id") equals (int)c.Element("C_id")
					   join h in hotels on (int)c.Element("H_id") equals (int)h.Element("H_id")
					   join g in guests on (int)d.Element("G_id") equals (int)g.Element("G_id")
					   let bPrice = (int)c.Element("Price") * (int)d.Element("Nights")
					   let price = ((DateTime)d.Element("Z_date") - (DateTime)d.Element("B_date")).TotalDays > 14 ? bPrice * 1.2 : bPrice
					   select new
					   {
						   hotel = (string)h.Element("H_name"),
						   guest = (string)g.Element("Surname"),
						   price = price
					   };
			return new XElement("TaskD",
				from d in data
				group d by d.hotel into hot
				orderby hot.Key ascending
				let guestTotals = from ho in hot
								  group ho by ho.guest into g
								  select new
								  {
									  Surname = g.Key,
									  Total = g.Sum(x => x.price)
								  }
				let max = guestTotals.Max(x => x.Total)
				select new XElement("Hotel",
					new XAttribute("Name", hot.Key),
					from gt in guestTotals
					where gt.Total == max
					select new XElement("Guest",
						new XAttribute("Surname", gt.Surname),
						new XAttribute("Total", gt.Total)
					)
				)
			);
		}
	}
	class Program
	{
		static void Main(string[] args)
		{
			var hotels = XDocument.Load("hotels.xml").Descendants("Hotel");
			var guests = XDocument.Load("guests.xml").Descendants("Guest");
			var categories = XDocument.Load("categories.xml").Descendants("Categorie");
			var datas1 = XDocument.Load("datas1.xml").Descendants("Data");
			var datas2 = XDocument.Load("datas2.xml").Descendants("Data");
			var datas = datas1.Concat(datas2);

			var taskA = HotLogic.TaskA(datas, hotels, guests, categories, "Lviv");
			taskA.Save("TaskA.xml");
			var taskB = HotLogic.TaskB(datas, hotels, categories, new DateTime(2026, 01, 01), new DateTime(2026, 06, 06), 0.0);
			taskB.Save("TaskB.xml");
			var taskC = HotLogic.TaskC(datas, categories);
			taskC.Save("TaskC.xml");
			var taskD = HotLogic.TaskD(datas, hotels, guests, categories);
			taskD.Save("TaskD.xml");
		}
	}
}