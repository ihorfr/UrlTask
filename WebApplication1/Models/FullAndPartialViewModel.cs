using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class FullAndPartialViewModel
    {
        public int CategoryId { get; set; }
        public List<UrlInfo> Products { get; set; }
        public List<UrlInfo> CategoryList { get; set; }
    }
}