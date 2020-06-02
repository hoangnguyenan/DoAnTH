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

namespace HTFood.Controllers
{
    public class ChucVuController : Controller
    {
        string url = Constants.url;
        HttpClient client;
        public static List<ChucVu> listcv = new List<ChucVu>();
        public ChucVuController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jason"));
        }
        private dbHutechfoodContext db = new dbHutechfoodContext();

        // GET: QuyenAdmin
        public async Task<ActionResult> Index()
        {
            HttpResponseMessage responseMessage = await client.GetAsync(url + @"chucvu/");
            List<ChucVu> cv = getAllChucVu(responseMessage);
            if (cv != null)
            {
                ViewBag.accept = false;
                var list = cv.ToList();
                return View(list);
            }
            return View("Error");
        }
        public static List<ChucVu> getAllChucVu(HttpResponseMessage responseMessage)
        {
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                List<ChucVu> chucVus = JsonConvert.DeserializeObject<List<ChucVu>>(responseData, settings);
                var listnv = chucVus.ToList();
                return listnv;
            }
            return null;
        }
        // GET: QuyenAdmin/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            ChucVu chucVu = null;
            HttpResponseMessage response = await client.GetAsync(url + @"chucvu/" + id);
            if (response.IsSuccessStatusCode)
            {
                ViewBag.accept = false;
                chucVu = await response.Content.ReadAsAsync<ChucVu>();
            }
            return View(chucVu);
        }

        // GET: QuyenAdmin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: QuyenAdmin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChucVu chucVu)
        {
            HttpResponseMessage response = client.PostAsJsonAsync(url + @"chucvu/", chucVu).Result;
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Detail = "Sucess";
            }
            return RedirectToAction("Index");
        }

        // GET: QuyenAdmin/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            ChucVu chucVu = null;
            HttpResponseMessage response = await client.GetAsync(url + @"chucvu/" + id);
            if (response.IsSuccessStatusCode)
            {
                chucVu = await response.Content.ReadAsAsync<ChucVu>();
            }
            return View(chucVu);
        }

        // POST: QuyenAdmin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ChucVu chucVu)
        {
            HttpResponseMessage response = client.PutAsJsonAsync(url + @"chucvu/" + chucVu.Id, chucVu).Result;
            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }

        // GET: QuyenAdmin/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            HttpResponseMessage response = await client.DeleteAsync(url + @"chucvu/" + id);
            return RedirectToAction("Index", "ChucVu");
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
