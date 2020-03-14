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
    public class KhuyenmaiController : Controller
    {
        string url = Constants.url;
        HttpClient client;
        public static List<Khuyenmai> listKM = new List<Khuyenmai>();
        public KhuyenmaiController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jason"));
        }
        private dbHutechfoodContext db = new dbHutechfoodContext();

        // GET: Khuyenmai
        public async Task<ActionResult> Index(int? page)
        {
            HttpResponseMessage responseMessage = await client.GetAsync(url + @"khuyenmai/");
            List<Khuyenmai> list = getAllKM(responseMessage);
            if (list != null)
            {
                int pageSize = 8;//so san pham moi trang
                int pageNum = (page ?? 1);//tao so trang            
                return View(list.ToPagedList(pageNum, pageSize));
            }
            return View("Error");
        }

        public static List<Khuyenmai> getAllKM(HttpResponseMessage responseMessage)
        {
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                List<Khuyenmai> khuyenmais = JsonConvert.DeserializeObject<List<Khuyenmai>>(responseData, settings);
                return khuyenmais;
            }
            return null;
        }

        // GET: Khuyenmai/Details/5
        public async Task<ActionResult> Details(int? id, int? MaKH)
        {
            HttpResponseMessage response = await client.GetAsync(url + @"chitietkhuyenmai/" + id);
            if (response.IsSuccessStatusCode)
            {
                var responseData = response.Content.ReadAsStringAsync().Result;
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                var chiTietKhuyenMais = JsonConvert.DeserializeObject<List<ChiTietKhuyenMai>>(responseData, settings);
                List<ChiTietKhuyenMai> ds = chiTietKhuyenMais.ToList();
                // id Makm
                ViewBag.MDH = id;
                // Get khuyen mai
                HttpResponseMessage responseMessage = await client.GetAsync(url + @"khuyenmai/");
                List<Khuyenmai> khuyenmais = getAllKM(responseMessage);
                Khuyenmai km = khuyenmais.SingleOrDefault(n => n.MaKM == id);


                return View(chiTietKhuyenMais.ToList());
            }
            return View();
        }

        // GET: Khuyenmai/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Khuyenmai/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Khuyenmai khuyenmai)
        {
            HttpResponseMessage response = client.PostAsJsonAsync(url + @"khuyenmai/", khuyenmai).Result;
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Detail = "Sucess";
            }
            return RedirectToAction("Index");
        }

        // GET: Khuyenmai/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            Khuyenmai khuyenmai = null;
            HttpResponseMessage response = await client.GetAsync(url + @"khuyenmai/" + id);
            if (response.IsSuccessStatusCode)
            {
                khuyenmai = await response.Content.ReadAsAsync<Khuyenmai>();
            }
            return View(khuyenmai);
        }

        // POST: Khuyenmai/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Khuyenmai khuyenmai)
        {
            HttpResponseMessage response = client.PutAsJsonAsync(url + @"nhanviengiaohang/" + khuyenmai.MaKM, khuyenmai).Result;
            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }

        // GET: Khuyenmai/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            HttpResponseMessage response = await client.DeleteAsync(url + @"khuyenmai/" + id);
            return RedirectToAction("Index", "KhuyenMai");
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
