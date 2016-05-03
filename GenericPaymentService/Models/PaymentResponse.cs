using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericPaymentService.Models
{
    public class PaymentResponse
    {
        public string TransactionId { get; set; }
        public bool Success { get; set; }
    }
}
