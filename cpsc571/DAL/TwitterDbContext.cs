using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using cpsc571.Models;

namespace cpsc571.DAL
{
    public class TwitterDbContext : DbContext
    {
        public TwitterDbContext() : base("TwitterDbContext")
        {

        }

        public DbSet<Tweet> Tweets { get; set; }
    }
}