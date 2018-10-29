using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UrlShortnerClient.LocalResource;

namespace UrlShortnerClient.Models
{
    public class RegistrationViewModel
    {
        public long RegistId { get; set; }

        [Required(ErrorMessageResourceName = "UserNameIsRequired", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Firstname", ResourceType = typeof(Resource))]
        public string FirstName { get; set; }
        [Display(Name = "Lastname", ResourceType = typeof(Resource))]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceName = "EmailIsRequired", ErrorMessageResourceType = typeof(Resource))]
        [EmailAddress(ErrorMessageResourceName = "PleaseEnterValidEmail", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Email", ResourceType = typeof(Resource))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "Passwordisrequired",ErrorMessageResourceType =typeof(Resource))]
        [Display(Name ="Password",ResourceType =typeof(Resource))]
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string MemberType { get; set; }
        public string MemberShipType { get; set; }
        public bool ActiveStatus { get; set; }
        public long UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}