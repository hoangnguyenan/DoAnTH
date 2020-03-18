using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTFood.Models
{
    public class Constants
    {
        public static string url = "http://localhost/hutechfoodserver/api/";
        public static int timeShiper = 5;

        // STATUS
        public static int STATUS_SHIP_SUCCESSFUL = 1;
        public static int STATUS_SHIP_IN_PROCCESING = 2;
        public static int STATUS_SHIP_BOOM = 3;
        public static int STATUS_SHIP_FAIL = 4;
        //Order
        public static int STATUS_ORDER_CONFIRM = 1;
        public static int STATUS_ORDER_DELIVERY = 2;
        public static int STATUS_ORDER_DELIVERD = 3;
        public static int STATUS_ORDER_BOOM = 4;
    }
}