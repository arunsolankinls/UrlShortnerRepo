using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UrlShortnerClient.LocalResource;

namespace UrlShortnerClient.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceName = "EmailIsRequired", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name ="Email",ResourceType =typeof(Resource))]
        [EmailAddress(ErrorMessageResourceName = "PleaseEnterValidEmail", ErrorMessageResourceType = typeof(Resource))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "Passwordisrequired", ErrorMessageResourceType =typeof(Resource))]
        [Display(Name ="Password",ResourceType =typeof(Resource))]
        public string Password { get; set; }        
    }
}