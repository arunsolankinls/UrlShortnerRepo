using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UrlShortner.Database.Entities
{
    [Table("PaymentTransaction")]
    public class PaymentTransaction
    {
        [Key]
        public long PaymentId { get; set; }
        [ForeignKey("Registration")]
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
        public bool IsSuccess { get; set; }
        public string Response { get; set; }
        public int PaymentMethod { get; set; }

        //Zarinpal Response
        public string Authority { get; set; }
        public int Payment_RequestResponse { get; set; }
        public string PaymentRequest_ErrorResponse { get; set; }
        public string Status { get; set; }
        public long ReferenceID { get; set; }
        public int PaymentVerificationResponse { get; set; }
        public virtual Registration Registration { get; set; }
    }
}
