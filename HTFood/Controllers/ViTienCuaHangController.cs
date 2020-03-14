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
    public class ViTienCuaHangController : Controller
    {
        string url = Constants.url;
        HttpClient client;
        public static List<ViTienCuaHang> listViCH = new List<ViTienCuaHang>();
        public ViTienCuaHangController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jason"));
        }
        private dbHutechfoodContext db = new dbHutechfoodContext();

        // GET: ViTienCuaHangs
        public async Task<ActionResult> Index(int? page)
        {
            HttpResponseMessage responseMessage = await client.GetAsync(url + @"vitiencuahang/");
            List<ViTienCuaHang> list = getAllViCH(responseMessage);
            if (list != null)
            {
                int pageSize = 8;//so san pham moi trang
                int pageNum = (page ?? 1);//tao so trang            
                return View(list.ToPagedList(pageNum, pageSize));
            }
            return View("Error");
        }

        public static List<ViTienCuaHang> getAllViCH(HttpResponseMessage responseMessage)
        {
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                List<ViTienCuaHang> viTienCuaHangs = JsonConvert.DeserializeObject<List<ViTienCuaHang>>(responseData, settings);
                return viTienCuaHangs;
            }
            return null;
        }

        // GET: ViTienCuaHangs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            ViTienCuaHang vitiencuahangs = null;
            HttpResponseMessage response = await client.GetAsync(url + @"vitiencuahang/" + id);
            if (response.IsSuccessStatusCode)
            {
                vitiencuahangs = await response.Content.ReadAsAsync<ViTienCuaHang>();
            }
            return View(vitiencuahangs);
        }

        // GET: ViTienCuaHangs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ViTienCuaHangs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ViTienCuaHang vitiencuahangs)
        {
            HttpResponseMessage response = client.PostAsJsonAsync(url + @"vitiencuahang/", vitiencuahangs).Result;
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                SetAlert("Thêm hàng thành công!!!", "success");
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // GET: ViTienCuaHangs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            ViTienCuaHang viTienCuaHang = null;
            HttpResponseMessage response = await client.GetAsync(url + @"vitiencuahang/" + id);
            if (response.IsSuccessStatusCode)
            {
                viTienCuaHang = await response.Content.ReadAsAsync<ViTienCuaHang>();
            }
            return View(viTienCuaHang);
        }

        // POST: ViTienCuaHangs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaViCH,SoDu,NgayGiaoDich,SoTienGiaoDich,MaCH")] ViTienCuaHang viTienCuaHang)
        {
            HttpResponseMessage response = client.PutAsJsonAsync(url + @"vitiencuahang/" + viTienCuaHang.MaViCH, viTienCuaHang).Result;
            response.EnsureSuccessStatusCode();
            SetAlert("Đã lưu chỉnh sửa!!!", "success");
            return RedirectToAction("Index");
        }

        // GET: ViTienCuaHangs/Delete/5


        // POST: ViTienCuaHangs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            HttpResponseMessage response = await client.DeleteAsync(url + @"vitiencuahang/" + id);
            SetAlert("Xóa thành công!!!", "success");
            return RedirectToAction("Index", "ViTienCuaHang");
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
