using System;

namespace WebApplication1.Models
{
    public class UrlInfo
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public DateTime LastRequestDate { get; set; }
        public string LastTitle { get; set; }
        public string LastStatus { get; set; }
    }
}