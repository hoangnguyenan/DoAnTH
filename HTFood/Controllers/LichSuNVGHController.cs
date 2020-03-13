using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HTFood.Models;
using Newtonsoft.Json;
using PagedList;

namespace HTFood.Controllers
{
    public class LichSuNVGHController : Controller
    {
        string url = Constants.url;
        HttpClient client;
        public static List<LichSuNVGH> listLsnv = new List<LichSuNVGH>();
        public LichSuNVGHController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jason"));
        }
        private dbHutechfoodContext db = new dbHutechfoodContext();

        // GET: LichSuNVGH
        public async Task<ActionResult> Index(int? page)
        {
            HttpResponseMessage responseMessage = await client.GetAsync(url + @"lichsunvgh/");
            List<LichSuNVGH> ls = getAllLichSuGD(responseMessage);
            if (ls != null)
            {
                int pageSize = 8;//so san pham moi trang
                int pageNum = (page ?? 1);//tao so trang      
                var list = ls.ToList();
                return View(list.ToPagedList(pageNum, pageSize));
            }
            return View("Error");
        }
        public static List<LichSuNVGH> getAllLichSuGD(HttpResponseMessage responseMessage)
        {
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                List<LichSuNVGH> lichSuNVGHs = JsonConvert.DeserializeObject<List<LichSuNVGH>>(responseData, settings);
                var listlsnv = lichSuNVGHs.ToList();
                return listlsnv;
            }
            return null;
        }
        // GET: LichSuNVGH/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            LichSuNVGH lichSuNVGH = null;
            HttpResponseMessage response = await client.GetAsync(url + @"lichsunvgh/" + id);
            if (response.IsSuccessStatusCode)
            {
                lichSuNVGH = await response.Content.ReadAsAsync<LichSuNVGH>();
            }
            return View(lichSuNVGH);
        }

        // GET: LichSuNVGH/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LichSuNVGH/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LichSuNVGH lichSuNVGH)
        {
            HttpResponseMessage response = client.PostAsJsonAsync(url + @"lichsunvgh/", lichSuNVGH).Result;
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Detail = "Sucess";
            }
            return RedirectToAction("Index");
        }

        // GET: LichSuNVGH/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            LichSuNVGH lichSuNVGH = null;
            HttpResponseMessage response = await client.GetAsync(url + @"lichsunvgh/" + id);
            if (response.IsSuccessStatusCode)
            {
                lichSuNVGH = await response.Content.ReadAsAsync<LichSuNVGH>();
            }
            return View(lichSuNVGH);
        }

        // POST: LichSuNVGH/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LichSuNVGH lichSuNVGH)
        {
            HttpResponseMessage response = client.PutAsJsonAsync(url + @"lichsunvgh/" + lichSuNVGH.MaLS, lichSuNVGH).Result;
            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }

        // GET: LichSuNVGH/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            HttpResponseMessage response = await client.DeleteAsync(url + @"lichsunvgh/" + id);
            return RedirectToAction("Index", "LichSuNVGH");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
