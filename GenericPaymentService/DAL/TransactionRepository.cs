using GenericPaymentService.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericPaymentService.DAL
{
    public class TransactionRepository : EntityRepository<PaymentTransactionEntities, PaymentTransaction>
    {

    }
}
