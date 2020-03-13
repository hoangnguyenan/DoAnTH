using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HTFood.Models
{
    public class KhachHang
    {
        [Key]
        public int MaKH { get; set; }
        public string HoTenKH { get; set; }
        public string TaiKhoanKH { get; set; }
        public string MatKhauKH { get; set; }
        public string EmailKH { get; set; }
        public string DiaChiKH { get; set; }
        public int DienThoaiKH { get; set; }
        public DateTime NgaySinhKH { get; set; }
    }
    public class DonDatHang
    {
        [Key]
        public int MaDonHang { get; set; }
        public string TinhTrangDonHang { get; set; }
        public DateTime NgayDat { get; set; }
        public DateTime NgayGiao { get; set; }
        public short TgGiao { get; set; }
        public int MaKH { get; set; }
        public int MaNV { get; set; }
        public int MaVT { get; set; }
    }
    public class ChiTietDonHang
    {
        [Key]
        public int MaDonHang { get; set; }
        public int MaDA { get; set; }
        public int SoLuong { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
    public class DoAn
    {
        [Key]
        public int MaDA { get; set; }
        public string TenDA { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public decimal DonGia { get; set; }
        public string AnhDA { get; set; }
        public string MoTa { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime NgayCapNhat { get; set; }
        public int SoLuongTon { get; set; }
        public string TrangThaiDA { get; set; }
        public short DanhGiaDoAn { get; set; }
        public int MaDM { get; set; }
    }
    public class DanhMucDoAn
    {
        [Key]
        public int MaDM { get; set; }
        public string TenDM { get; set; }
        public int MaCH { get; set; }
    }
    public class NhanVienGiaoHang
    {
        [Key]
        public int MaNV { get; set; }
        public string HoTenNV { get; set; }
        public string TaiKhoanNV { get; set; }
        public string MatKhauNV { get; set; }
        public string EmailNV { get; set; }
        public string DiaChiNV { get; set; }
        public string DienThoaiNV { get; set; }
        public DateTime NgaySinhNV { get; set; }
        public string GioiTinh { get; set; }
        public string TrangThaiNV { get; set; }
        public string ViTriHienTai { get; set; }
        public int DiemThuong { get; set; }
        public short DoUuTien { get; set; }
        public short DanhGiaNVGH { get; set; }
    }
    public class ViTriGiaoHang
    {
        [Key]
        public int MaVT { get; set; }
        public string TenVT { get; set; }
        public string Khu { get; set; }
        public string Tang { get; set; }
    }
    public class Cuahang
    {
        [Key]
        public int MaCH { get; set; }
        public string TenCH { get; set; }
        public string DiachiCH { get; set; }
        public string DienthoaiCH { get; set; }
        public string MotaCH { get; set; }
        public short DanhgiaCH { get; set; }
        public int MaDonHang { get; set; }
        public int MaKM { get; set; }
    }
    public class Khuyenmai
    {
        [Key]
        public int MaKM { get; set; }
        public string TenKM { get; set; }
        public string MotaKM { get; set; }
        public DateTime TgBatDau { get; set; }
        public DateTime TgKetThuc { get; set; }
        public int MaCH { get; set; }
    }
    public class ViTienCuaHang
    {
        [Key]
        public int MaViCH { get; set; }
        public decimal SoDu { get; set; }
        public DateTime NgayGiaoDich { get; set; }
        public decimal SoTienGiaoDich { get; set; }
        public int MaCH { get; set; }
    }
    public class LichSuGD
    {
        [Key]
        public int MaGD { get; set; }
        public string MoTa { get; set; }
        public short TinhTrang { get; set; }
        public decimal SoTienDaNap { get; set; }
        public decimal SoTienDaTieu { get; set; }
        public DateTime NgayNapTien { get; set; }
        public DateTime NgayTieuTien { get; set; }
        public int MaViTien { get; set; }
    }
    public class ViTien
    {
        [Key]
        public int MaViTien { get; set; }
        public int MaKH { get; set; }
        public int SoDu { get; set; }
    }

    public class dbHutechfoodContext : DbContext
    {   
        public DbSet<KhachHang> KhachHangs { get; set; }

        public DbSet<DonDatHang> DonDatHangs { get; set; }

        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

        public DbSet<NhanVienGiaoHang> NhanVienGiaoHangs { get; set; }

        public DbSet<ViTriGiaoHang> ViTriGiaoHangs { get; set; }

        public DbSet<DoAn> DoAns { get; set; }

        public DbSet<DanhMucDoAn> DanhMucDoAns { get; set; }

        public DbSet<Cuahang> Cuahangs { get; set; }

        public System.Data.Entity.DbSet<HTFood.Models.ViTien> ViTiens { get; set; }

        public System.Data.Entity.DbSet<HTFood.Models.LichSuGD> LichSuGDs { get; set; }
    }
}