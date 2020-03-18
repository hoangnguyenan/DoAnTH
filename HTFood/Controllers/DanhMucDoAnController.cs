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
    public class DanhMucDoAnController : Controller
    {
        string url = Constants.url;
        HttpClient client;
        public static List<DanhMucDoAn> listdm = new List<DanhMucDoAn>();
        public DanhMucDoAnController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jason"));
        }
        private dbHutechfoodContext db = new dbHutechfoodContext();

        // GET: DanhMucDoAn
        public async Task<ActionResult> Index(int? page)
        {
            HttpResponseMessage responseMessage = await client.GetAsync(url + @"danhmucdoan/");
            List<DanhMucDoAn> dm = getAllDanhMuc(responseMessage);
            if (dm != null)
            {
                ViewBag.accept = false;
                var list = dm.ToList();
                return View(list);
            }
            return View("Error");            
        }
        public static List<DanhMucDoAn> getAllDanhMuc(HttpResponseMessage responseMessage)
        {
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                List<DanhMucDoAn> danhMucDoAns = JsonConvert.DeserializeObject<List<DanhMucDoAn>>(responseData, settings);
                var listdm = danhMucDoAns.ToList();
                return listdm;
            }
            return null;
        }
        // GET: DanhMucDoAn/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            DanhMucDoAn danhmucdoans = null;
            HttpResponseMessage response = await client.GetAsync(url + @"danhmucdoan/" + id);
            if (response.IsSuccessStatusCode)
            {
                danhmucdoans = await response.Content.ReadAsAsync<DanhMucDoAn>();
            }
            return View(danhmucdoans);
        }

        // GET: DanhMucDoAn/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DanhMucDoAn/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DanhMucDoAn danhmucdoans)
        {
            HttpResponseMessage response = client.PostAsJsonAsync(url + @"danhmucdoan/", danhmucdoans).Result;
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                SetAlert("Thêm danh mục thành công!!!", "success");
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // GET: DanhMucDoAn/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            DanhMucDoAn danhmucdoans = null;
            HttpResponseMessage response = await client.GetAsync(url + @"danhmucdoan/" + id);
            if (response.IsSuccessStatusCode)
            {
                danhmucdoans = await response.Content.ReadAsAsync<DanhMucDoAn>();
            }
            return View(danhmucdoans);
        }

        // POST: DanhMucDoAn/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( DanhMucDoAn danhmucdoans)
        {
            HttpResponseMessage response = client.PutAsJsonAsync(url + @"danhmucdoan/" + danhmucdoans.MaDM, danhmucdoans).Result;
            response.EnsureSuccessStatusCode();
            SetAlert("Đã lưu chỉnh sửa!!!", "success");
            return RedirectToAction("Index");

        }

        // GET: DanhMucDoAn/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            HttpResponseMessage response = await client.DeleteAsync(url + @"danhmucdoan/" + id);
            return RedirectToAction("Index", "DanhMucDoAn");
        }
        protected void SetAlert(string message, string type)
        {
            TempData["AlertMessage"] = message;
            if (type == "success")
            {
                TempData["AlertType"] = "alert bg-green";
            }
            else if (type == "warning")
            {
                TempData["AlertType"] = "alert-warning";

            }
            else if (type == "error")
            {
                TempData["AlertType"] = "alert-danger";
            }
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
