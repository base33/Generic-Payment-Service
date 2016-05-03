using GenericPaymentService.Crypto;
using GenericPaymentService.DAL;
using GenericPaymentService.DAL.Entities;
using GenericPaymentService.Interfaces;
using GenericPaymentService.PaymentGateways;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericPaymentService
{
    public class PaymentService
    {
        /// <summary>
        /// Generates a payment request with a unique transaction id
        /// </summary>
        /// <returns></returns>
        public PaymentRequest CreatePaymentRequest()
        {
            return new PaymentRequest()
            {
                TransactionId = EncryptionHelper.GenerateUniqueString(50)
            };
        }

        public void CommitPaymentRequest(PaymentRequest paymentRequest)
        {
            using(var repo = new TransactionRepository())
            {
                var transaction = new PaymentTransaction();
                transaction.TransactionId = paymentRequest.TransactionId;
                transaction.Amount = paymentRequest.Amount;
                transaction.PaymentRequest = paymentRequest.Encrypt();
                transaction.ProductType = paymentRequest.ProductType;
                transaction.Paid = false;
                transaction.Notified = false;

                repo.Insert(transaction);
                repo.Save();
            }
        }

        public PaymentRequest GetPaymentRequest(string transactionId)
        {
            using (var repo = new TransactionRepository())
            {
                string encryptedPaymentRequest = repo.Get(t => t.TransactionId == transactionId).PaymentRequest;
                return PaymentRequest.Decrypt(encryptedPaymentRequest);
            }
        }

        internal bool HaveWeBeenNotified(string transactionId)
        {
            using(var repo = new TransactionRepository())
            {
                var transaction = repo.Get(t => t.TransactionId == transactionId);
                return transaction.Notified;
            }
        }

        public void UpdatePaymentStatus(string transactionId, bool paymentTaken, bool? notified = null)
        {
            using(var repo = new TransactionRepository())
            {
                var transaction = repo.Get(t => t.TransactionId == transactionId);
                transaction.Paid = paymentTaken;

                if (notified.HasValue)
                    transaction.Notified = notified.Value;

                repo.Update(transaction);
                repo.Save();
            }
        }

        public static void RegisterPaymentGateway<T>() where T : IPaymentGateway, new()
        {
            PaymentGatewayDependancyContainer.PaymentGateway = typeof(T);
        }
    }
}
