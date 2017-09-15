using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
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
                reportDataSource2.Value = db.mOperations.ToList();
            }
            
            localReport.DataSources.Add(reportDataSource2);

            string reportType = "Image";
            string mimeType;
            string encoding;
            string fileNameExtension;

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report

            if (format == null)
            {
                renderedBytes = localReport.Render(reportType, null, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
                Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);

                return File(renderedBytes, "image/jpeg");
            }
            else if (format.ToUpper() == "PDF")
            {
                renderedBytes = localReport.Render("pdf", null, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
                Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers.pdf");

                return File(renderedBytes, "pdf");
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
    }
}