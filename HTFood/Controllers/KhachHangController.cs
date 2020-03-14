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
    public class KhachHangController : Controller
    {
        string url = Constants.url;

        HttpClient client;
        public KhachHangController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jason"));
        }
        private dbHutechfoodContext db = new dbHutechfoodContext();

        // GET: KhachHang
        public async Task<ActionResult> Index(int? page)
        {
            HttpResponseMessage responseMessage = await client.GetAsync(url + @"khachhang/");
            List<KhachHang> khachHangs = getAllCustomerAsync(responseMessage);
            if (khachHangs != null)
            {
                int pageSize = 8;//so san pham moi trang
                int pageNum = (page ?? 1);//tao so trang            
                var list = khachHangs.ToList();
                return View(list.ToPagedList(pageNum, pageSize));
            }
            return View("Error");
        }

        public static List<KhachHang> getAllCustomerAsync(HttpResponseMessage responseMessage)
        {
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                List<KhachHang> khachHangs = JsonConvert.DeserializeObject<List<KhachHang>>(responseData, settings);
                var list = khachHangs.ToList();
                return list;
            }
            return null;
        }

        // GET: KhachHang/Details/5       
        public async Task<ActionResult> Details(int? id)
        {
            // Init variable
            KhachHang khachHangs = null;
            HttpResponseMessage response = await client.GetAsync(url + @"khachhang/" + id);
            if (response.IsSuccessStatusCode)
            {
                khachHangs = await response.Content.ReadAsAsync<KhachHang>();

                // Call api
                HttpResponseMessage responseMessage = await client.GetAsync(url + @"vitien/");
                // Get all data from the ViTien table 
                List<ViTien> viTiens = ViTienController.getAllViTien(responseMessage);
                // Check data with id customer
                viTiens = viTiens.Where(n=>n.MaKH == id).ToList();
                ViewBag.vitien = viTiens;
                // Get all data from the LichSuGD table
                responseMessage = await client.GetAsync(url + @"lichsugd/");
                // Init list receive data
                List<LichSuGD> lichSuGDs = new List<LichSuGD>();
                foreach (ViTien vt in viTiens)
                {
                    // Check data with id ViTien
                    List<LichSuGD> temp = LichSuGDController.getAllLichSuGD(responseMessage).Where(n => n.MaViTien == vt.MaViTien).ToList();
                    // Is exist
                    if (temp != null)
                    {
                        // Save data
                        foreach (LichSuGD lichSuGD in temp)
                        {
                            lichSuGDs.Add(lichSuGD);
                        }
                    }
                }
                // assign values to variables ViewBag.lsgd
                ViewBag.lsgd = lichSuGDs.ToList();
            }

            // Return
            return View(khachHangs);
        }
        // GET: KhachHang/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: KhachHang/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(KhachHang khachHangs)
        {
            HttpResponseMessage response = client.PostAsJsonAsync(url + @"khachhang/", khachHangs).Result;
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Detail = "Sucess";
            }
            return RedirectToAction("Index");
        }
        // GET: KhachHang/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            KhachHang khachHangs = null;
            HttpResponseMessage response = await client.GetAsync(url + @"khachhang/" + id);
            if (response.IsSuccessStatusCode)
            {
                khachHangs = await response.Content.ReadAsAsync<KhachHang>();
            }
            return View(khachHangs);
        }
        // POST: KhachHang/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaKH,HoTenKH,TaiKhoanKH,MatKhauKH,EmailKH,DiaChiKH,DienThoaiKH,NgaySinhKH")] KhachHang khachHangs)
        {
            HttpResponseMessage response = client.PutAsJsonAsync(url + @"khachhang/" + khachHangs.MaKH, khachHangs).Result;
            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }
        // GET: Food/Delete
        public async Task<ActionResult> Delete(int? id)
        {
            HttpResponseMessage response = await client.DeleteAsync(url + @"khachhang/" + id);
            return RedirectToAction("Index", "KhachHang");
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
