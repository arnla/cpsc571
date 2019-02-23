using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using cpsc571.Models;

namespace cpsc571.DAL
{
    public class DataInitializer : DropCreateDatabaseAlways<TwitterDbContext>
    {
        protected override void Seed(TwitterDbContext context)
        {
            
        }
    }
}