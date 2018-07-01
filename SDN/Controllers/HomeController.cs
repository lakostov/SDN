using SDN.Processors;
using System.Web.Mvc;

namespace SDN.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //Check if local and remote SDN files are equal
            XmlDiffHtml xdh = new XmlDiffHtml();
            ViewBag.HtmlContent = xdh.GetXmlDiffHtml();

            return View();
        }
        public ActionResult Patch()
        {
            //Patch local file with the difference
            XmlDiffHtml xdh = new XmlDiffHtml();
            xdh.PatchList();
            //Check if local and remote SDN files are equal 
            ViewBag.HtmlContent = xdh.GetXmlDiffHtml();
            return View("Index");
        }

    }
}