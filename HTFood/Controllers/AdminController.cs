﻿using HTFood.Models;
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

        // GET: Admin
        public async Task<ActionResult> Index()
        {
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
        public ActionResult Adminstrator()
        {
            return View();
        }
        public ActionResult InfoAdmin()
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
    }
}