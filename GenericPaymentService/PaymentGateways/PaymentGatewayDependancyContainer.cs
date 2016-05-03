using GenericPaymentService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericPaymentService.PaymentGateways
{
    internal class PaymentGatewayDependancyContainer
    {
        /// <summary>
        /// Holds a single payment gateway type.  This can be extended to support more, but I would recommend adding a method to IPaymentGateway 
        /// that will allow the Payment System to choose which payment gateway to use (possibly by product type?)
        /// </summary>
        public static Type PaymentGateway { get; set; }

        public static void RegisterPaymentGateway<T>() where T  : IPaymentGateway, new()
        {
            PaymentGateway = typeof(T);
        }

        public static IPaymentGateway GetPaymentGateway()
        {
            //currently doing nothing with PaymentRequest - but in the future, we can add logic here to choose what payment gateway to use
            //so that we can support multiple payment gateways
            return (IPaymentGateway)Activator.CreateInstance(PaymentGateway);
        }
    }
}
