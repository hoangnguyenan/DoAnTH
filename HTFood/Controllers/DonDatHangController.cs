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
    public class DonDatHangController : Controller
    {
        string url = Constants.url;
        HttpClient client;
        public DonDatHangController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jason"));
        }
        private dbHutechfoodContext db = new dbHutechfoodContext();

        // GET: DonDatHang
        public async Task<ActionResult> Index(int? page)
        {
            HttpResponseMessage responseMessage = await client.GetAsync(url + @"dondathang/");
            List<DonDatHang> list = getAllOrder(responseMessage);
            if (list != null)
            {
                int pageSize = 8;//so san pham moi trang
                int pageNum = (page ?? 1);//tao so trang            
                return View(list.ToPagedList(pageNum, pageSize));
            }
            return View("Error");
        }

        public static List<DonDatHang> getAllOrder(HttpResponseMessage responseMessage)
        {
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                List<DonDatHang> ddh = JsonConvert.DeserializeObject<List<DonDatHang>>(responseData, settings);
                return ddh;
            }
            return null;
        }

        // GET: chiTietDonHAng/Details        
        public async Task<ActionResult> Details(int? id, int? MaKH)
        {
            HttpResponseMessage response = await client.GetAsync(url + @"chitietdonhang/" + id);
            if (response.IsSuccessStatusCode)
            {
                var responseData = response.Content.ReadAsStringAsync().Result;
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                var chiTietDonHangs = JsonConvert.DeserializeObject<List<ChiTietDonHang>>(responseData, settings);
                List<ChiTietDonHang> ds = chiTietDonHangs.ToList();
                // id mdh
                ViewBag.MDH = id;

                // Get total price
                ViewBag.total = getTotal(ds);

                // Get don dat hang
                HttpResponseMessage responseMessage = await client.GetAsync(url + @"dondathang/");
                List<DonDatHang> donDatHangs = getAllOrder(responseMessage);
                DonDatHang ddh = donDatHangs.SingleOrDefault(n => n.MaDonHang == id);
                ViewBag.ngaydat = ddh.NgayDat;
                ViewBag.tinhtrang = ddh.TinhTrangDonHang;

                //get DoAn
                responseMessage = await client.GetAsync(url + @"doan/");
                List<DoAn> listda = HTFood.Controllers.DoAnController.getAllDoAn(responseMessage);
                List<string> dsTen = new List<string>();
                foreach (ChiTietDonHang ctdh in ds)
                {
                    string name = listda.Where(n => n.MaDA == ctdh.MaDA).SingleOrDefault().TenDA;
                    dsTen.Add(name);
                }
                //DoAn doAn = listda.Where(n => n.MaDA == );
                ViewBag.nameDa = dsTen;

                // Get name user
                responseMessage = await client.GetAsync(url + @"khachhang/");
                List<KhachHang> list = HTFood.Controllers.KhachHangController.getAllCustomerAsync(responseMessage);
                KhachHang khachHang = list.Where(n=>n.MaKH == MaKH).SingleOrDefault();
                ViewBag.name = khachHang.HoTenKH;
                ViewBag.id = khachHang.MaKH;
                ViewBag.dtkh = khachHang.DienThoaiKH;

                // Get position
                responseMessage = await client.GetAsync(url + @"vitrigiaohang/");
                List<ViTriGiaoHang> listVt = HTFood.Controllers.ViTriGiaoHangController.getAllPosition(responseMessage);
                ViTriGiaoHang viTri = listVt.Where(n => n.MaVT == ddh.MaVT).SingleOrDefault();
                ViewBag.vitri = viTri.TenVT;

                //get NVGH
                responseMessage = await client.GetAsync(url + @"nhanviengiaohang/");
                List<NhanVienGiaoHang> listnv = HTFood.Controllers.NhanVienGiaoHangController.getAllNhanVienGH(responseMessage);
                NhanVienGiaoHang nhanvien = listnv.Where(n => n.MaNV == ddh.MaNV).SingleOrDefault();
                ViewBag.namenv = nhanvien.HoTenNV;
                ViewBag.idnv = nhanvien.MaNV;
                ViewBag.dtnv = nhanvien.DienThoaiNV;

                ////get DoAn
                //responseMessage = await client.GetAsync(url + @"doan/");
                //List<DoAn> listda = HTFood.Controllers.DoAnController.getAllDoAn(responseMessage);
                //DoAn doAn = listda.Where(n => n.MaDA == MaDA).SingleOrDefault();
                //ViewBag.nameDa = doAn.TenDA;

                return View(chiTietDonHangs.ToList());
            }
            return View();
            //ChiTietDonHang chiTietDonHang = null;
            //HttpResponseMessage response = await client.GetAsync(url + @"chitietdonhang/" + id);
            //if (response.IsSuccessStatusCode)
            //{
            //    chiTietDonHang = await response.Content.ReadAsAsync<ChiTietDonHang>();
            //}
            //return View(chiTietDonHang);
        }

        private int getTotal(List<ChiTietDonHang> ds)
        {
            int total = 0;
            foreach (ChiTietDonHang item in ds)
            {
                total += (int) item.ThanhTien;
            }
            return total;
        }

        // GET: DonDatHang/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DonDatHang/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DonDatHang dondathangs)
        {
            HttpResponseMessage response = client.PostAsJsonAsync(url + @"dondathang/", dondathangs).Result;
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Detail = "Sucess";
            }
            return RedirectToAction("Index");
        }
        // GET: DonDatHang/Edit/5       
        public async Task<ActionResult> Edit(int? id)
        {
            DonDatHang donDatHangs = null;
            HttpResponseMessage response = await client.GetAsync(url + @"dondathang/" + id);
            if (response.IsSuccessStatusCode)
            {
                donDatHangs = await response.Content.ReadAsAsync<DonDatHang>();
            }
            return View(donDatHangs);
        }
        // POST: DonDatHang/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TinhTrangDonHang,NgayDat,NgayGiao,TgGiao,MaKH,MaNV,MaVT")] DonDatHang donDatHang)
        {
            HttpResponseMessage response = client.PutAsJsonAsync(url + @"dondathang/" + donDatHang.MaDonHang, donDatHang).Result;
            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }
        
        // GET: DonDatHang/Delete/5
       public async Task<ActionResult> Delete(int? id)
        {
            HttpResponseMessage response = await client.DeleteAsync(url + @"dondathang/" + id);
            return RedirectToAction("Index", "DonDatHang");
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
