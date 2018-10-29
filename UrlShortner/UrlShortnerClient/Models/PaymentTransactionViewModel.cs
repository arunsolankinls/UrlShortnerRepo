using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UrlShortnerClient.Models
{
    public class PaymentTransactionViewModel
    {
        public long PaymentId { get; set; }
        public long RegistId { get; set; }

        public string Customer_stripeEmail { get; set; }
        public string Customer_stripeToken { get; set; }

        public string Customer_Id { get; set; }
        public DateTime Customer_Created { get; set; }
        public string Customer_DefaultSourceId { get; set; }
        public string Customer_InvoicePrefix { get; set; }
        public string Customer_StripeResponse_ResponseJson { get; set; }
        public string Customer_StripeResponse_RequestId { get; set; }

        //Stripe Charge
        public string Charge_Id { get; set; }
        public string Charge_Description { get; set; }
        public string Charge_CustomerId { get; set; }
        public DateTime Charge_Created { get; set; }
        public string Charge_BalanceTransactionId { get; set; }
        public int Charge_Amount { get; set; }
        public bool Charge_Paid { get; set; }
        public string Charge_FailureCode { get; set; }
        public string Charge_Outcome_SellerMessage { get; set; }
        public string Charge_Source_Id { get; set; }
        public string Charge_Status { get; set; }
        public string Charge_StripeResponse_RequestId { get; set; }
        public string Charge_StripeResponse_ResponseJson { get; set; }
        public string Charge_FailureMessage { get; set; }

        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public DateTime ExpireDate { get; set; } = DateTime.Now;

        public virtual RegistrationViewModel RegistrationViewModel { get; set; }

    }
}