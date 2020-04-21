using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
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
    public class DoAnController : Controller
    {
        string url = Constants.url;
        HttpClient client;
        public static List<DoAn> listda = new List<DoAn>();
        public DoAnController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jason"));
        }
        private dbHutechfoodContext db = new dbHutechfoodContext();

        // GET: DoAn
        public async Task<ActionResult> Index(int? page)
        {
            HttpResponseMessage responseMessage = await client.GetAsync(url + @"doan/");
            List<DoAn> da = getAllDoAn(responseMessage);
            if (da != null)
            {
                ViewBag.accept = false;
                var list = da.ToList();
                return View(list);
            }
            return View("Error");
        }
        public static List<DoAn> getAllDoAn(HttpResponseMessage responseMessage)
        {
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                List<DoAn> doAns = JsonConvert.DeserializeObject<List<DoAn>>(responseData, settings);
                var listda = doAns.ToList();
                return listda;
            }
            return null;
        }
        // GET: DoAn/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            DoAn doAn = null;
            HttpResponseMessage response = await client.GetAsync(url + @"doan/" + id);
            if (response.IsSuccessStatusCode)
            {
                doAn = await response.Content.ReadAsAsync<DoAn>();
            }
            return View(doAn);
        }

        // GET: DoAn/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DoAn/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DoAn doAn, HttpPostedFileBase fileupload)
        {
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh";
                return View();
            }
            //Them vao CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileupload.FileName);
                    //Luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/images"), fileName);
                    //Kiem tra hình anh ton tai chua?
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                        return View(doAn);
                    }
                    else
                    {
                        //Luu hinh anh vao duong dan
                        fileupload.SaveAs(path);
                    }
                    doAn.AnhDA = fileName;
                    //Luu vao CSDL
                    db.DoAns.Add(doAn);
                    db.SaveChanges();
                    HttpResponseMessage response = client.PostAsJsonAsync(url + @"doan/", doAn).Result;
                    response.EnsureSuccessStatusCode();
                }
                return RedirectToAction("Index");
            }
        }

        // GET: DoAn/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            DoAn doAn = null;
            HttpResponseMessage response = await client.GetAsync(url + @"doan/" + id);
            if (response.IsSuccessStatusCode)
            {
                doAn = await response.Content.ReadAsAsync<DoAn>();
            }
            return View(doAn);
        }

        // POST: DoAn/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaDA,TenDA,DonGia,AnhDA,MoTa,NgayCapNhat,SoLuongTon,TrangThaiDA,DanhGiaDoAn,MaDM")] DoAn doAn, HttpPostedFileBase fileupload)
        {

            if (fileupload != null)
            {
                var fileName = Path.GetFileName(fileupload.FileName);
                //Luu duong dan cua file
                var path = Path.Combine(Server.MapPath("~/images"), fileName);
                if (System.IO.File.Exists(path))
                {
                    //Xoa hinh cu trong duong dan
                    System.IO.File.Delete(path);
                    //Luu hinh anh moi vao duong dan
                    fileupload.SaveAs(path);
                }
                doAn.AnhDA = fileName;
            }

            db.SaveChanges();

            HttpResponseMessage response = client.PutAsJsonAsync(url + @"doan/" + doAn.MaDA, doAn).Result;
            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }
        // GET: DoAn/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            //LOI: XOA DC NHUNG CHUA XOA IMAGES TRONG PROJECT
            HttpResponseMessage response = await client.DeleteAsync(url + @"doan/" + id);
            return RedirectToAction("Index", "DoAn");
        }

        //// POST: DoAn/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    DoAn doAn = db.DoAns.Find(id);
        //    db.DoAns.Remove(doAn);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
