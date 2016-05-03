using Newtonsoft.Json;
using GenericPaymentService.Crypto;
using GenericPaymentService.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GenericPaymentService
{
    public delegate void PaymentMadeCallback(string transactionId, bool success, Dictionary<string, string> additionalData);
    public delegate ActionResult NextActionCallback(string transactionId, bool success, Dictionary<string, string> additionalData);

    public class PaymentRequest
    {
        protected const string PASSWORD = "446ed910-11c7-4b81-abc9-57f5cba580ce";

        /// <summary>
        /// The campaign Id for the payment
        /// </summary>
        public string CampaignId { get; set; }

        /// <summary>
        /// The amount to charge
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The unique transaction Id
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Additional data to return in callback
        /// </summary>
        public Dictionary<string, string> AdditionalData { get; set; }

        /// <summary>
        /// Product Description (product name for payment gateway)
        /// </summary>
        public string ProductDescription { get; set; }

        /// <summary>
        /// The product type - so you know what product the transaction relates to (Event, Donation etc)
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// Details of the payee who will be purchasing.  If supported, would be used for prefilling information on the payment page
        /// </summary>
        public PayeeDetails PayeeDetails { get; set; }

        /// <summary>
        /// The payment result callback method that is called when a payment is made
        /// Param 1: transaction Id
        /// Param 2: success
        /// </summary>
        [JsonIgnore]
        public PaymentMadeCallback PaymentMadeCallback { get; set; }

        /// <summary>
        /// The callback method to get the action based on result
        /// Param 1: transaction Id
        /// Param 2: success
        /// </summary>
        [JsonIgnore]
        public NextActionCallback NextActionCallback { get; set; }

        internal PaymentRequest()
        {
            PayeeDetails = new PayeeDetails();
        }

        public string Encrypt()
        {
            var data = new Dictionary<string, string>();
            var delegateSerialiser = new DelegateSerializer.Serializer();
            data.Add("Data", JsonConvert.SerializeObject(this));
            data.Add("PaymentMadeCallback", delegateSerialiser.Serialize(PaymentMadeCallback));
            data.Add("NextActionCallback", delegateSerialiser.Serialize(NextActionCallback));
            return EncryptionHelper.EncryptDictionary(data, PASSWORD);
        }

        public static PaymentRequest Decrypt(string value)
        {
            var data = EncryptionHelper.DecryptDictionary(value, PASSWORD);
            var delegateSerialiser = new DelegateSerializer.Serializer();
            var paymentRequest = JsonConvert.DeserializeObject<PaymentRequest>(data["Data"]);
            paymentRequest.PaymentMadeCallback = (PaymentMadeCallback)delegateSerialiser.Deserialize<PaymentMadeCallback>(data["PaymentMadeCallback"]);
            paymentRequest.NextActionCallback = (NextActionCallback)delegateSerialiser.Deserialize<NextActionCallback>(data["NextActionCallback"]);
            return paymentRequest;
        }
    }
}
