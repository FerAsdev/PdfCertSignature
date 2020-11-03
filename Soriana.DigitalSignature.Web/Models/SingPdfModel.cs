using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Soriana.DigitalSignature.Web.Models
{
    public class SingPdfModel
    {
        [Required]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase PdfFileBase { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase CertFileBase { get; set; }

        [Required]
        public string Password { get; set; }

        public string IsSignVisible { get; set; }

    }
}