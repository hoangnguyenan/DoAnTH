using HTFood.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HTFood.Controllers
{
    public class AdminController : Controller
    {
        string url = Constants.url;
        HttpClient client;
        public AdminController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jason"));
        }
        private dbHutechfoodContext db = new dbHutechfoodContext();

        [HttpGet]
        public ActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignIn(FormCollection collection)
        {

            var user = collection["UserAdmin"];
            var pass = collection["PassAdmin"];
            if (String.IsNullOrEmpty(user))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(pass))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                HttpResponseMessage response = client.GetAsync(url + @"admin/").Result;
                Admin ad = getAllAdmin(response).Where(n => n.UserAdmin.CompareTo(user) == 0 && n.PassAdmin.CompareTo(pass) == 0).SingleOrDefault();
                //ad = db.Admins.Where(n => n.UserAdmin == user && n.PassAdmin == pass).SingleOrDefault();

                if (ad != null)
                {
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }
        public ActionResult LogOut()
        {
            Session["Taikhoanadmin"] = null;
            Session.Clear();
            Response.Redirect(Url.Action("SignIn"));
            return View("SignIn");
        }
        public static List<Admin> getAllAdmin(HttpResponseMessage responseMessage)
        {
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                List<Admin> ad = JsonConvert.DeserializeObject<List<Admin>>(responseData, settings);
                return ad;
            }
            return null;
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
                List<ChucVu> cv = JsonConvert.DeserializeObject<List<ChucVu>>(responseData, settings);
                return cv;
            }
            return null;
        }
        // GET: Admin
        public async Task<ActionResult> Index()
        {
            ViewBag.accept = false;
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("SignIn");
            }
            HttpResponseMessage responseMessage = await client.GetAsync(url + @"dondathang/");
            List<DonDatHang> list = DonDatHangController.getAllOrder(responseMessage);
            ViewBag.CountOrder = list.Count;

            String dataChart = "";
            // data = ''
            dataChart += list.Where(n => int.Parse(n.TinhTrangDonHang) == Constants.STATUS_ORDER_CONFIRM).Count() + ",";
            // data = '2,'
            dataChart += list.Where(n => int.Parse(n.TinhTrangDonHang) == Constants.STATUS_ORDER_DELIVERY).Count() + ",";
            // data = '2,1,'
            dataChart += list.Where(n => int.Parse(n.TinhTrangDonHang) == Constants.STATUS_ORDER_DELIVERED).Count() + ",";
            // data = '2,1,1'
            dataChart += list.Where(n => int.Parse(n.TinhTrangDonHang) == Constants.STATUS_SHIP_BOOM).Count();

            ViewBag.DataChart = dataChart;


            responseMessage = await client.GetAsync(url + @"khachhang/");
            List<KhachHang> listkh = KhachHangController.getAllCustomer(responseMessage);
            ViewBag.CountCustomer = listkh.Count;

            responseMessage = await client.GetAsync(url + @"chitietdonhang/");
            List<ChiTietDonHang> listctdh = getAllDetailOrder(responseMessage);
            ViewBag.revenue = getRevenue(listctdh);

            responseMessage = await client.GetAsync(url + @"nhanviengiaohang/");
            List<NhanVienGiaoHang> listnv = NhanVienGiaoHangController.getAllNhanVienGH(responseMessage);
            ViewBag.CountShipper = listnv.Count;

            responseMessage = await client.GetAsync(url + @"cuahang/");
            List<Cuahang> listch = CuahangController.getAllCuaHang(responseMessage);
            ViewBag.CountStore = listch.Count;

            responseMessage = await client.GetAsync(url + @"doan/");
            List<DoAn> listda = DoAnController.getAllDoAn(responseMessage);
            ViewBag.CountFood = listda.Count;

            responseMessage = await client.GetAsync(url + @"danhmucdoan/");
            List<DanhMucDoAn> listdm = DanhMucDoAnController.getAllDanhMuc(responseMessage);
            ViewBag.CountMenu = listdm.Count;

            responseMessage = await client.GetAsync(url + @"vitrigiaohang/");
            List<ViTriGiaoHang> listvt = ViTriGiaoHangController.getAllPosition(responseMessage);
            ViewBag.CountPosition = listvt.Count;

            return View();
        }
        //danh sách chi tiết đơn hàng
        public static List<ChiTietDonHang> getAllDetailOrder(HttpResponseMessage responseMessage)
        {
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                List<ChiTietDonHang> chiTietDonHangs = JsonConvert.DeserializeObject<List<ChiTietDonHang>>(responseData, settings);
                var list = chiTietDonHangs.ToList();
                return list;
            }
            return null;
        }
        //Tong doanh thu
        private int getRevenue(List<ChiTietDonHang> ds)
        {
            int total = 0;
            foreach (ChiTietDonHang item in ds)
            {
                total += (int)item.ThanhTien;
            }
            return total;
        }
        public async Task<ActionResult> MemberAdmin()
        {
            HttpResponseMessage responseMessage = await client.GetAsync(url + @"admin/");
            List<Admin> admins = getAllAdmin(responseMessage);
            if (admins != null)
            {
                ViewBag.accept = false;
                var list = admins.ToList();
                return View(list);
            }
            return View("Error");
        }

        public async Task<ActionResult> Create()
        {
            HttpResponseMessage response = await client.GetAsync(url + @"chucvu/");
            List<ChucVu> list = getAllChucVu(response);
            ViewBag.TenCV = list;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Admin admins, FormCollection f)
        {
            // Get QuyenAdmin
            int quyenAdmin = int.Parse(f["AdminRole"]);
            AdminRole adminRole = new AdminRole();
            // Admin
            adminRole.Id = admins.Id;
            adminRole.HotenAdmin = admins.HotenAdmin;
            adminRole.PassAdmin = admins.PassAdmin;
            adminRole.UserAdmin = admins.UserAdmin;
            adminRole.EmailAdmin = admins.EmailAdmin;
            adminRole.MaQuyen = quyenAdmin;
            // Call API Create
            HttpResponseMessage response = client.PostAsJsonAsync(url + @"adminrole/", adminRole).Result;
            response.EnsureSuccessStatusCode();
            return RedirectToAction("MemberAdmin", "Admin");
        }
        public async Task<ActionResult> Edit(int? id)
        {
            AdminRole adminRole = null;
            HttpResponseMessage response = await client.GetAsync(url + @"adminrole/" + id);
            if (response.IsSuccessStatusCode)
            {
                adminRole = await response.Content.ReadAsAsync<AdminRole>();
            }
            response = await client.GetAsync(url + @"chucvu/");
            List<ChucVu> list = getAllChucVu(response);
            ViewBag.TenCV = list;
            return View(adminRole);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Admin admins, FormCollection f)
        {
            // Get QuyenAdmin
            int quyenAdmin = int.Parse(f["AdminRole"]);
            AdminRole adminRole = new AdminRole();
            // Admin
            adminRole.Id = admins.Id;
            adminRole.HotenAdmin = admins.HotenAdmin;
            adminRole.PassAdmin = admins.PassAdmin;
            adminRole.UserAdmin = admins.UserAdmin;
            adminRole.EmailAdmin = admins.EmailAdmin;
            adminRole.MaQuyen = quyenAdmin;
            HttpResponseMessage response = client.PutAsJsonAsync(url + @"adminrole/" + adminRole.Id, adminRole).Result;
            response.EnsureSuccessStatusCode();

            return RedirectToAction("MemberAdmin", "Admin");
        }
        public async Task<ActionResult> Delete(int? id)
        {
            HttpResponseMessage response = await client.DeleteAsync(url + @"adminrole/" + id);
            return RedirectToAction("MemberAdmin", "Admin");
        }
        public async Task<ActionResult> InfoAdmin(int? id)
        {
            Admin admin = null;
            HttpResponseMessage response = await client.GetAsync(url + @"admin/" + id);
            if (response.IsSuccessStatusCode)
            {
                admin = await response.Content.ReadAsAsync<Admin>();
                //var responseData = response.Content.ReadAsStringAsync().Result;
                //var settings = new JsonSerializerSettings
                //{
                //    NullValueHandling = NullValueHandling.Ignore,
                //    MissingMemberHandling = MissingMemberHandling.Ignore
                //};
                //var admin = JsonConvert.DeserializeObject<List<Admin>>(responseData, settings);
                //List<Admin> ds = getAllAdmin(response);

                //response = await client.GetAsync(url + @"chucvu/");
                //List<ChucVu> listcv = getAllChucVu(response);
                //List<string> dsTen = new List<string>();
                //foreach (Admin ad in ds)
                //{
                //    string name = listcv.Where(n => n.Id == ad.Id).SingleOrDefault().TenCV;
                //    dsTen.Add(name);
                //}
                //ViewBag.tencv = dsTen;
            }
            return View(admin);
        }
        public ActionResult Sanpham()
        {
            return View();
        }
        public ActionResult Khachhang()
        {
            return View();
        }
        public ActionResult ChitietKH()
        {
            return View();
        }
        
        
        public ActionResult Diadiem()
        {
            return View();
        }
        public ActionResult Dondathang()
        {
            return View();
        }
        public ActionResult Nhanviengh()
        {
            return View();
        }
        public ActionResult ChitietNV()
        {
            return View();
        }
        public ActionResult Chitietdonhang()
        {
            return View();
        }
        public ActionResult Tintuc()
        {
            return View();
        }
        public ActionResult Cuahang()
        {
            return View();
        }
        public ActionResult Danhmuc()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> getProductAsync(String input)
        {
            List<DoAn> results = new List<DoAn>();
            HttpResponseMessage responseMessage = await client.GetAsync(url + @"doan/");
            List<DoAn> listDoAn = DoAnController.getAllDoAn(responseMessage);            
            results = listDoAn.Where(n => n.TenDA.ToLower().Contains(input.ToLower())).ToList();

            //List<NhanVienGiaoHang> results1 = new List<NhanVienGiaoHang>();
            //responseMessage = await client.GetAsync(url + @"nhanviengiaohang/");
            //List<NhanVienGiaoHang> lstnv = NhanVienGiaoHangController.getAllNhanVienGH(responseMessage);
            //results1 = lstnv.Where(n => n.HoTenNV.ToLower().Contains(input.ToLower())).ToList();

            return Json(results, JsonRequestBehavior.AllowGet);
        }
    }
}