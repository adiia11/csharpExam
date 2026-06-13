using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TravelSystem
{
    public class TravelLogic
    {
        public static XElement GetRouteTransitStat(IEnumerable<XElement> passengers, IEnumerable<XElement> routes, IEnumerable<XElement> tickets)
        {
            var flatData = from t in tickets
                           join p in passengers on (int)t.Element("PassengerId")! equals (int)p.Element("Id")!
                           join r in routes on (int)t.Element("TrainId")! equals (int)r.Element("Id")!
                           select new
                           {
                               RouteName = (string)r.Element("RouteName")!,
                               LastName = (string)p.Element("LastName")!,
                               Price = (decimal)r.Element("TicketPrice")!
                           };
            var eachPassenger = from d in flatData
                                group d by new { d.RouteName, d.LastName } into gr
                                select new
                                {
                                    gr.Key.RouteName,
                                    gr.Key.LastName,
                                    TotalSum = gr.Sum(x => x.Price),
                                    TotalTrips = gr.Count()
                                };

            var finalMerge = from d in eachPassenger
                             group d by d.RouteName into routeGr
                             orderby routeGr.Key ascending
                             let maxTrips = routeGr.Max(x => x.TotalTrips)
                             select new XElement("Route",
                                 new XAttribute("Name", routeGr.Key),
                                 from p in routeGr
                                 where p.TotalTrips == maxTrips
                                 orderby p.LastName ascending
                                 select new XElement("Passenger",
                                     new XAttribute("LastName", p.LastName),
                                     new XAttribute("TotalSpent", p.TotalSum)
                                 )
                             );

            return new XElement("LoyaltyReport", finalMerge);
        }

        public static XElement GetOverspentReport(IEnumerable<XElement> passengers, IEnumerable<XElement> routes, IEnumerable<XElement> services, IEnumerable<XElement> tickets)
        {
            var flatData = from t in tickets
                           join p in passengers on (int)t.Element("PassengerId")! equals (int)p.Element("Id")!
                           join r in routes on (int)t.Element("TrainId")! equals (int)r.Element("Id")!
                           join s in services on (int?)t.Element("ServiceId") equals (int?)s.Element("Id") into servGroup
                           from serv in servGroup.DefaultIfEmpty()
                           select new
                           {
                               LastName = (string)p.Element("LastName")!,
                               Date = (DateTime)t.Element("Date")!,
                               Cost = (decimal)r.Element("TicketPrice")! + (serv != null ? (decimal)serv.Element("ServicePrice")! : 0m)
                           };

            var eachDay = from d in flatData
                          group d by new { d.LastName, d.Date } into dayGroup
                          let dailyTotal = dayGroup.Sum(x => x.Cost)
                          select new { dayGroup.Key.LastName, dayGroup.Key.Date, dailyTotal };

            var finalMerge = from e in eachDay
                             group e by e.LastName into patGroup
                             orderby patGroup.Key ascending
                             select new XElement("Passenger",
                                 new XAttribute("LastName", patGroup.Key),
                                 from p in patGroup
                                 orderby p.Date ascending
                                 select new XElement("BudgetDay",
                                     new XAttribute("Date", p.Date.ToString("yyyy-MM-dd")),
                                     new XAttribute("TotalSpent", p.dailyTotal)
                                 )
                             );

            return new XElement("HealthyBudgetReport", finalMerge);
        }
    }
}