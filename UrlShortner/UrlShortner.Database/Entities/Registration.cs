using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UrlShortner.Database.Entities
{
    [Table("Registration")]
    public class Registration
    {
        public Registration()
        {
            UrlShortner = new HashSet<UrlShortner>();
            UrlShortnerHistory = new HashSet<UrlShortnerHistory>();
            AdManages = new HashSet<AdManage>();
            PaymentTransactions = new HashSet<PaymentTransaction>();
        }
        [Key]
        public long RegistId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string MemberType { get; set; }
        public string MemberShipType { get; set; }
        public bool PaymentStatus { get; set; }
        public bool ActiveStatus { get; set; }
        public long UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now.Date;
        public DateTime ExpireDate { get; set; } = DateTime.Now.Date;
        public string Code { get; set; }
        public bool PasswordReset { get; set; }
        public virtual ICollection<UrlShortner> UrlShortner { get; set; }
        public virtual ICollection<UrlShortnerHistory> UrlShortnerHistory { get; set; }
        public virtual ICollection<AdManage> AdManages { get; set; }
        public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; }
    }
}
