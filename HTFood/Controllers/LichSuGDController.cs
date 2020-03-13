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
    public class LichSuGDController : Controller
    {
        string url = Constants.url;
        HttpClient client;
        public static List<LichSuGD> listvi = new List<LichSuGD>();
        public LichSuGDController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jason"));
        }
        private dbHutechfoodContext db = new dbHutechfoodContext();

        // GET: LichSuGD
        public async Task<ActionResult> Index(int? page)
        {
            HttpResponseMessage responseMessage = await client.GetAsync(url + @"lichsugd/");
            List<LichSuGD> ls = getAllLichSuGD(responseMessage);
            if (ls != null)
            {
                int pageSize = 8;//so san pham moi trang
                int pageNum = (page ?? 1);//tao so trang      
                var list = ls.ToList();
                return View(list.ToPagedList(pageNum, pageSize));
            }
            return View("Error");
        }
        public static List<LichSuGD> getAllLichSuGD(HttpResponseMessage responseMessage)
        {
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                List<LichSuGD> lichSuGDs = JsonConvert.DeserializeObject<List<LichSuGD>>(responseData, settings);
                var listls = lichSuGDs.ToList();
                return listls;
            }
            return null;
        }
        // GET: LichSuGD/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            LichSuGD lichSuGD = null;
            HttpResponseMessage response = await client.GetAsync(url + @"lichsugd/" + id);
            if (response.IsSuccessStatusCode)
            {
                lichSuGD = await response.Content.ReadAsAsync<LichSuGD>();
            }
            return View(lichSuGD);
        }

        // GET: LichSuGD/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LichSuGD/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LichSuGD lichSuGD)
        {
            HttpResponseMessage response = client.PostAsJsonAsync(url + @"lichsugd/", lichSuGD).Result;
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Detail = "Sucess";
            }
            return RedirectToAction("Index");
        }

        // GET: LichSuGD/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            LichSuGD lichSuGD = null;
            HttpResponseMessage response = await client.GetAsync(url + @"lichsugd/" + id);
            if (response.IsSuccessStatusCode)
            {
                lichSuGD = await response.Content.ReadAsAsync<LichSuGD>();
            }
            return View(lichSuGD);
        }

        // POST: LichSuGD/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LichSuGD lichSuGD)
        {
            HttpResponseMessage response = client.PutAsJsonAsync(url + @"lichsugd/" + lichSuGD.MaGD, lichSuGD).Result;
            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }

        // GET: LichSuGD/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            HttpResponseMessage response = await client.DeleteAsync(url + @"lichsugd/" + id);
            return RedirectToAction("Index", "LichSuGD");
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
