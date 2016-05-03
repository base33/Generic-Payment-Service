using GenericPaymentService.PaymentGateways;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace GenericPaymentService.Controllers
{
    /// <summary>
    /// This is the Out and In controller for payment gateways
    /// </summary>
    public class PaymentController : Controller
    {
        /// <summary>
        /// Concurrent Dictionary set up to prevent handling multiple notifications
        /// </summary>
        protected static ConcurrentDictionary<string, string> Notifications = new ConcurrentDictionary<string, string>();

        public ActionResult ForwardToPaymentGateway(string transactionId)
        {
            //save the payment request to storage
            var service = new PaymentService();
            var paymentRequest = service.GetPaymentRequest(transactionId);

            //get the payment gateway
            var paymentGateway = PaymentGatewayDependancyContainer.GetPaymentGateway();

            //call the payment provider to get the next action result to proceed to payment
            return paymentGateway.ProceedToPayment(paymentRequest, "/payment/HandlePaymentResponse");
        }

        public ActionResult HandlePaymentCallback()
        {
            //get request data
            Dictionary<string, string> responseData = GetRequestData(Request);

            //declare the action result that should be set inside the payment provider (via out parameter)
            ActionResult actionResult = null;

            //get the payment gateway
            var paymentGateway = PaymentGatewayDependancyContainer.GetPaymentGateway();

            //call the payment provider to handle the callback
            var paymentResponse = paymentGateway.HandleCallback(responseData, out actionResult);

            //add the transaction id to the notifications dictionary, to prevent handling multiple callbacks
            if(Notifications.TryAdd(paymentResponse.TransactionId, "Processing/Processed"))
            {
                var service = new PaymentService();
                //update the payment status for the transaction and that we have been notified
                service.UpdatePaymentStatus(paymentResponse.TransactionId, paymentResponse.Success, true);

                //get the payment request from storage for this transaction
                var request = service.GetPaymentRequest(paymentResponse.TransactionId);

                //call the PaymentMadeCallback handler on the original soure
                request.PaymentMadeCallback(request.TransactionId, paymentResponse.Success, request.AdditionalData);
            }

            return actionResult;
        }

        public ActionResult HandlePaymentRedirect()
        {
            //get request data
            Dictionary<string, string> responseData = GetRequestData(Request);

            //get the payment gateway
            var paymentGateway = PaymentGatewayDependancyContainer.GetPaymentGateway();

            //call the payment provider to handle the end result
            var paymentResponse = paymentGateway.HandleEndResult(responseData);

            var service = new PaymentService();
            //get the payment request for the current transaction
            var request = service.GetPaymentRequest(paymentResponse.TransactionId);

            //check if we haven't been notified about the payment yet (support for payment systems without callbacks)
            if (!service.HaveWeBeenNotified(paymentResponse.TransactionId))
            {
                //update the payment status for the transaction and they we have now been notified
                service.UpdatePaymentStatus(paymentResponse.TransactionId, paymentResponse.Success, true);

                //call the PaymentMadeCallback method on the original source
                request.PaymentMadeCallback(paymentResponse.TransactionId, paymentResponse.Success, request.AdditionalData);
            }

            //call NextActionCallback on the original source to know where to forward the browser
            return request.NextActionCallback(request.TransactionId, paymentResponse.Success, request.AdditionalData);
        }

        protected Dictionary<string, string> GetRequestData(HttpRequestBase request)
        {
            //get the correct data from either body or uri depending on the http method
            if (Request.HttpMethod.Equals("POST", StringComparison.InvariantCultureIgnoreCase))
            {
                return Request.Form.Cast<string>()
                    .Select(c => new KeyValuePair<string, string>(c, Request.Form[c]))
                    .ToDictionary(c => c.Key, c => c.Value);
            }
            else
            {
                return Request.QueryString.Cast<string>()
                    .Select(c => new KeyValuePair<string, string>(c, Request.Form[c]))
                    .ToDictionary(c => c.Key, c => c.Value);
            }
        }
    }
}
