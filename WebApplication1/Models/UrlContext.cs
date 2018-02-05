using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class UrlContext : DbContext
    {
        public UrlContext() :base("DbConnection")
        { }

        public DbSet<UrlInfo> Url { get; set; }
    }
}