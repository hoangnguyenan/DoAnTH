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

    public class NhanVienGiaoHangController : Controller
    {
        string url = Constants.url;
        HttpClient client;
        public static List<NhanVienGiaoHang> listnvgh = new List<NhanVienGiaoHang>();
        public NhanVienGiaoHangController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jason"));
        }
        private dbHutechfoodContext db = new dbHutechfoodContext();

        // GET: NhanVienGiaoHang
        public async Task<ActionResult> Index()
        {
            HttpResponseMessage responseMessage = await client.GetAsync(url + @"nhanviengiaohang/");
            List<NhanVienGiaoHang> nvgh = getAllNhanVienGH(responseMessage);
            if (nvgh != null)
            {
                ViewBag.accept = false;
                var list = nvgh.ToList();
                return View(list);
            }                       
            return View("Error");
        }
        public static List<NhanVienGiaoHang> getAllNhanVienGH(HttpResponseMessage responseMessage)
        {
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                List<NhanVienGiaoHang> nhanVienGiaoHangs = JsonConvert.DeserializeObject<List<NhanVienGiaoHang>>(responseData, settings);
                var listnv = nhanVienGiaoHangs.ToList();
                return listnv;
            }
            return null;
        }
        // GET: NhanVienGiaoHang/Details/5      
        public async Task<ActionResult> Details(int? id)
        {
            NhanVienGiaoHang nhanVienGiaoHang = null;
            HttpResponseMessage response = await client.GetAsync(url + @"nhanviengiaohang/" + id);
            if (response.IsSuccessStatusCode)
            {
                ViewBag.accept = false;
                nhanVienGiaoHang = await response.Content.ReadAsAsync<NhanVienGiaoHang>();

                response = await client.GetAsync(url + @"lichsunvgh/");
                List<LichSuNVGH> ls = LichSuNVGHController.getAllLichSuGD(response).Where(n=>n.MaNV == id).ToList();
                ViewBag.lsnvgh = ls;
            }
            return View(nhanVienGiaoHang);
        }
        // GET: NhanVienGiaoHang/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NhanVienGiaoHang/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NhanVienGiaoHang nhanVienGiaoHang)
        {
            HttpResponseMessage response = client.PostAsJsonAsync(url + @"nhanviengiaohang/", nhanVienGiaoHang).Result;
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Detail = "Sucess";
            }
            return RedirectToAction("Index");
        }

        // GET: NhanVienGiaoHang/Edit/5        
        public async Task<ActionResult> Edit(int? id)
        {
            NhanVienGiaoHang nhanVienGiaoHang = null;
            HttpResponseMessage response = await client.GetAsync(url + @"nhanviengiaohang/" + id);
            if (response.IsSuccessStatusCode)
            {
                nhanVienGiaoHang = await response.Content.ReadAsAsync<NhanVienGiaoHang>();
            }
            return View(nhanVienGiaoHang);
        }

        // POST: NhanVienGiaoHang/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NhanVienGiaoHang nhanVienGiaoHang)
        {
            HttpResponseMessage response = client.PutAsJsonAsync(url + @"nhanviengiaohang/" + nhanVienGiaoHang.MaNV, nhanVienGiaoHang).Result;
            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }

        // GET: NhanVienGiaoHang/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            HttpResponseMessage response = await client.DeleteAsync(url + @"nhanviengiaohang/" + id);
            return RedirectToAction("Index", "NhanVienGiaoHang");
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
