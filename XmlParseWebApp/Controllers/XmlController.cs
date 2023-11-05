using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using Twilio.Rest.Monitor.V1;

namespace XmlParseWebApp.Controllers
{
    public class XmlController : Controller
    {

        // GET: XML
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ProcessXml(string xmlUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // XML verisini al
                    XDocument doc = XDocument.Load(xmlUrl);

                    // Verileri depolamak için liste oluştur
                    List<Dictionary<string, string>> dataList = new List<Dictionary<string, string>>();

                    // XML içeriğini oku ve listeye ekle
                    XNamespace ns = doc.Root.GetDefaultNamespace();
                    foreach (XElement element in doc.Descendants(ns + "item"))
                    {
                        Dictionary<string, string> dataDictionary = new Dictionary<string, string>();
                        foreach (XElement field in element.Elements())
                        {
                            dataDictionary[field.Name.LocalName] = field.Value;
                        }
                        dataList.Add(dataDictionary);
                    }

                    // Controller'da ViewData kullanarak view'e liste verisini gönder
                    ViewData["DataList"] = dataList;

                    // View'e git
                    return View("Index");
                }
                catch (Exception ex)
                {
                    // Hata durumunda yapılacaklar
                    return Content("Hata oluştu: " + ex.Message);
                }
            }
        }

        private decimal ConvertPriceToDecimal(string price)
        {
            string numericString = price.Replace("TRY", "").Trim();
            if (decimal.TryParse(numericString.Replace(",", "."), out decimal result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        [HttpPost]
        public async Task<ActionResult> SortByPrice()
        {
            List<Dictionary<string, string>> dataList = (List<Dictionary<string, string>>)ViewData["DataList"];

            if (dataList == null || dataList.Count == 0)
            {
                Console.WriteLine("Null Değer Var");
            }
            else
            {
                dataList = dataList.OrderBy(x => ConvertPriceToDecimal(x["price"])).ToList();
            }
           
            ViewData["DataList"] = dataList;

            return View("Index");
        }
    }
}

