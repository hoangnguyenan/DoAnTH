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
    public class ViTriGiaoHangController : Controller
    {
        string url = "http://localhost/hutechfoodserver/api/";
        HttpClient client;
        public static List<ViTriGiaoHang> listViTri = new List<ViTriGiaoHang>();
        public ViTriGiaoHangController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jason"));
        }
        private dbHutechfoodContext db = new dbHutechfoodContext();

        // GET: ViTriGiaoHang
        public async Task<ActionResult> Index(int? page)
        {
            HttpResponseMessage responseMessage = await client.GetAsync(url + @"vitrigiaohang/");
            List<ViTriGiaoHang> list = getAllPosition(responseMessage);
            if(list != null)
            {
                int pageSize = 8;//so san pham moi trang
                int pageNum = (page ?? 1);//tao so trang            
                return View(list.ToPagedList(pageNum, pageSize));
            }
            return View("Error");
        }

        public static List<ViTriGiaoHang> getAllPosition(HttpResponseMessage responseMessage)
        {
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                List<ViTriGiaoHang> viTriGiaoHangs = JsonConvert.DeserializeObject<List<ViTriGiaoHang>>(responseData, settings);
                return viTriGiaoHangs;
            }
            return null;
        }

        // GET: ViTriGiaoHang/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            ViTriGiaoHang viTriGiaoHangs = null;
            HttpResponseMessage response = await client.GetAsync(url + @"vitrigiaohang/" + id);
            if (response.IsSuccessStatusCode)
            {
                viTriGiaoHangs = await response.Content.ReadAsAsync<ViTriGiaoHang>();
            }
            return View(viTriGiaoHangs);
        }

        // GET: ViTriGiaoHang/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ViTriGiaoHang/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ViTriGiaoHang viTriGiaoHangs)
        {
            HttpResponseMessage response = client.PostAsJsonAsync(url + @"vitrigiaohang/", viTriGiaoHangs).Result;
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Detail = "Sucess";
            }
            return RedirectToAction("Index");
        }

        // GET: ViTriGiaoHang/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            ViTriGiaoHang viTriGiaoHangs = null;
            HttpResponseMessage response = await client.GetAsync(url + @"vitrigiaohang/" + id);
            if (response.IsSuccessStatusCode)
            {
                viTriGiaoHangs = await response.Content.ReadAsAsync<ViTriGiaoHang>();
            }
            return View(viTriGiaoHangs);
        }

        // POST: ViTriGiaoHang/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ViTriGiaoHang viTriGiaoHangs)
        {
            HttpResponseMessage response = client.PutAsJsonAsync(url + @"vitrigiaohang/" + viTriGiaoHangs.MaVT, viTriGiaoHangs).Result;
            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }

        // GET: ViTriGiaoHang/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            HttpResponseMessage response = await client.DeleteAsync(url + @"vitrigiaohang/" + id);
            return RedirectToAction("Index", "ViTriGiaoHang");
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
