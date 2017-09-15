using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAppReportTest.Models
{
    public class SearchParameterModel
    {

        [Display(Name = "Buscar por Id")]
        public string Id
        {
            get;

            set;
        }
        public string Format
        {
            get;

            set;
        }
    }
}