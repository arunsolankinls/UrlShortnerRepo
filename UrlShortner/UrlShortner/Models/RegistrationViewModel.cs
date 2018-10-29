using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UrlShortner.Database.Entities;
using UrlShortner.LocalResource;

namespace UrlShortner.Models
{
    public class RegistrationViewModel
    {
        public RegistrationViewModel() {
            AdManages = new HashSet<AdManage>();
            PaymentTransactions = new HashSet<PaymentTransaction>();
        }

        public long RegistId { get; set; }
        [Display(Name = "FirstName", ResourceType = typeof(Resource))]
        public string FirstName { get; set; }
        [Display(Name = "LastName", ResourceType = typeof(Resource))]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceName = "EmailIsRequired", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Email", ResourceType = typeof(Resource))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "PasswordIsRequired", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Password", ResourceType = typeof(Resource))]
        public string Password { get; set; }
        [Display(Name = "Phone", ResourceType = typeof(Resource))]
        public string Phone { get; set; }
        [Display(Name = "Address", ResourceType = typeof(Resource))]
        public string Address { get; set; }
        [Display(Name = "City", ResourceType = typeof(Resource))]
        public string City { get; set; }
        [Display(Name = "Country", ResourceType = typeof(Resource))]
        public string Country { get; set; }
        [Display(Name = "State", ResourceType = typeof(Resource))]
        public string State { get; set; }
        [Display(Name = "Zipcode", ResourceType = typeof(Resource))]
        public string Zipcode { get; set; }
        public string MemberType { get; set; }
        public string MemberShipType { get; set; }
        public bool ActiveStatus { get; set; }
        public long UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime ExpireDate { get; set; }

        public virtual ICollection<AdManage> AdManages { get; set; }
        public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; }
    }
}