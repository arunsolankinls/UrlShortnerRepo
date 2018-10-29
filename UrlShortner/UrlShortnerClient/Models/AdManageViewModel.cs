using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static UrlShortnerClient.Comman.CommanClass;

namespace UrlShortnerClient.Models
{
    public class AdManageViewModel
    {
        public long AdId { get; set; }
        [Required(ErrorMessage = "Please enter title.")]
        public string Title { get; set; }
        public string Description { get; set; }

        [Required(ErrorMessage = "Please upload file.")]
        public string UploadFile { get; set; }
        public int AddedBy { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime Addeddate { get; set; }
        public DateTime UpdateDate { get; set; }
        public virtual ICollection<UrlShortnerViewModel> UrlShortner { get; set; }
        public virtual ICollection<UrlShortnerHistoryViewModel> UrlShortnerHistory { get; set; }
        public long RegistAdId { get; set; }
        public virtual RegistrationViewModel Registration { get; set; }
        
        //File upload
        public HttpPostedFileBase File { get; set; }
        public string ResponseMessage { get; set; }
        public PaymentStatus Paymentstatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}