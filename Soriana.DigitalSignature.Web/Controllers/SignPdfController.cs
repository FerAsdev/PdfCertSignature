using Soriana.DigitalSignature.Web.Helpers;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Soriana.DigitalSignature.Web.Controllers
{
    public class SignPdfController : Controller
    {
        // GET: SignPdf
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignPdf(HttpPostedFileBase pdfFile, HttpPostedFileBase certFile,
            HttpPostedFileBase signImage, string certPassword, string signVisible, string signatrueRenderType,
            string signLocation)
        {
            try
            {
                if (pdfFile != null && pdfFile.ContentLength > 0 &&
                    certFile != null && certFile.ContentLength > 0 &&
                    signImage != null && signImage.ContentLength > 0)
                {
                    var pdfMimeType = MimeMapping.GetMimeMapping(pdfFile.FileName);
                    var certMimeType = MimeMapping.GetMimeMapping(certFile.FileName);
                    MimeMapping.GetMimeMapping(signImage.FileName);

                    if (pdfMimeType != "application/pdf")
                        return new JsonResult()
                        {
                            Data = new
                            {
                                ErrorCode = "1",
                                ErrorDesc = "El archivo no es un PDF"
                            }
                        };

                    if (certMimeType != "application/x-pkcs12")
                        return new JsonResult()
                        {
                            Data = new
                            {
                                ErrorCode = "2",
                                ErrorDesc = "Archivo no es un certificado valido"
                            }
                        };

                    //if (signImageMimeType != "")
                    //    return new JsonResult()
                    //    {
                    //        Data = new
                    //        {
                    //            ErrorCode = "3",
                    //            ErrorDesc = "Imagén de Firma no valida"
                    //        }
                    //    };

                    var pdfMemoryStream = new MemoryStream();
                    pdfFile.InputStream.CopyTo(pdfMemoryStream);
                    var pdfBytes = pdfMemoryStream.ToArray();

                    var certMemoryStream = new MemoryStream();
                    certFile.InputStream.CopyTo(certMemoryStream);
                    var certBytes = certMemoryStream.ToArray();

                    var signImageMemoryStream = new MemoryStream();
                    signImage.InputStream.CopyTo(signImageMemoryStream);
                    var signImageBytes = signImageMemoryStream.ToArray();

                    var isSignVisible = signVisible == "1";

                    var signedPdfBytes = SignPdfHelper.SignPdf(pdfBytes, certBytes, signImageBytes, certPassword,
                        isSignVisible, signatrueRenderType, signLocation);

                    var fileResult = new MemoryStream(signedPdfBytes);

                    // Generate a new unique identifier against which the file can be stored
                    var handle = Guid.NewGuid().ToString();
                    TempData[handle] = fileResult.ToArray();

                    return new JsonResult()
                    {
                        Data = new
                        {
                            ErrorCode = "0",
                            ErrorDesc = "",
                            FileGuid = handle,
                            FileName = "signed_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + pdfFile.FileName
                        }
                    };

                    //return File(fileResult, "application/pdf", "signed_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + pdfFile.FileName);
                }

                return new JsonResult()
                {
                    Data = new
                    {
                        ErrorCode = "-1",
                        ErrorDesc = "Archivo no valido"
                    }
                };
            }
            catch (Exception ex)
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        ErrorCode = "-1",
                        ErrorDesc = ex.Message
                    }
                };
            }
        }

        [HttpGet]
        public virtual ActionResult DownloadSignedPdf(string fileGuid, string fileName)
        {
            if (TempData[fileGuid] != null)
            {
                var data = TempData[fileGuid] as byte[];
                return File(data, "application/pdf", fileName);
            }

            // Problem - Log the error, generate a blank file,
            //           redirect to another controller action - whatever fits with your application
            return new EmptyResult();
        }
    }
}