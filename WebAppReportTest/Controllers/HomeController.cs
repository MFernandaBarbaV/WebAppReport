using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WebAppReportTest.Models;

namespace WebAppReportTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ReportViewer(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ViewResult ReportViewer(SearchParameterModel um)
        {
            return View("Index", um);
        }

        private IList<Stream> m_streams = new List<Stream>();


        public FileContentResult GenerateAndDisplayReport(string territory, string format)
        {
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Content/Report1.rdlc");

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            using (Entities1 db = new Entities1())
            {
                if (territory != null)
                    reportDataSource.Value = db.mUsers.Where(u => u.UserID.ToString() == territory).ToList();
                else
                    reportDataSource.Value = db.mUsers.ToList();
            }

            localReport.DataSources.Add(reportDataSource);

            ReportDataSource reportDataSource2 = new ReportDataSource();
            reportDataSource2.Name = "DataSet2";
            using (Entities1 db = new Entities1())
            {
                if (territory != null)
                    reportDataSource2.Value = db.mOperations.Where(u => u.OperationID.ToString() == territory).ToList();
                else
                    reportDataSource2.Value = db.mOperations.ToList();
             
            }

            localReport.DataSources.Add(reportDataSource2);

            string reportType = "Image";
            string mimeType;
            string encoding;
            string fileNameExtension;

            Warning[] warnings;
            string[] streams = new string[10];
            byte[] renderedBytes = null;

            string deviceInfo =
        "<DeviceInfo>" +
          "  <OutputFormat>EMF</OutputFormat>" +
          "  <PageWidth>8.5in</PageWidth>" +
          "  <PageHeight>11in</PageHeight>" +
          "  <MarginTop>0.25in</MarginTop>" +
          "  <MarginLeft>0.25in</MarginLeft>" +
          "  <MarginRight>0.25in</MarginRight>" +
          "  <MarginBottom>0.25in</MarginBottom>" +
          "</DeviceInfo>";

            //Render the report

            if (format == null)
            {
                renderedBytes = localReport.Render(reportType, null, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
                //   renderedBytes = localReport.Render("Image", deviceInfo, PageCountMode.Estimate, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

                //  localReport.Render("Image", deviceInfo, CreateStream, out warnings);
                Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers.tif");

                //while (m_streams.Count() == 0)
                //    Thread.Sleep(50);

                //Image imageFile = Image.FromFile("Report1_0.EMF");
                //Graphics newGraphics = Graphics.FromImage(imageFile);
                //newGraphics.FillRectangle(new SolidBrush(Color.Black), 100, 50, 100, 100);

                //for (int i = 0; i < m_streams.Count; i++)
                //{

                //    m_streams[i].Flush();
                //    m_streams[i].Close();
                //    m_streams[i].Dispose();

                //    newGraphics.DrawImage(new Bitmap("Report1_" + i + ".EMF"), new PointF(0.0F, 0.0F));


                //}

                //newGraphics.Save();

                int pages = localReport.GetTotalPages();

                return File(renderedBytes, "image/tiff");

            }
            else if (format.ToUpper() == "PDF")
            {
                renderedBytes = localReport.Render("pdf", null, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
                //   Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers.pdf");

                Response.AddHeader("content-disposition", "inline; filename=MyFile.pdf");



                return File(renderedBytes, "application/pdf");
            }
            else if (format.ToUpper() == "EXCEL")
            {
                renderedBytes = localReport.Render("Excel", null, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
                Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers.xls");

                return File(renderedBytes, "application/vnd.ms-excel");
            }
            else
            {
                renderedBytes = localReport.Render(reportType, null, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
                return File(renderedBytes, "image/jpeg");
            }
        }


        public ActionResult GenerateAndDisplayReportPDF(string parameter)
        {
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Content/Report1.rdlc");

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            using (Entities1 db = new Entities1())
            {
                if (parameter != null)
                    reportDataSource.Value = db.mUsers.Where(u => u.UserID.ToString() == parameter).ToList();
                else
                    reportDataSource.Value = db.mUsers.ToList();
            }

            localReport.DataSources.Add(reportDataSource);

            ReportDataSource reportDataSource2 = new ReportDataSource();
            reportDataSource2.Name = "DataSet2";
            using (Entities1 db = new Entities1())
            {
                reportDataSource2.Value = db.mOperations.ToList();
            }

            localReport.DataSources.Add(reportDataSource2);

            string reportType = "Image";
            string mimeType;
            string encoding;
            string fileNameExtension;

            Warning[] warnings;
            string[] streams = new string[10];
            byte[] renderedBytes = null;
            //Render the report
            renderedBytes = localReport.Render("pdf", null, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);          
            Response.Clear();
            Response.AddHeader("Content - Disposition","inline; filename = sample.pdf");
            Response.AddHeader("Content - Type","application / pdf");
            Response.ClearHeaders();
            Response.AddHeader("Content - Length", renderedBytes.Length.ToString());
            return new FileContentResult(renderedBytes, "application/pdf");
        }
        private Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            Stream stream = new FileStream(name + "." + fileNameExtension, FileMode.OpenOrCreate);
            m_streams.Add(stream);
            return stream;
        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}