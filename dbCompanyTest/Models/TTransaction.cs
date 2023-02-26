using System;
using System.Collections.Generic;

namespace dbCompanyTest.Models
{
    public partial class TTransaction
    {
        public int Fid { get; set; }
        public string? FDate { get; set; }
        public int? FCustomerId { get; set; }
        public int? FProductId { get; set; }
        public int? FCount { get; set; }
        public decimal? FPrice { get; set; }
    }
}
