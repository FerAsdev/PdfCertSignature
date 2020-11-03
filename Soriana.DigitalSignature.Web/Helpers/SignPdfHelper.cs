using System;
using System.IO;
using System.Security.Cryptography;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Security.Cryptography.X509Certificates;
using SysX509 = System.Security.Cryptography.X509Certificates;

namespace Soriana.DigitalSignature.Web.Helpers
{
    public static class SignPdfHelper
    {
        public static byte[] SignPdf(byte[] pdf, byte[] cert, byte[] firma, string certPassword, bool isSignVisible,
            string signatureRender, string signaturePosition)
        {
            using (MemoryStream output = new MemoryStream())
            {
                SysX509.X509Certificate2 cert1 = new SysX509.X509Certificate2(cert, certPassword, SysX509.X509KeyStorageFlags.MachineKeySet |
                    SysX509.X509KeyStorageFlags.PersistKeySet | SysX509.X509KeyStorageFlags.Exportable | SysX509.X509KeyStorageFlags.UserKeySet);

                //using (CngKey certKey = cert1.GetCngPrivateKey())
                //{
                //    //get private key to sign pdf
                //    var privateKey1 = certKey;
                //}

                //get private key to sign pdf
                var privateKey = Org.BouncyCastle.Security.DotNetUtilities.GetKeyPair(cert1.PrivateKey).Private;
                
                // convert the type to be used at .SetCrypt();
                Org.BouncyCastle.X509.X509Certificate bcCert = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(cert1);

                // get the pdf u want to sign
                PdfReader pdfReader = new PdfReader(pdf);
                PdfStamper stamper = PdfStamper.CreateSignature(pdfReader, output, '\0');
                PdfSignatureAppearance pdfSignatureAppearance = stamper.SignatureAppearance;

                //.SetCrypt(); sign the pdf
                pdfSignatureAppearance.SetCrypto(privateKey, new Org.BouncyCastle.X509.X509Certificate[] { bcCert },
                    null, PdfSignatureAppearance.WINCER_SIGNED);

                //pdfSignatureAppearance.Reason = "";
                //pdfSignatureAppearance.Location = "";
                pdfSignatureAppearance.SignDate = DateTime.Now;

                if (isSignVisible)
                {
                    Image image = Image.GetInstance(firma);
                    Rectangle rectangle;

                    switch (signatureRender)
                    {
                        //Graphic
                        default:
                            pdfSignatureAppearance.Acro6Layers = true;
                            pdfSignatureAppearance.SignatureGraphic = image;
                            pdfSignatureAppearance.Render = PdfSignatureAppearance.SignatureRender.Graphic;
                            rectangle = GetRectangleSign(pdfReader, signaturePosition);
                            pdfSignatureAppearance.SetVisibleSignature(rectangle, 1, null);
                            break;

                        //GraphicAndDescription
                        case "1":
                            pdfSignatureAppearance.Acro6Layers = true;
                            pdfSignatureAppearance.SignatureGraphic = image;
                            pdfSignatureAppearance.Render = PdfSignatureAppearance.SignatureRender.GraphicAndDescription;
                            rectangle = GetRectangleSign(pdfReader, signaturePosition);
                            pdfSignatureAppearance.SetVisibleSignature(rectangle, 1, null);
                            break;
                    }
                }

                //Firma no visible y info certificado true.
                if (!isSignVisible && signatureRender == "1")
                {
                    Rectangle rectangle;
                    pdfSignatureAppearance.Acro6Layers = true;
                    pdfSignatureAppearance.Render = PdfSignatureAppearance.SignatureRender.Description;
                    rectangle = GetRectangleSign(pdfReader, signaturePosition);
                    pdfSignatureAppearance.SetVisibleSignature(rectangle, 1, null);
                }

                stamper.Close();
                return output.ToArray();
            }
        }

        private static Rectangle GetRectangleSign(PdfReader pdfReader, string signaturePosition)
        {
            //Sign Width and Height
            float width = 150;
            float height = 50;
            float margin = 50;

            Rectangle rectangle;
            Rectangle pageSize = pdfReader.GetPageSize(1);

            switch (signaturePosition)
            {
                //top left
                case "TL":
                    rectangle = new Rectangle(pageSize.GetLeft(margin), pageSize.GetTop(height + margin),
                                              pageSize.GetLeft(width), pageSize.GetTop(margin));
                    return rectangle;

                //top right
                case "TR":
                    rectangle = new Rectangle(pageSize.GetRight(width), pageSize.GetTop(height + margin),
                                              pageSize.GetRight(margin), pageSize.GetTop(margin));
                    return rectangle;

                //bottom left
                case "BL":
                    rectangle = new Rectangle(pageSize.GetLeft(margin), pageSize.GetBottom(margin),
                                              pageSize.GetLeft(width), pageSize.GetBottom(height + margin));
                    return rectangle;

                //bottom right
                case "BR":
                    rectangle = new Rectangle(pageSize.GetRight(width), pageSize.GetBottom(margin),
                                              pageSize.GetRight(margin), pageSize.GetBottom(height + margin));
                    return rectangle;

                //bottom center
                case "CN":
                    rectangle = new Rectangle(pageSize.GetLeft(pageSize.Width - 150), margin,
                                              pageSize.GetLeft(150), pageSize.GetBottom(50 + margin));
                    return rectangle;
                default:
                    rectangle = new Rectangle(pageSize.GetLeft(pageSize.Width - 150), margin,
                                              pageSize.GetLeft(150), pageSize.GetBottom(50 + margin));


                    return rectangle;
            }
        }
    }


}