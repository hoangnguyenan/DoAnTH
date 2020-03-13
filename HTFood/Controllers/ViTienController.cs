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
    public class ViTienController : Controller
    {
        string url = Constants.url;
        HttpClient client;
        public static List<ViTien> listvi= new List<ViTien>();
        public ViTienController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jason"));
        }
        private dbHutechfoodContext db = new dbHutechfoodContext();

        // GET: ViTien
        public async Task<ActionResult> Index(int? page)
        {
            HttpResponseMessage responseMessage = await client.GetAsync(url + @"vitien/");
            List<ViTien> vi = getAllViTien(responseMessage);
            if (vi != null)
            {
                int pageSize = 8;//so san pham moi trang
                int pageNum = (page ?? 1);//tao so trang      
                var list = vi.ToList();
                return View(list.ToPagedList(pageNum, pageSize));
            }
            return View("Error");
        }
        public static List<ViTien> getAllViTien(HttpResponseMessage responseMessage)
        {
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                List<ViTien> viTiens = JsonConvert.DeserializeObject<List<ViTien>>(responseData, settings);
                var listvi = viTiens.ToList();
                return listvi;
            }
            return null;
        }
        // GET: ViTien/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            ViTien viTien = null;
            HttpResponseMessage response = await client.GetAsync(url + @"vitien/" + id);
            if (response.IsSuccessStatusCode)
            {
                viTien = await response.Content.ReadAsAsync<ViTien>();
            }
            return View(viTien);
        }

        // GET: ViTien/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ViTien/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ViTien viTien)
        {
            HttpResponseMessage response = client.PostAsJsonAsync(url + @"vitien/", viTien).Result;
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Detail = "Sucess";
            }
            return RedirectToAction("Index");
        }

        // GET: ViTien/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            ViTien viTien = null;
            HttpResponseMessage response = await client.GetAsync(url + @"vitien/" + id);
            if (response.IsSuccessStatusCode)
            {
                viTien = await response.Content.ReadAsAsync<ViTien>();
            }
            return View(viTien);
        }

        // POST: ViTien/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( ViTien viTien)
        {
            HttpResponseMessage response = client.PutAsJsonAsync(url + @"vitien/" + viTien.MaViTien, viTien).Result;
            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }

        // GET: ViTien/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            HttpResponseMessage response = await client.DeleteAsync(url + @"vitien/" + id);
            return RedirectToAction("Index", "ViTien");
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
