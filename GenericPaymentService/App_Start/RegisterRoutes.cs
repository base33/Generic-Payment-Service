using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web.Mvc;
using GenericPaymentService.App_Start;
using System.Web;

[assembly:PreApplicationStartMethod(typeof(RegisterRoutes), "RegisterRoute")]
namespace GenericPaymentService.App_Start
{
    public class RegisterRoutes
    {
        public static void RegisterRoute()
        {
            RouteTable.Routes.MapRoute("PaymentResponse", "payment/{action}",
                new { controller = "Payment", action = "Index" });
        }
    }
}
