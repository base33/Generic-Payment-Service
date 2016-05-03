using GenericPaymentService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GenericPaymentService.Interfaces
{
    /// <summary>
    /// Interface for an implementation of a Payment Gateway
    /// </summary>
    public interface IPaymentGateway
    {
        /// <summary>
        /// Prepares the payment and returns an action that will send the user to the payment screen
        /// </summary>
        /// <param name="payment">The payment request</param>
        /// <param name="paymentResponseUrl">The url to return to, if supported</param>
        /// <returns>An action result that will forward the user to the payment screen</returns>
        ActionResult ProceedToPayment(PaymentRequest payment, string paymentResponseUrl);

        /// <summary>
        /// Handles a Server-to-Server callback - used if supported
        /// </summary>
        /// <param name="response">The response parameters from the URI or Body sent from the payment gateway</param>
        /// <param name="actionResult">What data to return to the payment gateway after callback</param>
        /// <returns>The payment response - including transaction Id and whether payment taken</returns>
        PaymentResponse HandleCallback(Dictionary<string, string> response, out ActionResult actionResult);

        /// <summary>
        /// Handles when the users browser is redirected back to the web site
        /// </summary>
        /// <param name="response">The response parameters from the URI or Body sent from the payment gateway</param>
        /// <returns>The payment response - including transaction Id and whether payment taken</returns>
        PaymentResponse HandleEndResult(Dictionary<string, string> response);
    }
}
