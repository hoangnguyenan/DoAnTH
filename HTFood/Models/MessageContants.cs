using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTFood.Models
{
    public class MessageContants
    {
        //lich su nhân vien giao hang
        public static string STATUS_BILL_SUCCESSFUL = "THÀNH CÔNG";
        public static string STATUS_BILL_FAIL = "THẤT BẠI";
        public static string STATUS_SHIP_SUCCESSFUL= "GIAO HÀNG THÀNH CÔNG";
        public static string STATUS_SHIP_IN_PROCCESING = "ĐANG GIAO HÀNG";
        public static string STATUS_SHIP_BOOM = "BOOM HÀNG";
        public static string STATUS_SHIP_FAIL = "THẤT BẠI";
        //don dat hang
        public static string STATUS_ORDER_CONFIRM = "NGƯỜI BÁN ĐÃ XÁC NHẬN";
        public static string STATUS_ORDER_DELIVERY = "ĐANG GIAO HÀNG ";
        public static string STATUS_ORDER_DELIVERED = "SHIPPER ĐÃ GIAO HÀNG";
        public static string STATUS_ORDER_BOOM = "BOOM HÀNG";
    }
}