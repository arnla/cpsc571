using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace cpsc571.Models
{
    public class Tweet
    {
        
        [Key]
        public String Keyword { get; set; }

        public int Count { get; set; }
    }
}