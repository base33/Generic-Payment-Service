using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GenericPaymentService
{
    /// <summary>
    /// Action result that will redirect the user to the Payment Page
    /// </summary>
    public class PaymentRequestActionResult : RedirectResult
    {
        public PaymentRequestActionResult(PaymentRequest payment)
            : base(GenerateUrl(payment))
        {
            
        }

        protected static string GenerateUrl(PaymentRequest payment)
        {
            var url = "/payment/ForwardToPaymentGateway?transactionId=" + payment.TransactionId;

            return url;
        }
    }
}
