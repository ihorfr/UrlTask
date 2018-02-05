using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class UrlInfoModel
    {
        public string URL { get; set; }
        public string LastRequestDate { get; set; }
        public string LastTitle { get; set; }
        public string LastStatus { get; set; }
    }
}