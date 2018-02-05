using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UrlController : Controller
    {
        private readonly UrlContext _db = new UrlContext();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }

            base.Dispose(disposing);
        }

        // GET: Test
        public ActionResult Index()
        {
            return RedirectToAction("UrlForm", "Url");
        }

        [HttpGet]
        public ActionResult UrlForm()
        {
            return View();
        }

        // POST: Test/UrlForm
        [HttpPost]
        public ActionResult UrlForm(UrlViewModel model)
        {
            string parsedUrl = model.UrlText;
            //var list = parsedUrl.Split('\n');
            var list = parsedUrl.Split(new char[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
            return View("UrlInfo", list);
        }

        [HttpGet]
        public async Task<ActionResult> GetUrlInfo(string url)
        {
            // Validate URL
            // Add HTTP prefix if necessary
            var uri = new UriBuilder(url).Uri;

            var now = DateTime.Now;

            var model = new UrlInfoModel();
            model.URL = url;
            model.LastRequestDate = now.ToString("G");

            try
            {
                // Query data
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(uri);

                    model.LastStatus = response.StatusCode.ToString();

                    if (response.IsSuccessStatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        model.LastTitle = GetTitle(body);
                    }
                }
            }
            catch (Exception)
            {
                model.LastStatus = "Error";
            }

            // Save to database
            _db.Url.Add(new UrlInfo
            {
                URL = uri.ToString(),
                LastRequestDate = now,
                LastStatus = model.LastStatus,
                LastTitle = model.LastTitle,
            });
            _db.SaveChanges();

            return Json(model, JsonRequestBehavior.AllowGet);
        }
      
        [HttpGet]
        public ActionResult UrlStatistics(string url)
        {
            IList<UrlInfo> listInfo = new List<UrlInfo>();

            var listStat = (from p in _db.Url
                group p by p.URL
                into grp
                let LastRequestDate = grp.Max(p => p.LastRequestDate)
                let URL = grp.Key
                from p in grp
                where p.URL == URL &&
                      p.LastRequestDate == LastRequestDate
                select p).ToList();

            return View(listStat);
        }

        public ActionResult Graf(string url)
        {
            return View((object)url);
        }

        [HttpGet] 
        public ActionResult GrafData(string url)
        {
            var query = (from p in _db.Url
                where p.URL == url
                group p by EntityFunctions.TruncateTime(p.LastRequestDate) // в базе нужно поменять на дату без времени
                into grpByDate
                select new
                {
                    Date = grpByDate.Key,
                    Statuses = grpByDate.GroupBy(x => x.LastStatus).Select(x => new { Status = x.Key, Count = x.Count() })
                }).ToList();

            // Workaround to format DateTime
            var result = query.Select(x => new {Date = x.Date.ToString(), Statuses = x.Statuses});
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        static string GetTitle(string site)
        {
            Match m = Regex.Match(site, @"<title>\s*(.+?)\s*</title>");
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            else
            {
                return "";
            }
        }
    }
}
