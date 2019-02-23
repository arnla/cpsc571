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
        public int Id { get; set; }
        public String FullText { get; set; }
    }
}