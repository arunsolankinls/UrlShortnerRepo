using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortner.Database.Entities;

namespace UrlShortner.Database.Repository
{
    public interface IPaymentRepository
    {
        void AddPayment(PaymentTransaction payment);
        bool SaveAll();
        void Edit(PaymentTransaction payment);
        PaymentTransaction GetPaymentByRegistId(long registId);
        void InsertPaymentTransactionData(PaymentTransaction paymentdata);
        void UpdatePaymentTransactionData(PaymentTransaction paymentdata);
    }
}
