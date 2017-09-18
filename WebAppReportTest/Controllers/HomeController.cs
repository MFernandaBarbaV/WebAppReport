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

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ViewResult ReportViewer(SearchParameterModel model)
        {
            return View("Index", model);
        }

        public FileContentResult GenerateAndDisplayReportExcel(string parameter)
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
                if (parameter != null)
                    reportDataSource2.Value = db.mOperations.Where(u => u.OperationID.ToString() == parameter).ToList();
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
            
            //Render the report

            renderedBytes = localReport.Render("Excel", null, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers.xls");

            return File(renderedBytes, "application/vnd.ms-excel");
        }
        
        public FileContentResult GenerateAndDisplayReportPDF(string parameter)
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
            Response.AddHeader("Content - Disposition", "inline; filename = sample.pdf");
            Response.AddHeader("Content - Type", "application / pdf");
            Response.ClearHeaders();
            Response.AddHeader("Content - Length", renderedBytes.Length.ToString());
            return new FileContentResult(renderedBytes, "application/pdf");
        }




    }
}