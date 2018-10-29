using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortner.Database.Entities;


namespace UrlShortner.Database.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        public ShortnerContext _ctx;

        public PaymentRepository(ShortnerContext ctx)
        {
            this._ctx = ctx;
        }

        public void AddPayment(PaymentTransaction payment)
        {
            _ctx.PaymentTransaction.Add(payment);
            SaveAll();
        }

        public bool SaveAll()
        {
            return _ctx.SaveChanges() > 0;
        }

        public bool CheckIsPaidByRegisterId(long registId)
        {
           var payment= _ctx.PaymentTransaction.Where(x => x.RegistId == registId).LastOrDefault();

           if (payment != null && payment.IsSuccess==true)
               return true;

            return false;
        }

        public PaymentTransaction GetPaymentByRegistId(long registId)
        {
            return _ctx.PaymentTransaction.Where(x => x.RegistId == registId).OrderByDescending(x=>x.PaymentId).FirstOrDefault();
        }

        public void Edit(PaymentTransaction payment)
        {
            _ctx.Entry(payment).State = EntityState.Modified;
            _ctx.SaveChanges();
        }

        public void InsertPaymentTransactionData(PaymentTransaction paymentdata)
        {
            try
            {
                PaymentTransaction paymenttransaction = new PaymentTransaction();
                paymenttransaction.RegistId = paymentdata.RegistId;
                paymenttransaction.Customer_Created = DateTime.Now;
                paymenttransaction.Charge_Created = DateTime.Now;
                paymenttransaction.Charge_Amount = Convert.ToInt32(paymentdata.Charge_Amount);
                paymenttransaction.Charge_Paid = paymentdata.Charge_Paid;
                paymenttransaction.UpdatedDate = DateTime.Now;
                paymenttransaction.ExpireDate = DateTime.Now.AddDays(30);
                paymenttransaction.PaymentMethod = paymentdata.PaymentMethod;
                paymenttransaction.Response = paymentdata.Response;
                paymenttransaction.IsSuccess = paymentdata.IsSuccess;
                paymenttransaction.Authority = paymentdata.Authority;
                paymenttransaction.Payment_RequestResponse = paymentdata.Payment_RequestResponse;
                paymenttransaction.PaymentRequest_ErrorResponse = paymentdata.PaymentRequest_ErrorResponse;
                paymenttransaction.Status = paymentdata.Status;
                paymenttransaction.ReferenceID = paymentdata.ReferenceID;
                paymenttransaction.PaymentVerificationResponse = paymentdata.PaymentVerificationResponse;

                _ctx.PaymentTransaction.Add(paymenttransaction);
                _ctx.SaveChanges();
            }
            catch(Exception ex)
            {

            }
        }

        public void UpdatePaymentTransactionData(Entities.PaymentTransaction paymentdata)
        {
            var paymenttransaction = _ctx.PaymentTransaction.Find(paymentdata.PaymentId);
            if (paymenttransaction != null)
            {
                //PaymentTransaction paymenttransaction = new PaymentTransaction();
                paymenttransaction.PaymentId = paymentdata.PaymentId;
                paymenttransaction.RegistId = paymentdata.RegistId;
                paymenttransaction.Charge_Amount = paymentdata.Charge_Amount;
                paymenttransaction.Charge_Paid = paymentdata.Charge_Paid;
                paymenttransaction.UpdatedDate = DateTime.Now;
                paymenttransaction.ExpireDate = DateTime.Now.AddDays(30);
                paymenttransaction.IsSuccess = paymentdata.IsSuccess;
                paymenttransaction.PaymentMethod = paymentdata.PaymentMethod;
                paymenttransaction.Response = paymentdata.Response;

                paymenttransaction.Authority = paymentdata.Authority;
                paymenttransaction.Payment_RequestResponse = paymentdata.Payment_RequestResponse;
                paymenttransaction.PaymentRequest_ErrorResponse = paymentdata.PaymentRequest_ErrorResponse;
                paymenttransaction.Status = paymentdata.Status;
                paymenttransaction.ReferenceID = paymentdata.ReferenceID;
                paymenttransaction.PaymentVerificationResponse = paymentdata.PaymentVerificationResponse;

                _ctx.PaymentTransaction.Attach(paymenttransaction);
                _ctx.Entry(paymenttransaction).State = EntityState.Modified;
                _ctx.SaveChanges();
            }
        }
    }
}
