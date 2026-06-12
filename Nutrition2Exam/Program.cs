using System.Xml.Linq;
namespace Nutrition
{
    public class NutritionLogic
    {
        /// <summary> в межах норми
        /// Patient: Id, LastName, CarbsNorm,ProteinsNorm,FatsNorm
        /// Product: Id , Name, CarbsPr,ProteinsPr,FatsPr\
        /// Info: Date,PatientId,ProductId,Weight
        /// </summary> get=> for eachPorduct allPatiens and PersonalConsWeight, 
        /// <param name="patients"></param>
        /// <param name="products"></param>
        /// <param name="infos"></param>
        /// <returns></returns>
        public static XElement GetProductStat(IEnumerable<XElement> patients, IEnumerable<XElement> products, IEnumerable<XElement> infos)
        {
            var flatData = from i in infos
                           join pt in patients on (int)i.Element("PatientId")! equals (int)pt.Element("Id")!
                           join pr in products on (int)i.Element("ProductId")! equals (int)pr.Element("Id")!
                           select new
                           {
                               PrName = (string)pr.Element("Name")!,
                               PtLastName = (string)pt.Element("LastName")!,
                               ConsumedW = (decimal)i.Element("Weight")!

                           };
            var eachProduct = from f in flatData
                              group f by new { f.PrName, f.PtLastName } into ProdGr
                              let PerConsumedWeight = ProdGr.Sum(x => x.ConsumedW)
                              select new
                              {
                                  ProductN = ProdGr.Key.PrName,
                                  PtLastName = ProdGr.Key.PtLastName,
                                  PersonalConWeight = PerConsumedWeight
                              };
            var finalMerge = from e in eachProduct
                             group e by e.ProductN into EachPrGr
                             orderby EachPrGr.Key ascending
                             select new XElement("Products",
                             new XElement("Name", EachPrGr.Key),
                             from e in EachPrGr
                             orderby e.PersonalConWeight descending
                             select new XElement("Patient",
                             new XElement("LastName", e.PtLastName),
                             new XAttribute("PersonalConWeight", e.PersonalConWeight))
                             );

            return new XElement("ProductStatistic",finalMerge);
        }
        /// <summary>
        ///  Patient: Id, LastName, CarbsNorm,ProteinsNorm,FatsNorm
        /// Product: Id , Name, CarbsPr,ProteinsPr,FatsPr\
        /// Info: Date,PatientId,ProductId,Weight
        /// flatData->PtLastName and Date
        /// eachPatient-> PtLastName,Date,DayCarbs,DayProt, DayFats compare using where <=Norm select Patient, date DayCarbs
        /// </summary>
        /// <param name="patients"></param>
        /// <param name="products"></param>
        /// <param name="infos"></param>
        /// <returns></returns>
        public static XElement GetPatientNutrition(IEnumerable<XElement> patients, IEnumerable<XElement> products, IEnumerable<XElement> infos)
        {
            var flatData = from i in infos
                           join pt in patients on (int)i.Element("PatientId")! equals (int)pt.Element("Id")!
                           join pr in products on (int)i.Element("ProductId")! equals (int)pr.Element("Id")!
                           select new
                           {
                               PatLastName = (string)pt.Element("LastName")!,
                               CarbsNorm = (decimal)pt.Element("CarbsNorm")!,
                               ProteinsNorm = (decimal)pt.Element("ProteinsNorm")!,
                               FatsNorm = (decimal)pt.Element("FatsNorm")!,
                               Date = (DateTime)i.Element("Date")!,
                               CarbsCon = (decimal)i.Element("Weight")!*(decimal)pr.Element("CarbsPr")!,
                               ProteinsCon = (decimal)i.Element("Weight")! *(decimal)pr.Element("ProteinsPr")!,
                               FatsCon = (decimal)i.Element("Weight")! * (decimal)pr.Element("FatsPr")!
                           };
            var eachPatient = from f in flatData
                              group f by new { f.PatLastName, f.Date } into PatientGroup
                              let DayCarbsCon = PatientGroup.Sum(x => x.CarbsCon)
                              let DayProtsCon = PatientGroup.Sum(x => x.ProteinsCon)
                              let DayFatsCon = PatientGroup.Sum(x => x.FatsCon)
                              let patient = PatientGroup.First()
                            
                              where patient.CarbsNorm>=DayCarbsCon &&patient.ProteinsNorm>=DayProtsCon && patient.FatsNorm>=DayFatsCon
                              select new
                              {
                                  PtLastName = PatientGroup.Key.PatLastName,
                                  DateCon = PatientGroup.Key.Date,
                                  DayCarbsCons = DayCarbsCon,
                                  DayProtsCons = DayProtsCon,
                                  DayFatsCons = DayFatsCon
                              };
            var finalMerge = from e in eachPatient
                             group e by e.PtLastName into PatGroup
                             orderby PatGroup.Key ascending
                             select new XElement("Patients",
                             new XElement("LastName", PatGroup.Key),
                             from p in PatGroup
                             orderby p.DateCon ascending
                             select new XElement("Dates", new XAttribute("Date", p.DateCon.ToString("yyyy-MM-dd")),
                             new XAttribute("ConsumedCarbs", p.DayCarbsCons),
                             new XAttribute("ConsumedProteins", p.DayProtsCons),
                             new XAttribute("ConsumedFats", p.DayFatsCons)));


                           

            return new XElement("PatientsStatistic",finalMerge);
        }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            var patients = XDocument.Load("patients.xml").Descendants("Patient");
            var products = XDocument.Load("products.xml").Descendants("Product");
            var info1 = XDocument.Load("info1.xml").Descendants("Info");
            var info2 = XDocument.Load("info2.xml").Descendants("Info");
            var infos = info1.Concat(info2);
            var taskA = NutritionLogic.GetProductStat(patients, products, infos);
            string pathA = "D:\\Users\\LEGEND\\source\\repos\\Nutrition2Exam\\Nutrition2Exam\\taskA.xml";
            string pathB = "D:\\Users\\LEGEND\\source\\repos\\Nutrition2Exam\\Nutrition2Exam\\taskB.xml";
            taskA.Save(pathA);
            Console.WriteLine(taskA.ToString());
            var taskB = NutritionLogic.GetPatientNutrition(patients, products, infos);
            taskB.Save(pathB);
            Console.WriteLine(taskB.ToString());

        }
    }


}
